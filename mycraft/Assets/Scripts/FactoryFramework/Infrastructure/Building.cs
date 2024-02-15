//using MyCraft;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

namespace FactoryFramework
{
	public class Building : LogisticComponent
	{
		//public UnityEvent<Building> OnBuildingDestroyed;

		public InputSocket[] inputSockets;
		public OutputSocket[] outputSockets;

		protected MyCraft.InvenBase _inven;

		public List<MyCraft.BuildingPanel> _panels = new List<MyCraft.BuildingPanel>();
		public List<MyCraft.Progress> _progresses = new List<MyCraft.Progress>();

		//outline
		Material _outline;
		Renderer _renderers;
		List<Material> _materials = new List<Material>();


        public override void fnAwake()
		{
            // conveyor는 ItemOnBelt때문에 적용할 수 없습니다.
            // building에서만 호출되어야 합니다.
            base.Init();
		}
		public override void fnStart()
        {
			base._IsWorking = false;
			_materials.Clear();

			//for (int i = 0; i < this._progresses.Count; ++i)
			//	this._progresses[i].InitProgress();	//구지 초기화할 필요가 있을까? 우선 주석처리

			//outline
			_outline = new Material(Shader.Find("Draw/OutlineShader"));
			_renderers = this.transform.GetComponent<Renderer>();
			
			base.fnStart();
		}

		//private void Update()
		//{
		//	//_IsWorking = false;
		//	base.ProcessLoop();
		//}
		//public override void fnDestroy()
		//{
		//	//OnBuildingDestroyed?.Invoke(this);
		//}

        //DestroyProcess에 의해 철거될때 호출(bReturn:true이면 인벤으로 회수)
        public override void OnDeleted(bool bReturn)
		{
			base.Clear();

			if (null != this._inven)
			{
				this._inven.Clear();
				this._inven.gameObject.SetActive(false);
			}

			OutLine(false); //outline 꺼준다.

			base.GUID = Guid.Empty; //GUID중복:save파일과 Mem에 동일할 GUID가 존재한다.
            OnDisconnect();
			OnReset(bReturn);
			//conveyor(자신) 회수
			if (bReturn) MyCraft.Managers.Game.AddItem(base._itembase.id, 1, MyCraft.Global.FILLAMOUNT_DEFAULT);
		}
		public virtual void OnDisconnect()
		{
			foreach(var inputSocket in this.inputSockets) inputSocket.Disconnect();
			foreach(var outputSocket in this.outputSockets) outputSocket.Disconnect();
		}
        //machine의 output=null설정할 때.(bReturn:true이면 인벤으로 회수)
        public virtual void OnReset(bool bReturn)
		{
			for (int p = 0; p < this._panels.Count; ++p)
			{
				List<MyCraft.BuildingSlot> slots = this._panels[p]._slots;
				if (null == slots) continue;

				for (int i = 0; i < slots.Count; ++i)
				{
					if(bReturn) MyCraft.Managers.Game.AddItem(slots[i]._item._itemid, slots[i]._item._amount, slots[i]._item._fillAmount);
					slots[i].Clear();   //아이템제거
				}
			}
		}

		//설치전에는 collider를 disable 시켜둔다.(카메라 왔다갔다 현상)
		public override void SetEnable_2(bool enable)
		{
			//this.enabled = enable;
			this.GetComponent<BoxCollider>().enabled = enable;

			foreach (var input in inputSockets)		input.GetComponent<BoxCollider>().enabled = enable;
			foreach (var output in outputSockets)	output.GetComponent<BoxCollider>().enabled = enable;
		}

		//socket에 의한 위치보정
		public virtual bool LocationCorrectForSocket(RaycastHit hit, ref Vector3 groundPos, ref Vector3 groundDir) { return false; }
		public virtual bool AssignRecipe(MyCraft.JSonDatabase jsondata) { return false; }
		public virtual void OnProgressCompleted(MyCraft.PROGRESSID id) { }  //progress 완료를 통보합니다.
																			//public virtual void OnProgressReaching(MyCraft.PROGRESSID id) { }   //중간정산 통보합니다.(_maxMultiple 회수만큼 통보한다.)
		void LogPowerCable()
		{
			if (false == this.TryGetComponent<PowerGridComponent>(out PowerGridComponent C))
				return;

			Debug.Log($"[{C.grid.ToString().Substring(0,7)}] 연결 ({C.Connections.Count})개 - 전력량: {C.grid.Load}/{C.grid.Production}, node({C.grid.nodes.Count})");
		}

