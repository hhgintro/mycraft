using MyCraft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace FactoryFramework
{
	/// <summary>
	/// Base class used for all things that create/process/destroy/transfer materials
	/// </summary>
	/// 
	[RequireComponent(typeof(SerializationReference))]
	public abstract class LogisticComponent : MonoBehaviour
	{
		protected GlobalLogisticsSettings settings { get { return ConveyorLogisticsUtils.settings; } }
		protected SerializationReference _sRef;
		public System.Guid GUID
		{
			get { if (_sRef == null) _sRef ??= GetComponent<SerializationReference>(); return _sRef.GUID; }
			set { _sRef.GUID = value; }
		}

		protected PowerGridComponent _powerGridComponent;
		public float PowerEfficiency
		{ get
			{
				//return 1f;	//@@01 : 전력체크안함

				//if (this._powerGridComponent?.basePowerDraw > 0)
				//	return (this._powerGridComponent?.grid?.Efficiency) ?? 0f;
				//else
				//	return 1f;
				if (null == this._powerGridComponent || this._powerGridComponent.basePowerDraw <= 0)
					return 1f;
				return (this._powerGridComponent?.grid?.Efficiency) ?? 0f;
			}
		}

		protected GameObject _PowerSymbol;

		//public int _itemid;   //ItemBase.id
		public MyCraft.ItemBase _itembase;
		public bool _IsWorking { get; protected set; }


		//original materials
		//토대의 경우, 자신의 material정보를 가지고 있지 않으면,
		// (재사용시)DemoProps의 material로 덮어씌워진다.
		List<Material> _originalMaterials = new List<Material>();		//green,red로 바꾸기전 자신의 원래의 색
		List<MeshRenderer> _targetMeshRenders = new List<MeshRenderer>();		//green,red로 바꿀 대상.

		private void Awake()
		{
			_powerGridComponent ??= GetComponent<PowerGridComponent>();
			_sRef ??= GetComponent<SerializationReference>();
			
			fnAwake();
		}

		private void Start()
		{
			fnStart();
		}

		private void OnValidate()
		{
			_powerGridComponent ??= GetComponent<PowerGridComponent>();
			_sRef ??= GetComponent<SerializationReference>();
		}

		private void Update()
		{
			//if(true == _IsWorking)
			ProcessLoop();


		}

		private void OnDestroy()
		{
			fnDestroy();

			//HG_TEST: 재시작할때 전기가중상태로 인식되어 초기화한다.
			//this._powerGridComponent.grid = null;
		}
		public virtual void fnAwake() { }
		public virtual void fnStart()
		{
			if (null != _powerGridComponent)
			{
				if (null == _PowerSymbol)
				{
					_PowerSymbol = Managers.Resource.Instantiate(Managers.Game.PowerSymbol, this.transform);
					_PowerSymbol.GetComponent<PowerEfficiencyToColor>().SetOwner(this);
				}
			}
        }
        public virtual void ProcessLoop() { }
		public virtual void fnDestroy() { }
		//cancel or 초기화가 필요할때는 Clear()
		//DestroyProcess에 의해 철거될때 OnDeleted(bReturn:true이면 인벤으로 회수)
		public virtual void OnDeleted(bool bReturn) { }

		public virtual void SetEnable_2(bool enable) { }   //설치전에는 collider를 disable 시켜둔다.(카메라 왔다갔다 현상)

		public void Init()
		{
			// conveyor는 ItemOnBelt때문에 적용할 수 없습니다.
			// building에서만 호출되어야 합니다.


			//자신의 original materials
			_originalMaterials.Clear();
			_targetMeshRenders.Clear();
			foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
			{
				//(conveyor 등등.)연결 socket의 이름을 고정("Indicator")하여, material이 바뀌는 것을 막는다.
				if (mr.name == "Indicator") continue;
				_targetMeshRenders.Add(mr);
				_originalMaterials.Add(mr.sharedMaterial);
			}
		}

		//cancel or 초기화가 필요할때는 Clear()
		//DestroyProcess에 의해 철거될때 OnDeleted(bReturn:true이면 인벤으로 회수)
		public virtual void Clear()
		{
			_powerGridComponent?.Clear();
		}

		public virtual void SetMaterials(Material frameMat, Material beltMat = null)
		{
			if (null != frameMat)
			{
				//green or red
				foreach (MeshRenderer mr in _targetMeshRenders)
					mr.sharedMaterial = frameMat;
				return;
			}

			if (0 < _originalMaterials.Count)
			{
				//자신의 원래 material로...
				int index = 0;
				foreach (MeshRenderer mr in _targetMeshRenders)
					mr.sharedMaterial = _originalMaterials[index++];
			}
		}

#if ITEM_MESH_ON_BELT  //virtual GetSharedMesh()                            
		public virtual Mesh GetSharedMesh() {  return this._itembase.prefab.GetComponent<MeshFilter>().sharedMesh; }
		public virtual Material GetSharedMaterial() { return this._itembase.prefab.GetComponent<MeshRenderer>().sharedMaterial; }
#else
#endif //..ITEM_MESH_ON_BELT
		public virtual float GetLocalScale() { return this._itembase.scaleOnBelt; }

		#region SAVE
		public virtual void Save(BinaryWriter writer)
		{
			writer.Write(this._sRef.resourcesPath);
			writer.Write(this._itembase.id);
			writer.Write(this.GUID.ToString());
			//Debug.Log($"SAVE:{this._sRef.resourcesPath}/{this._itembase.id}/{this._sRef.GUID.ToString()}");
		}
		public virtual void Load(BinaryReader reader)
		{
			//this._sRef.resourcesPath = reader.ReadString();
			int itemid = reader.ReadInt32();
			string guid = reader.ReadString();

			_itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(itemid);
			////Debug.Log($"LOAD:{this._sRef.resourcesPath}/{this._itembase.id}/{this._sRef.GUID.ToString()}");
			////this._sRef.GUID = Guid.Parse(guid);
			this.GUID = Guid.Parse(guid);

			_powerGridComponent?.Init();
		}
		#endregion //..SAVE

	}
}