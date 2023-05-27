using MyCraft;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

namespace FactoryFramework
{
	public class Building : LogisticComponent
	{
		public UnityEvent<Building> OnBuildingDestroyed;

		public InputSocket[] inputSockets;
		public OutputSocket[] outputSockets;

		protected MyCraft.InvenBase _inven;

		public List<MyCraft.BuildingPanel> _panels = new List<MyCraft.BuildingPanel>();
		public List<MyCraft.Progress> _progresses = new List<MyCraft.Progress>();

		public virtual void Init()
		{
			base._IsWorking = false;

			for (int i = 0; i < this._progresses.Count; ++i)
				this._progresses[i].InitProgress();
		}

		private void Update()
		{
			//_IsWorking = false;
			this.ProcessLoop();
		}

		//파괴될 때.
		public virtual void OnDeleted()
		{
			if (null != this._inven)
			{
				this._inven.Clear();
				this._inven.gameObject.SetActive(false);
			}

			for (int p = 0; p < this._panels.Count; ++p)
			{
				List<BuildingSlot> slots = this._panels[p]._slots;
				if (null == slots) continue;

				for (int i = 0; i < slots.Count; ++i)
				{
					MyCraft.Managers.Game.AddItem(slots[i]._itemid, slots[i]._amount);
					slots[i].Clear();   //아이템제거
				}
			}
		}
		private void OnDestroy()
		{
			OnBuildingDestroyed?.Invoke(this);
		}

		public virtual void Reset() { }     //machine의 output=null설정
		public virtual bool AssignRecipe(ItemBase itembase) { return false; }
		public virtual void OnProgressCompleted(MyCraft.PROGRESSID id) { }
		public virtual void OnClicked() { }
		public virtual void SetInven(MyCraft.InvenBase inven)
		{
			this._inven = inven;

			for (int i = 0; i < this._progresses.Count; ++i)
				this._progresses[i].SetInven(inven);
		}

		//Block에서 변경된 내용을 Inven에 반영합니다.
		// destroy : amount가 0이면 파괴
		public virtual void SetBlock2Inven(int panel, int slot, int itemid, int amount, bool destroy)
		{
			if (null == this._inven)
				return;
			this._inven.SetItem(panel, slot, itemid, amount, destroy);
		}
		public virtual void SetItem(int panel, int slot, int itemid, int amount)
		{
			if (this._panels[panel]._slots.Count <= slot)
				return;

			if (0 != _panels[panel]._slots[slot]._itemid && _panels[panel]._slots[slot]._itemid != itemid)
			{
				Debug.LogError("error: block different item id");
				return;
			}

			if (amount <= 0)
			{
				//this._panels[panel]._slots[slot]._itemid = 0;
				this._panels[panel]._slots[slot]._amount = 0;
				//Debug.Log("block slot" + slot + ": " + base._panels[panel]._slots[slot].amount);
				return;
			}

			this._panels[panel]._slots[slot]._itemid = itemid;
			this._panels[panel]._slots[slot]._amount = amount;
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


        #region SAVE
		public override void Save(BinaryWriter writer)
		{
			base.Save(writer);

			//position
			MyCraft.Common.WriteVector3(writer, this.transform.position);
			//rotation
			MyCraft.Common.WriteQuaternion(writer, this.transform.rotation);

			//panel count
			//writer.Write(this._panels.Count);
			for (int p = 0; p < this._panels.Count; ++p)
			{
				//1. slot amount
				writer.Write(this._panels[p]._slot);

				//임시 List<> 에 저장
				List<BuildingSlot> items = new List<BuildingSlot>();
				for (int s = 0; s < this._panels[p]._slots.Count; ++s)
				{
					if (this._panels[p]._slots[s]._itemid <= 0) continue;
					if (this._panels[p]._slots[s]._amount <= 0) continue;
					items.Add(new BuildingSlot(0, s) { _panel = 0, _slot = s, _itemid = this._panels[p]._slots[s]._itemid, _amount = this._panels[p]._slots[s]._amount });
				}

				//2. item count
				writer.Write(items.Count);
				//3. item info
				for (int i = 0; i < items.Count; ++i)
				{
					writer.Write(items[i]._slot);   //slot
					writer.Write(items[i]._itemid); //item id
					writer.Write(items[i]._amount); //amount
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

			this.Init();

			//panel count
			//if (this._panels.Count <= 0) return;
			for (int p = 0; p < this._panels.Count; ++p)
			{
				//1. slot amount
				int slotAmount = reader.ReadInt32();
				this._panels[p].SetSlots(slotAmount);
				//Debug.Log("slot = " + slotAmount);

				//2. item count
				int itemcount = reader.ReadInt32();
				//Debug.Log("itemcount = " + itemcount);
				//3. item info
				for (int i = 0; i < itemcount; ++i)
				{
					int slot = reader.ReadInt32();
					int id = reader.ReadInt32();
					int amount = reader.ReadInt32();
					//Debug.Log("slot[" + slot + "], id[" + id + "], amount[" + amount + "]");

					SetItem(p, slot, id, amount);
				}
			}
        }
        #endregion //..SAVE
    }
}