		//전기줄이 연결되면 true를 리턴합니다.
		public virtual bool OnClicked(Building holding)
		{
			LogPowerCable();	//전기줄 디버깅용

			//C: 클릭한 건물(click)
			//H: 들고있는 건물(hold)
			//source: 들고있는 건물과 연결된 건물

			if(null == holding) return false;

			//클릭한 건물이 전력과 관련없으면...무시
			if (false == this.TryGetComponent<PowerGridComponent>(out PowerGridComponent C))
				return false;

			//들고있는 건물이 전봇대이면...
			if (false == holding.TryGetComponent<PowerPole>(out PowerPole _))
				return false;
			if (false == holding.TryGetComponent<PowerGridComponent>(out PowerGridComponent H))
				return false;

			//들고있는 전봇대가 어디에도 연결되어 있지 않다면...전봇대와만 연결할 수 있습니다.
			if (H.Connections.Count <= 0)
			{
				//클릭한 건물이 전봇대이면...
				if (this.TryGetComponent<PowerPole>(out PowerPole _))
					H.Connect(C);	//false:들고있는 건물(전봇대)과는 grid까지 연결하지 않는다.
			}
			else
			{
				//(C가)전봇대의 경우
				//before: source --> H, C ( source과 H연결상태에서 B클릭하면 )
				//after: source --> C, C --> H ( source과 C를 연결하고 H는 C랑 연결한다.(source연결을 끊는다))

				//(C가 전봇대가 아닌)건물의 경우
				//after: source --> C

				PowerGridComponent source = H.Connections.ElementAt(0);
				if (source == C) return false;

				//클릭한 건물이 전봇대이면...
				if(this.TryGetComponent<PowerPole>(out PowerPole _))
				{
					C.Connect(H);
					//Disconnect는 단방향이라 각각 호출해 줘야 한다.
					source.Disconnect(H);
					H.Disconnect(source);
					source.grid.RemoveNode(H);	//끊어지면 node에서 빼야. 실시간 체크가 가능하다.
                }
				source.Connect(C);
			}
			return true;
		}

		public virtual void SetInven(MyCraft.InvenBase inven)
		{
			_inven = inven;

			for (int i = 0; i < this._progresses.Count; ++i)
				_progresses[i].SetInven(inven);
		}

		//Block에서 변경된 내용을 Inven에 반영합니다.
		// destroy : amount가 0이면 파괴
		public virtual void SetBlock2Inven(int panel, int slot, int itemid, int amount, float fillAmount, bool destroy)
		{
			if (null == _inven)
				return;
			_inven.SetItem(panel, slot, itemid, amount, fillAmount, destroy);
		}
		public virtual void SetItem(int panel, int slot, int itemid, int amount, float fillAmount)
		{
			if (_panels[panel]._slots.Count <= slot)
				return;

			//HG[2023.07.13]forge의 input에 "구리광석"을 "철광석으로 대체할 때를 위해 주석처리함.
			//if ( (0 != _panels[panel]._slots[slot]._item._itemid) && (_panels[panel]._slots[slot]._item._itemid != itemid) )
			//{
			//	Debug.LogError("error: block different item id");
			//	//return;	//HG[2023.07.13]forge의 input에 "구리광석"을 "철광석으로 대체할 때를 위해 주석처리함.
			//	// 잘못들어오는 경우를 인지하기 위해 LogError()는 유지합니다.
			//}

			if (amount <= 0)
			{
				_panels[panel]._slots[slot]._item._itemid = 0;
				_panels[panel]._slots[slot]._item._amount = 0;
				//Debug.Log("block slot" + slot + ": " + base._panels[panel]._slots[slot].amount);
				return;
			}

			_panels[panel]._slots[slot]._item._itemid		= itemid;
			_panels[panel]._slots[slot]._item._amount		= amount;
			_panels[panel]._slots[slot]._item._fillAmount	= fillAmount;
			//Debug.Log("block slot" + slot + ": " + base._panels[panel]._slots[slot].amount);
		}
		//block에 id인 아이템을 넣을 수 있는지 체크
		public virtual bool CheckPutdownGoods(int panel, int slot, int itemid)
		{
			return true;//넣을 수 있다.
		}

