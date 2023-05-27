using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyCraft;
using System.IO.IsolatedStorage;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using FactoryFramework;

namespace FactoryFramework
{
	public class SerializeManager : MonoBehaviour
	{
		[SerializeField] private bool _debugInfo;
		public FactorySaveData data;

		// Event is triggered when loading completes
		// Boolean value is returned depending on success
		[SerializeField]
		private UnityEvent<bool> _onLoadComplete;
		public UnityEvent<bool> OnLoadComplete
		{
			get { return _onLoadComplete; }
		}

		// Event is triggered when saving completes
		// Boolean value is returned depending on success
		[SerializeField]
		private UnityEvent<bool> _onSaveComplete;
		public UnityEvent<bool> OnSaveComplete
		{
			get { return _onSaveComplete; }
		}

		private string saveFileDefault = "save.json";
		private string saveFilePath;
		private void Awake()
		{
			//saveFilePath = Application.persistentDataPath + "/";
			saveFilePath = Application.dataPath + "/../save";
		}

		public void Load() => Load(saveFileDefault);
		public async void Load(string path)
		{
			string filePath = Path.Combine(saveFilePath, path);

			if (!File.Exists(filePath))
			{
				OnLoadComplete.Invoke(false);
				return;
			}
			try
			{
				string saveString = File.ReadAllText(filePath);

				// Deserialize data into a FactorySaveData object
				data = JsonUtility.FromJson<FactorySaveData>(saveString);

				// remove all existing buildings and cables
				CableRendererManager.instance?.Clear();
				foreach (SerializationReference obj in GameObject.FindObjectsOfType<SerializationReference>()) Destroy(obj.gameObject);

				// deserialize polymorphic list from string to json to type
				List<BaseSaveData> loadedObjs = new List<BaseSaveData>();
				for (int i =0; i < data.saveData.Length; i++)
				{
					Type type = Type.GetType(data.saveDataTypes[i]);
					loadedObjs.Add((BaseSaveData)JsonUtility.FromJson(data.saveData[i], type));
					//Debug.Log($"loading type of {type.ToString()} with {loadedObjs[i].GetType().Name}"); // debug
				}

				// list of power grids
				// each grid is an adjacency dict node: node's connections
				List<Dictionary<Guid, HashSet<Guid>>> powerGrids = new List<Dictionary<Guid, HashSet<Guid>>>();
				foreach(var strGrid in data.powerGrids)
				{
					var dict = new Dictionary<Guid, HashSet<Guid>>();
					foreach(var line in strGrid.Split('\n'))
					{
						// line by line grab the nodeKey:connectionValues
						string[] splt = line.Split(':');
						var root = new Guid(splt[0]);
						HashSet<Guid> connections = new HashSet<Guid>();
						
						foreach (var s in splt.Last().Split(','))
						{
							// don't add root to itself as a connection!
							if (s == splt[0] || s.Equals(string.Empty)) continue;

							connections.Add(new Guid(s));
						}
						dict.Add(root, connections);
					}
					powerGrids.Add(dict);
				}

				/// steps to recreate objects in level
				// 1. Spawn all objects
				// 2. Connect conveyors to right GUID-identified LogisticComponents
				// 3. connect cables between GUID_identified PowerGridcomponents
				///

				// build a lookup of Guid -> SerializationReference
				// this is used to re-link the conveyor belts to buildings
				Dictionary<Guid, SerializationReference> lookup = new Dictionary<Guid, SerializationReference>();
				// keep conveyors for a seoncd pass once all buildings are setup
				Dictionary<SerializationReference, ConveyorSaveData> conveyors = new Dictionary<SerializationReference, ConveyorSaveData>();

				foreach (BaseSaveData obj in loadedObjs)
				{
					// spawn the prefab
					SerializationReference sRef = InstantiateBuildingData(obj);
					lookup.Add(sRef.GUID, sRef);

					if (obj is ConveyorSaveData)
					{
						conveyors.Add(sRef, obj as ConveyorSaveData);
						continue;
					}

					//set appropriate data
					obj.Deserialize(sRef);
				}
				// second pass to connect all conveyors now
				foreach (KeyValuePair<SerializationReference, ConveyorSaveData> pair in conveyors)
				{
					pair.Value.Deserialize(pair.Key);

					Conveyor conveyor = pair.Key.GetComponent<Conveyor>();
					ConveyorSaveData sData = pair.Value;

					///
					// This part may look confusing. Input sockets only connect to output sockets and vice versa.
					// if inputSocketGUID exists, it is pointing to a building or other conveyor's output socket
					///

					if (sData.inputSocketGUID != null && !sData.inputSocketGUID.Equals(""))
					{
						var inputConnection = lookup[new Guid(sData.inputSocketGUID)];
						if (inputConnection.TryGetComponent(out Building building))
						{
							OutputSocket osocket = building.GetOutputSocketByIndex(sData.outputSocketIndex);
							osocket.Connect(conveyor.inputSocket);
						}
						else if (inputConnection.TryGetComponent(out Conveyor conv))
						{
							conv.outputSocket.Connect(conveyor.inputSocket);
						}
					}
					if (sData.outputSocketGUID != null && !sData.outputSocketGUID.Equals(""))
					{
						var outputConnection = lookup[new Guid(sData.outputSocketGUID)];
						if (outputConnection.TryGetComponent(out Building building))
						{
							InputSocket isocket = building.GetInputSocketByIndex(sData.inputSocketIndex);
							isocket.Connect(conveyor.outputSocket);
						}
						else if (outputConnection.TryGetComponent(out Conveyor conv))
						{
							conveyor.outputSocket.Connect(conv.inputSocket);
						}
					}
				}
				// wait one frame to allow all objects to run their Start() calls
				await Task.Yield(); // FIXME if you want to use webgl

				// finally connect the power grid back together
				foreach(Dictionary<Guid, HashSet<Guid>> pGrid in powerGrids)
				{
					foreach (KeyValuePair<Guid, HashSet<Guid>> adj in pGrid)
					{
						if (false == lookup.ContainsKey(adj.Key)) continue;
						PowerGridComponent a = lookup[adj.Key].GetComponent<PowerGridComponent>();
						if (a == null) continue;
						foreach (Guid connection in adj.Value)
						{
							if (lookup.TryGetValue(connection, out SerializationReference b))
								a.Connect(b.GetComponent<PowerGridComponent>());
						}
					}
				}

				OnLoadComplete.Invoke(true);
			}
			catch (Exception e)
			{
				Debug.LogError("FactoryFramework load failed! - " + e.ToString());
				OnLoadComplete.Invoke(false);
				throw e;
			}
		}
		public void Save() => Save(saveFileDefault);
		public void Save(string path)
		{
			//string filePath = Path.Combine(saveFilePath, path);

			//// collect and sort buildings
			//var serializables = FindObjectsOfType<SerializationReference>();
			//List<BaseSaveData> saveData = new List<BaseSaveData>();
			//List<string> saveDataTypes = new List<string>();

			//// serialize extra data for different types of LogisticComponent objects
			//foreach (var obj in serializables)
			//{
			//	if (obj.TryGetComponent(out Driller producer))
			//	{
			//		saveData.Add(new ProducerSaveData()
			//		{
			//			position = obj.transform.position,
			//			rotation = obj.transform.rotation,
			//			guid = obj.GUID.ToString(),
			//			resourcesPath = obj.resourcesPath,
			//			//itemStack = new SerializedItemStack() { itemResourcePath = producer.resource.itemStack.item.resourcesPath, amount = producer.resource.itemStack.amount},
			//			//overrideMaxStack = producer.resource.overrideMaxStack,
			//			//overrideMaxStackNum = producer.resource.overrideMaxStackNum
			//		});
			//		saveDataTypes.Add(typeof(ProducerSaveData).ToString());
			//	}
			//	else if (obj.TryGetComponent(out Forge processor))
			//	{
			//		saveData.Add(new ProcessorSaveData()
			//		{
			//			position = obj.transform.position,
			//			rotation = obj.transform.rotation,
			//			guid = obj.GUID.ToString(),
			//			resourcesPath = obj.resourcesPath,
			//			//HG[2023.06.09] Item -> MyCraft.ItemBase
			//			//recipeResourcePath = processor.recipe?.resourcesPath ?? null,
			//			currentInputs = processor.SerializeInputs(),
			//			currentOutputs = processor.SerializeOutputs()
			//		});
			//		saveDataTypes.Add(typeof(ProcessorSaveData).ToString());
			//	}
			//	else if (obj.TryGetComponent(out Storage storage))
			//	{
			//		StorageSaveData data = new StorageSaveData();
			//		data.itemid = storage._itembase.id;
			//		data.position = obj.transform.position;
			//		data.rotation = obj.transform.rotation;
			//		data.guid = obj.GUID.ToString();
			//		data.resourcesPath = obj.resourcesPath;

			//		int amount = 0;
			//		for (int p = 0; p < storage._panels.Count; ++p)
			//		{
			//			for (int s = 0; s < storage._panels[p]._slots.Count; ++s)
			//			{
			//				if (storage._panels[p]._slots[s]._itemid <= 0) continue;
			//				if (storage._panels[p]._slots[s]._amount <= 0) continue;
			//				++amount;
			//			}
			//		}
			//		data.slots = new SerializedItemStack[amount];
			//		int i = 0;
			//		for (int p = 0; p < storage._panels.Count; ++p)
			//		{
			//			for (int s = 0; s < storage._panels[p]._slots.Count; ++s)
			//			{
			//				if (storage._panels[p]._slots[s]._itemid <= 0) continue;
			//				if (storage._panels[p]._slots[s]._amount <= 0) continue;
			//				data.slots[i++] = new SerializedItemStack() { panel = p, slot = s, itemid = storage._panels[p]._slots[s]._itemid, amount = storage._panels[p]._slots[s]._amount };
			//			}
			//		}

			//		saveData.Add((BaseSaveData)data);
			//		//saveData.Add(new StorageSaveData()
			//		//{
			//		//    position = obj.transform.position,
			//		//    rotation = obj.transform.rotation,
			//		//    guid = obj.GUID.ToString(),
			//		//    resourcesPath = obj.resourcesPath,
			//		//    //@2026.06.26
			//		//    itemid = storage._itembase.id
			//		//    //slots = storage.storage.Select(istack => new SerializedItemStack() {itemResourcePath = istack.item?.resourcesPath ?? null, amount = istack.amount }).ToArray(),
			//		//    //capacity = storage.capacity
			//		//});
			//		saveDataTypes.Add(typeof(StorageSaveData).ToString());
			//	}
			//	else if (obj.TryGetComponent(out Conveyor conveyor))
			//	{
			//		saveData.Add(new ConveyorSaveData()
			//		{
			//			position = obj.transform.position,
			//			rotation = obj.transform.rotation,
			//			guid = obj.GUID.ToString(),
			//			resourcesPath = obj.resourcesPath,
			//			startPos = conveyor.data.start,
			//			startDir = conveyor.data.startDir,
			//			endPos = conveyor.data.end,
			//			endDir = conveyor.data.endDir,
			//			speed = conveyor.data.speed,
			//			inputSocketGUID = conveyor.InputSocketGuid,
			//			inputSocketIndex = conveyor.data.inputSocketIndex,
			//			outputSocketGUID = conveyor.OutputSocketGuid,
			//			outputSocketIndex = conveyor.data.outputSocketIndex,
			//			items = conveyor.items.Select(i => new ConveyorSaveData.SerializedItem()
			//			{
			//				//HG[2023.06.09] Item -> MyCraft.ItemBase
			//				//resourcesPath = i.item.resourcesPath, position = i.position
			//			}).ToArray()
			//		});
			//		saveDataTypes.Add(typeof(ConveyorSaveData).ToString());
			//	}
			//	else
			//	{
			//		saveData.Add(new BaseSaveData()
			//		{
			//			position = obj.transform.position,
			//			rotation = obj.transform.rotation,
			//			guid = obj.GUID.ToString(),
			//			resourcesPath = obj.resourcesPath
			//		});
			//		saveDataTypes.Add(typeof(BaseSaveData).ToString());
			//	}
			//}

			//// Serialize PowerGrid data
			//List<string> powerGrids = new List<string>();
			//foreach (var grid in FindObjectsOfType<PowerGrid>()){
			//	powerGrids.Add(grid.ToString());
			//}


			//data = new FactorySaveData()
			//{
			//	// cannot serializae polymorphic list, must convert all to string representation
			//	saveData = saveData.Select(x => JsonUtility.ToJson(x)).ToArray(),
			//	saveDataTypes = saveDataTypes.ToArray(),
			//	powerGrids = powerGrids.ToArray()
			//};

			//var jsonString = JsonUtility.ToJson(data, true);

			//File.WriteAllText(filePath, jsonString);
			//print($"saving data to {filePath}");

			//OnSaveComplete.Invoke(true);
		}

