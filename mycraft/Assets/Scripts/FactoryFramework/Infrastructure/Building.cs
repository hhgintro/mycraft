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
            // conveyor�� ItemOnBelt������ ������ �� �����ϴ�.
            // building������ ȣ��Ǿ�� �մϴ�.
            base.Init();
		}
		public override void fnStart()
        {
			base._IsWorking = false;
			_materials.Clear();

			//for (int i = 0; i < this._progresses.Count; ++i)
			//	this._progresses[i].InitProgress();	//���� �ʱ�ȭ�� �ʿ䰡 ������? �켱 �ּ�ó��

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

        //DestroyProcess�� ���� ö�ŵɶ� ȣ��(bReturn:true�̸� �κ����� ȸ��)
        public override void OnDeleted(bool bReturn)
		{
			base.Clear();

			if (null != this._inven)
			{
				this._inven.Clear();
				this._inven.gameObject.SetActive(false);
			}

			OutLine(false); //outline ���ش�.

			base.GUID = Guid.Empty; //GUID�ߺ�:save���ϰ� Mem�� ������ GUID�� �����Ѵ�.
            OnDisconnect();
			OnReset(bReturn);
			//conveyor(�ڽ�) ȸ��
			if (bReturn) MyCraft.Managers.Game.AddItem(base._itembase.id, 1, MyCraft.Global.FILLAMOUNT_DEFAULT);
		}
		public virtual void OnDisconnect()
		{
			foreach(var inputSocket in this.inputSockets) inputSocket.Disconnect();
			foreach(var outputSocket in this.outputSockets) outputSocket.Disconnect();
		}
        //machine�� output=null������ ��.(bReturn:true�̸� �κ����� ȸ��)
        public virtual void OnReset(bool bReturn)
		{
			for (int p = 0; p < this._panels.Count; ++p)
			{
				List<MyCraft.BuildingSlot> slots = this._panels[p]._slots;
				if (null == slots) continue;

				for (int i = 0; i < slots.Count; ++i)
				{
					if(bReturn) MyCraft.Managers.Game.AddItem(slots[i]._item._itemid, slots[i]._item._amount, slots[i]._item._fillAmount);
					slots[i].Clear();   //����������
				}
			}
		}

		//��ġ������ collider�� disable ���ѵд�.(ī�޶� �Դٰ��� ����)
		public override void SetEnable_2(bool enable)
		{
			//this.enabled = enable;
			this.GetComponent<BoxCollider>().enabled = enable;

			foreach (var input in inputSockets)		input.GetComponent<BoxCollider>().enabled = enable;
			foreach (var output in outputSockets)	output.GetComponent<BoxCollider>().enabled = enable;
		}

		//socket�� ���� ��ġ����
		public virtual bool LocationCorrectForSocket(RaycastHit hit, ref Vector3 groundPos, ref Vector3 groundDir) { return false; }
		public virtual bool AssignRecipe(MyCraft.JSonDatabase jsondata) { return false; }
		public virtual void OnProgressCompleted(MyCraft.PROGRESSID id) { }  //progress �ϷḦ �뺸�մϴ�.
																			//public virtual void OnProgressReaching(MyCraft.PROGRESSID id) { }   //�߰����� �뺸�մϴ�.(_maxMultiple ȸ����ŭ �뺸�Ѵ�.)
		void LogPowerCable()
		{
			if (false == this.TryGetComponent<PowerGridComponent>(out PowerGridComponent C))
				return;

			Debug.Log($"[{C.grid.ToString().Substring(0,7)}] ���� ({C.Connections.Count})�� - ���·�: {C.grid.Load}/{C.grid.Production}, node({C.grid.nodes.Count})");
		}

		//�������� ����Ǹ� true�� �����մϴ�.
		public virtual bool OnClicked(Building holding)
		{
			LogPowerCable();	//������ ������

			//C: Ŭ���� �ǹ�(click)
			//H: ����ִ� �ǹ�(hold)
			//source: ����ִ� �ǹ��� ����� �ǹ�

			if(null == holding) return false;

			//Ŭ���� �ǹ��� ���°� ���þ�����...����
			if (false == this.TryGetComponent<PowerGridComponent>(out PowerGridComponent C))
				return false;

			//����ִ� �ǹ��� �������̸�...
			if (false == holding.TryGetComponent<PowerPole>(out PowerPole _))
				return false;
			if (false == holding.TryGetComponent<PowerGridComponent>(out PowerGridComponent H))
				return false;

			//����ִ� �����밡 ��𿡵� ����Ǿ� ���� �ʴٸ�...������͸� ������ �� �ֽ��ϴ�.
			if (H.Connections.Count <= 0)
			{
				//Ŭ���� �ǹ��� �������̸�...
				if (this.TryGetComponent<PowerPole>(out PowerPole _))
					H.Connect(C);	//false:����ִ� �ǹ�(������)���� grid���� �������� �ʴ´�.
			}
			else
			{
				//(C��)�������� ���
				//before: source --> H, C ( source�� H������¿��� BŬ���ϸ� )
				//after: source --> C, C --> H ( source�� C�� �����ϰ� H�� C�� �����Ѵ�.(source������ ���´�))

				//(C�� �����밡 �ƴ�)�ǹ��� ���
				//after: source --> C

				PowerGridComponent source = H.Connections.ElementAt(0);
				if (source == C) return false;

				//Ŭ���� �ǹ��� �������̸�...
				if(this.TryGetComponent<PowerPole>(out PowerPole _))
				{
					C.Connect(H);
					//Disconnect�� �ܹ����̶� ���� ȣ���� ��� �Ѵ�.
					source.Disconnect(H);
					H.Disconnect(source);
					source.grid.RemoveNode(H);	//�������� node���� ����. �ǽð� üũ�� �����ϴ�.
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

		//Block���� ����� ������ Inven�� �ݿ��մϴ�.
		// destroy : amount�� 0�̸� �ı�
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

			//HG[2023.07.13]forge�� input�� "��������"�� "ö�������� ��ü�� ���� ���� �ּ�ó����.
			//if ( (0 != _panels[panel]._slots[slot]._item._itemid) && (_panels[panel]._slots[slot]._item._itemid != itemid) )
			//{
			//	Debug.LogError("error: block different item id");
			//	//return;	//HG[2023.07.13]forge�� input�� "��������"�� "ö�������� ��ü�� ���� ���� �ּ�ó����.
			//	// �߸������� ��츦 �����ϱ� ���� LogError()�� �����մϴ�.
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
		//block�� id�� �������� ���� �� �ִ��� üũ
		public virtual bool CheckPutdownGoods(int panel, int slot, int itemid)
		{
			return true;//���� �� �ִ�.
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

		//bOnOff: true�̸� ON, false�̸� OFF
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
				//1. slot amount : //forge�ε�� slot�� 2��ȴ�.
				//writer.Write(this._panels[p]._slot);

				//�ӽ� List<> �� ����
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
				//1. slot amount: //forge�ε�� slot�� 2��ȴ�.
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