		/// <summary>
		/// Get a List of all recipes from Resources
		/// </summary>
		/// <returns></returns>
		protected Recipe[] GetAllRecipes()
		{
			return Resources.LoadAll<Recipe>("");
		}

		/// <summary>
		/// Return the input socket at index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public InputSocket GetInputSocketByIndex(int index)
		{
			if (index < 0 || index >= inputSockets.Length)
				return null;

			return inputSockets[index];
		}

		/// <summary>
		/// Return the output socket at index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public OutputSocket GetOutputSocketByIndex(int index)
		{
			if (index < 0 || index >= outputSockets.Length)
				return null;

			return outputSockets[index];
		}

		/// <summary>
		/// Get the index of a specific input socket if it exists, else return -1
		/// </summary>
		/// <param name="cs"></param>
		/// <returns></returns>
		public int GetInputIndexBySocket(InputSocket cs)
		{
			for (int i = 0; i < inputSockets.Length; i++)
				if (cs.Equals(inputSockets[i]))
					return i;

			return -1;
		}

		/// <summary>
		/// Get the index of a specific output socket if it exists, else return -1
		/// </summary>
		/// <param name="cs"></param>
		/// <returns></returns>
		public int GetOutputIndexBySocket(OutputSocket cs)
		{
			for (int i = 0; i < outputSockets.Length; i++)
				if (cs.Equals(outputSockets[i]))
					return i;

			return -1;
		}

		//bOnOff: true이면 ON, false이면 OFF
		public virtual void OutLine(bool bOnOff)
		{
			if(null == this._renderers) return;

			_materials.Clear();
			_materials.AddRange(_renderers.sharedMaterials);
			if (true == bOnOff) _materials.Add(_outline);
			else this._materials.Remove(_outline);
			_renderers.materials = _materials.ToArray();
		}

		#region SAVE
		public override void Save(BinaryWriter writer)
		{
			base.Save(writer);

			//position
			MyCraft.Common.WriteVector3(writer, transform.position);
			//rotation
			MyCraft.Common.WriteQuaternion(writer, transform.rotation);

			//panel count
			//writer.Write(this._panels.Count);
			for (int p = 0; p < this._panels.Count; ++p)
			{
				//1. slot amount : //forge로드시 slot이 2배된다.
				//writer.Write(this._panels[p]._slot);

				//임시 List<> 에 저장
				List<MyCraft.BuildingSlot> items = new List<MyCraft.BuildingSlot>();
				for (int s = 0; s < _panels[p]._slots.Count; ++s)
				{
					if (_panels[p]._slots[s]._item._itemid <= 0) continue;
					if (_panels[p]._slots[s]._item._amount <= 0) continue;
					items.Add(new MyCraft.BuildingSlot(0, s, _panels[p]._slots[s]._item._itemid, _panels[p]._slots[s]._item._amount, _panels[p]._slots[s]._item._fillAmount));
				}

				//2. item count
				writer.Write(items.Count);
				//3. item info
				for (int i = 0; i < items.Count; ++i)
				{
					writer.Write(items[i]._slot);   //slot
					writer.Write(items[i]._item._itemid); //item id
					writer.Write(items[i]._item._amount); //amount
				}
			}
        }
        public override void Load(BinaryReader reader)
        {
            base.Load(reader);

			//position
			this.transform.position = MyCraft.Common.ReadVector3(reader);
			//rotation
			this.transform.rotation = MyCraft.Common.ReadQuaternion(reader);

			this.fnStart();


			//panel count
			//if (this._panels.Count <= 0) return;
			for (int p = 0; p < _panels.Count; ++p)
			{
				//1. slot amount: //forge로드시 slot이 2배된다.
				//int slotAmount = reader.ReadInt32();
				//this._panels[p].SetSlots(slotAmount);
				//Debug.Log("slot = " + slotAmount);

				//2. item count
				int itemcount = reader.ReadInt32();
				//Debug.Log("itemcount = " + itemcount);
				//3. item info
				for (int i = 0; i < itemcount; ++i)
				{
					int slot			= reader.ReadInt32();
					int id				= reader.ReadInt32();
					int amount			= reader.ReadInt32();
					float fillAmount	= MyCraft.Global.FILLAMOUNT_DEFAULT;
					//Debug.Log("slot[" + slot + "], id[" + id + "], amount[" + amount + "]");

					SetItem(p, slot, id, amount, fillAmount);
				}
			}

			//this.SetEnable(true);
        }
		#endregion //..SAVE
	}
}