		public SerializationReference InstantiateBuildingData(BaseSaveData obj)
		{
			//@2023.06.26
			GameObject prefab = Resources.Load<GameObject>(obj.resourcesPath);
			GameObject instantiated = Instantiate(prefab, obj.position, obj.rotation);
			//GameObject instantiated = Managers.Resource.Instantiate(obj.resourcesPath);
			//instantiated.transform.position = obj.position;
			//instantiated.transform.rotation = obj.rotation;
			if (instantiated.TryGetComponent(out FactoryFramework.LogisticComponent logic))
				logic._itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(obj.itemid);
			//..//@2023.06.26

			SerializationReference sRef = instantiated.GetComponent<SerializationReference>();
			sRef.GUID = new Guid(obj.guid);
			sRef.resourcesPath = obj.resourcesPath;

			return sRef;
		}

		//public void Clear()
		//{
		//    var prefabs = FindObjectsOfType<SerializablePrefab>(true);
		//    foreach (SerializablePrefab p in prefabs)
		//    {
		//        Destroy(p.gameObject);
		//    }
		//}

		private void OnGUI()
		{
			if (!_debugInfo) return;
			GUILayout.Space(100);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Save"))
			{
				this.Save();
			}
			if (GUILayout.Button("Load"))
			{
				this.Load();
			}
			GUILayout.EndHorizontal();
		}

