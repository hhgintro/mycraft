using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
		protected float PowerEfficiency 
		{ get 
			{ 
				if (this._powerGridComponent?.basePowerDraw > 0)
				{
					return (_powerGridComponent?.grid?.Efficiency) ?? 0f;

				} else
				return 1f;
			} 
		}

		//public int _itemid;   //ItemBase.id
		public MyCraft.ItemBase _itembase;
		public bool _IsWorking { get; protected set; }


		private void Awake()
		{
			_powerGridComponent ??= GetComponent<PowerGridComponent>();
			_sRef ??= GetComponent<SerializationReference>();
		}

		private void OnValidate()
		{
			_powerGridComponent ??= GetComponent<PowerGridComponent>();
			_sRef ??= GetComponent<SerializationReference>();
		}

		public virtual void ProcessLoop() { }


		#region SAVE
		public virtual void Save(BinaryWriter writer)
		{
			writer.Write(this._sRef.resourcesPath);
			writer.Write(this._itembase.id);
			writer.Write(this._sRef.GUID.ToString());
			Debug.Log($"SAVE:{this._sRef.resourcesPath}/{this._itembase.id}/{this._sRef.GUID.ToString()}");
		}
		public virtual void Load(BinaryReader reader)
		{
			//this._sRef.resourcesPath = reader.ReadString();
			int itemid = reader.ReadInt32();
			string guid = reader.ReadString();

			_itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(itemid);
			Debug.Log($"READ:{this._sRef.resourcesPath}/{this._itembase.id}/{this._sRef.GUID.ToString()}");
			this._sRef.GUID = Guid.Parse(guid);
		}
		#endregion //..SAVE

	}
}