		#region SAVE_DATA_TYPES
		[Serializable]
		public class BaseSaveData
		{
			public int itemid;  //자기자신

			// transsform data for positioning
			public Vector3 position;
			public Quaternion rotation;
			//public Vector3 scale;   // unnecessary?
			// reference data
			public string guid;
			public string resourcesPath;

			public virtual void Serialize(SerializationReference sRef)
			{

			}

			public virtual void Deserialize(SerializationReference sRef)
			{
				return;
			}
		}
		[Serializable]
		public class ProducerSaveData : BaseSaveData
		{
			public SerializedItemStack itemStack;
			public bool overrideMaxStack = false;
			[Min(1)]
			public int overrideMaxStackNum = 1;

			public override void Deserialize(SerializationReference sRef)
			{
				if (sRef.TryGetComponent(out Driller producer))
				{
					//producer.resource.itemStack = new ItemStack() {
					//    item = Resources.Load<Item>(itemStack.itemResourcePath),
					//    amount = itemStack.amount,
					//};
					//producer.resource.overrideMaxStackNum = overrideMaxStackNum;
					//producer.resource.overrideMaxStack = overrideMaxStack;
				}
			}
		}
		[Serializable]
		public class ProcessorSaveData : BaseSaveData
		{
			public string recipeResourcePath;

			public SerializedItemStack[] currentInputs;
			public SerializedItemStack[] currentOutputs;

			public override void Deserialize(SerializationReference sRef)
			{
				if (sRef.TryGetComponent(out Forge processor))
				{
					////HG[2023.06.09] Item -> MyCraft.ItemBase
					////processor.recipe = (recipeResourcePath != null && !recipeResourcePath.Equals(string.Empty)) ?  Resources.Load<Recipe>(recipeResourcePath) : null;
					//processor.DeserializeInputs(currentInputs);
					//processor.DeserializeOutputs(currentOutputs);
				}
			}
		}
		[Serializable]
		public class StorageSaveData : BaseSaveData
		{
			public SerializedItemStack[] slots;
			//public int capacity;
			public override void Deserialize(SerializationReference sRef)
			{
				if (sRef.TryGetComponent(out Storage storageBuilding))
				{
					storageBuilding.Init();
					for(int i=0; i<slots.Length; ++ i)
						storageBuilding.SetItem(slots[i].panel, slots[i].slot, slots[i].itemid, slots[i].amount);

					//storageBuilding.storage = new ItemStack[storage.Length];
					//for (int i =0; i < storage.Length; i++)
					//{
					//    SerializedItemStack s = storage[i];
					//    storageBuilding.storage[i] = new ItemStack() { 
					//        item = (s.itemResourcePath!=null && !s.itemResourcePath.Equals(string.Empty)) ? Resources.Load<Item>(s.itemResourcePath) : null,
					//        amount = s.amount };
					//}
					//storageBuilding.capacity = this.capacity;
				}
			}
		}
		[Serializable]
		public class ConveyorSaveData : BaseSaveData
		{
			public Vector3 startPos;
			public Vector3 startDir;
			public Vector3 endPos;
			public Vector3 endDir;
			public float speed;
			public int capacity;

			// Building that inputs to the conveyor
			public string inputSocketGUID;
			public int inputSocketIndex;

			// Building the conveyor outputs to
			public string outputSocketGUID;
			public int outputSocketIndex;

			[Serializable]
			public class SerializedItem
			{
				public string resourcesPath;
				public float position;
			}

			public SerializedItem[] items;

			public override void Deserialize(SerializationReference sRef)
			{
				if (sRef.TryGetComponent(out Conveyor conveyor))
				{
					conveyor.data.start = startPos;
					conveyor.data.startDir = startDir;
					conveyor.data.end = endPos;
					conveyor.data.endDir = endDir;
					conveyor.data.speed = speed;                    
					conveyor.inputSocket = conveyor.GetComponentInChildren<InputSocket>();
					conveyor.outputSocket = conveyor.GetComponentInChildren<OutputSocket>();
					conveyor.data.inputSocketIndex = inputSocketIndex;
					conveyor.data.outputSocketIndex = outputSocketIndex;
					// draw
					conveyor.UpdateMesh(true);
					conveyor.AddCollider();
					// handle items
					foreach (SerializedItem i in items)
					{
						conveyor.SetItemOnBelt(i.resourcesPath, i.position);
					}
				}
			}
		}

		[Serializable]
		public class FactorySaveData
		{
			// Cannot serialize polymorphic list of BaseSaveData[] so we're saving a bunch of strings
			public string[] saveData;
			public string[] saveDataTypes;

			public string[] powerGrids;
		}
		#endregion
	}
}
