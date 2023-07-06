using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyCraft;
using static UnityEditor.Rendering.CameraUI;
using UnityEngine.Windows;

namespace FactoryFramework
{
	//Forge
	public class Forge : Building, IInput, IOutput
	{
		MyCraft.BuildingPanel INPUT     => base._panels[0];
		MyCraft.BuildingPanel FUEL      => base._panels[1];
		MyCraft.BuildingPanel OUTPUT    => base._panels[2];

		MyCraft.Progress PROGRESS       => _progresses[0];
		MyCraft.Progress FUEL_PROGRESS  => _progresses[1];

		void Start()
		{
			Init();
		}
		public override void Init()
		{
			if (0 == base._panels.Count)
			{
				base._panels.Add(new MyCraft.BuildingPanel(base._panels.Count, ((FurnaceItemBase)base._itembase)._furnace.inputs));//input
				base._panels.Add(new MyCraft.BuildingPanel(base._panels.Count, ((FurnaceItemBase)base._itembase)._furnace.fuels));//fuel
				base._panels.Add(new MyCraft.BuildingPanel(base._panels.Count, ((FurnaceItemBase)base._itembase)._furnace.outputs));//output
			}

			if (0 == base._progresses.Count)
			{
				base._progresses.Add(new Progress(this, (MyCraft.PROGRESSID)0, 1f, false));//progress
				base._progresses.Add(new Progress(this, (MyCraft.PROGRESSID)1, 10f, true));//progress-fuel
			}
			base.Init();
		}

		public override void ProcessLoop()
		{
			if (false == base._IsWorking) return;

			for (int i = 0; i < this._progresses.Count; ++i)
				this._progresses[i].Update();
		}

		public override void OnClicked()
		{
			Managers.Game.ForgeInvens.LinkInven(this, base._panels, this._progresses, false);
			//active
			Managers.Game.Inventories.gameObject.SetActive(true);
			Managers.Game.ForgeInvens.gameObject.SetActive(true);
			Managers.Game.SkillInvens.gameObject.SetActive(true);
			//de-active
			Managers.Game.ChestInvens.gameObject.SetActive(false);
			Managers.Game.FactoryInvens.gameObject.SetActive(false);
		}

		public override void SetItem(int panel, int slot, int itemid, int amount)
		{
			base.SetItem(panel, slot, itemid, amount);

			//�������� ��ϵǸ� ���� ���θ� �Ǵ��մϴ�.
			this.StartAssembling();
		}
		//public void ClearInternalStorage()
		//{
		//    //HG[2023.06.09] Item -> MyCraft.ItemBase
		//    //_inputs = new Dictionary<Item, int>();
		//    _inputs = new Dictionary<MyCraft.ItemBase, int>();
		//}
		//public bool AssignRecipe(Recipe recipe, bool clearStorage =false)
		//{
		//    this.recipe = recipe;
		//    if (clearStorage)
		//        ClearInternalStorage();
		//    return true;
		//}

		//public void ResetRecipe()
		//{
		//    //input
		//    foreach(int itemid in _inputs.Keys)  MyCraft.Managers.Game.AddItem(itemid, _inputs[itemid]);
		//    _inputs.Clear();
		//    //output
		//    foreach(int itemid in _outputs.Keys) MyCraft.Managers.Game.AddItem(itemid, _outputs[itemid]);
		//    _outputs.Clear();
		//}
		private void AssignRecipe(int itemid)
		{
			if (0 == itemid)
			{
				Debug.LogError($"Fail: AssignRecipe(itemid:{itemid})");
				return;
			}
			//���ٸ�...����
			if (itemid == OUTPUT._slots[0]._itemid) return;
			//�ٸ���...�������� ���������� ����
			if (0 < OUTPUT._slots[0]._amount)
			{
				Debug.LogError($"Fail: AssignRecipe(diff itemid:{itemid}/{OUTPUT._slots[0]._itemid})");
				return;
			}
			//���
			OUTPUT._slots[0]._itemid = itemid;
		}

		public override void OnProgressCompleted(MyCraft.PROGRESSID id)
		{
			if (MyCraft.PROGRESSID.Progress == id)
			{
				//�������� ������ �ش�.
				if (false == CreateOutputProducts())
				{
					this._IsWorking = false;
					return;
				}
				ConsumeInputs();    //�������� �Ҹ�
				return;
			}

			//�ڿ�/���ᰡ ������ null, ������ �ش� slot�� �����մϴ�.
			if (0 == FUEL._slots[0]._itemid || 0 == FUEL._slots[0]._amount)
			{
				this._IsWorking = false;
				return;
			}
			ConsumeFuels();     //����Ҹ�
		}

		private bool StartAssembling()
		{
			//�̹� �۵���...
			if (true == base._IsWorking) return false;

			//��������,���� ������� üũ
			if (false == CanStartProduction()) return false;

			//check pregress
			if (this._progresses.Count < 2) return false;

			ConsumeFuels();     //����Ҹ�
			ConsumeInputs();    //�������� �Ҹ�

			base._IsWorking = true;
			return true;
		}

		private bool CanStartProduction()
		{
			//check fuel
			if(0 == FUEL._slots[0]._itemid || 0 == FUEL._slots[0]._amount)
				return false;//���ᰡ ������...�ߴ�

			//check...�ڿ�
			for (int s = 0; s < INPUT._slots.Count; ++s)
			{
				if (0 == INPUT._slots[s]._itemid || 0 == INPUT._slots[s]._amount)
					return false;//�ڿ��� ������...�ߴ�
			}

			//���� ������ �������� output�� ��ϵ� �����۰� ������ ���� üũ�մϴ�.
			if (0 < OUTPUT._slots[0]._itemid)
			{
				List<MyCraft.FurnaceInputItem> inputs = ((MyCraft.FurnaceItemBase)base._itembase)._furnace.input;
				for (int i = 0; i < inputs.Count; ++i)
				{
					if (INPUT._slots[0]._itemid == inputs[i].itemid)    //���Ե� ���������� üũ
						if (OUTPUT._slots[0]._itemid != inputs[i].output) return false; //���� ������ �������� output�� ��ϵ� �����۰� �ٸ���.
				}
			}

			if (OUTPUT.IsFull()) return false;  //output�� ���� á����...�ߴ�.
			return true;
			//need a recipe to make!
		}
		//��� �Ҹ�
		private void ConsumeInputs()
		{
			//output ����(�Ҹ�Ǵ� ������(intput.Key)�� ���� �����ȴ�.)
			int itemid = INPUT._slots[0]._itemid;//input
			//����� �� �ִ� �������̸�...recipe�� ������ �ݴϴ�.
			List<FurnaceInputItem> inputs = ((FurnaceItemBase)base._itembase)._furnace.input;
			for (int i = 0; i < inputs.Count; ++i)
			{
				if (itemid == inputs[i].itemid)
				{
					AssignRecipe(inputs[i].output);
					PROGRESS.SetTime(inputs[i].build_time);
					break;
				}
			}

			//������ ����
			INPUT._slots[0]._amount -= 1;
			//UI
			this.SetBlock2Inven(INPUT._slots[0]._panel, INPUT._slots[0]._slot, INPUT._slots[0]._itemid, INPUT._slots[0]._amount, false);
		}
		//���� �Ҹ�
		private void ConsumeFuels()
		{
			//�Ҹ��ϴ� �ڿ��� ���� bunning-time�� �����մϴ�.
			List<FurnaceFuelItem> fuels = ((FurnaceItemBase)base._itembase)._furnace.fuel;
			for (int i = 0; i < fuels.Count; ++i)
			{
				if (FUEL._slots[0]._itemid == fuels[i].itemid)
				{
					FUEL_PROGRESS.SetTime(fuels[i].burning_time);
					break;
				}
			}
			//������ ����
			FUEL._slots[0]._amount -= 1;
			//UI
			this.SetBlock2Inven(FUEL._slots[0]._panel, FUEL._slots[0]._slot, FUEL._slots[0]._itemid, FUEL._slots[0]._amount, false);
		}
		//false:����á�ų�, ������ �Ұ����Ҷ�. true:��ӻ��������ϴ�.
		private bool CreateOutputProducts()
		{
			if (base._panels.Count < 3) return false;
			OUTPUT._slots[0]._amount += 1;
			//UI
			this.SetBlock2Inven(OUTPUT._slots[0]._panel, OUTPUT._slots[0]._slot, OUTPUT._slots[0]._itemid, OUTPUT._slots[0]._amount, false);
			return true;
		}

		#region GIVE_OUTPUT
		public bool CanGiveOutput()
		{
			if (0 == OUTPUT._slots[0]._itemid || 0 == OUTPUT._slots[0]._amount) return false;
			return true;
		}

		//public Item OutputType()
		//{
		//    foreach (KeyValuePair<Item, int> availableOutput in _outputs)
		//    {
		//        if (availableOutput.Value > 0) return availableOutput.Key;
		//    }
		//    return null;
		//}
		public int OutputType()
		{
			return OUTPUT._slots[0]._itemid;
		}

		public int GiveOutput()
		{
			//������ ����
			OUTPUT._slots[0]._amount -= 1;
			//UI
			this.SetBlock2Inven(OUTPUT._slots[0]._panel, OUTPUT._slots[0]._slot, OUTPUT._slots[0]._itemid, OUTPUT._slots[0]._amount, false);

			//�������� ��ϵǸ� ���� ���θ� �Ǵ��մϴ�.
			this.StartAssembling();

			return OUTPUT._slots[0]._itemid;
		}
		#endregion //..GIVE_OUTPUT

		#region TAKE_INPUT
		public void TakeInput(int itemid)
		{
			if (0 == itemid) return;

			if (false == TakeInputItem(itemid))
				TakeInputFuel(itemid);

			//�������� ��ϵǸ� ���� ���θ� �Ǵ��մϴ�.
			this.StartAssembling();
		}

		private bool TakeInputItem(int itemid)
		{
			if (false == CanTakeInputItem(itemid)) return false;

			if (0 == INPUT._slots[0]._itemid)
			{
				INPUT._slots[0]._itemid = itemid;
				INPUT._slots[0]._amount = 1;
				return true;
			}
			if (itemid != INPUT._slots[0]._itemid) return false;
			INPUT._slots[0]._amount += 1;
			//UI
			this.SetBlock2Inven(INPUT._slots[0]._panel, INPUT._slots[0]._slot, INPUT._slots[0]._itemid, INPUT._slots[0]._amount, false);
			return true;
		}

		private bool TakeInputFuel(int itemid)
		{
			if (false == CanTakeInputFuel(itemid)) return false;
			if (0 == FUEL._slots[0]._itemid)
			{
				FUEL._slots[0]._itemid = itemid;
				FUEL._slots[0]._amount = 1;
				return true;
			}
			if (itemid != FUEL._slots[0]._itemid) return false;
			FUEL._slots[0]._amount += 1;
			//UI
			this.SetBlock2Inven(FUEL._slots[0]._panel, FUEL._slots[0]._slot, FUEL._slots[0]._itemid, FUEL._slots[0]._amount, false);
			return true;

		}

		public bool CanTakeInput(int itemid)
		{
			if (0 == itemid) return false;

			//input ������ �������� üũ
			if (true == CanTakeInputItem(itemid))
				return true;

			//_fuels
			if (true == CanTakeInputFuel(itemid))
				return true;

			return false;
		}

		private bool CanTakeInputItem(int itemid)
		{
			if (base._panels.Count < 3) return false;
			//������ �������� üũ
			if (0 == INPUT._slots[0]._itemid)
			{
				List<FurnaceInputItem> inputs = ((FurnaceItemBase)base._itembase)._furnace.input;
				for (int i = 0; i < inputs.Count; ++i)
					if (itemid == inputs[i].itemid) return true;    //���԰���
				return false;
			}
			//diff itemid
			if (itemid != INPUT._slots[0]._itemid) return false;
			//is full
			MyCraft.ItemBase itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(itemid);
			if (itembase.Stackable <= INPUT._slots[0]._amount) return false;
			return true;
		}
		private bool CanTakeInputFuel(int itemid)
		{
			if (base._panels.Count < 3) return false;
			//������ �������� üũ
			if (0 == FUEL._slots[0]._itemid)
			{
				List<FurnaceFuelItem> fuels = ((FurnaceItemBase)base._itembase)._furnace.fuel;
				for (int i = 0; i < fuels.Count; ++i)
					if (itemid == fuels[i].itemid) return true;    //���԰���
				return false;
			}
			//diff itemid
			if (itemid != FUEL._slots[0]._itemid) return false;
			//is full
			MyCraft.ItemBase itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(itemid);
			if (itembase.Stackable <= FUEL._slots[0]._amount) return false;
			return true;
		}
		//..//HG[2023.06.09] Item -> MyCraft.ItemBase
		#endregion //..TAKE_INPUT

		//#region SERIALIZATION_HELPERS
		////@@
		//////HG[2023.06.09] Item -> MyCraft.ItemBase
		//////private List<SerializedItemStack> SerializeField(Dictionary<Item, int> dict)
		//////{
		//////    List<SerializedItemStack> items = new List<SerializedItemStack>();
		//////    foreach(KeyValuePair<Item, int> obj in dict)
		//////    {
		//////        items.Add(new SerializedItemStack(){ itemResourcePath = obj.Key.resourcesPath, amount = obj.Value});
		//////    }
		//////    return items;
		//////}
		////private List<SerializedItemStack> SerializeField(Dictionary<int, int> dict)
		////{
		////    List<SerializedItemStack> items = new List<SerializedItemStack>();
		////    //foreach (KeyValuePair<MyCraft.ItemBase, int> obj in dict)
		////    //{
		////    //    items.Add(new SerializedItemStack() { itemResourcePath = obj.Key.resourcesPath, amount = obj.Value });
		////    //}
		////    return items;
		////}
		//////..//HG[2023.06.09] Item -> MyCraft.ItemBase
		////public SerializedItemStack[] SerializeInputs() => SerializeField(_inputs).ToArray();
		////public SerializedItemStack[] SerializeOutputs() => SerializeField(_outputs).ToArray();

		//////HG[2023.06.09] Item -> MyCraft.ItemBase
		//////private Dictionary<Item, int> DeserializeField(List<SerializedItemStack> items)
		//////{
		//////    Dictionary<Item, int> dict = new Dictionary<Item, int>();
		//////    foreach(var iStack in items)
		//////    {
		//////        dict.Add(Resources.Load<Item>(iStack.itemResourcePath), iStack.amount);
		//////    }
		//////    return dict;
		//////}
		////private Dictionary<int, int> DeserializeField(List<SerializedItemStack> items)
		////{
		////    Dictionary<int, int> dict = new Dictionary<int, int>();
		////    //foreach (var iStack in items)
		////    //{
		////    //    dict.Add(Resources.Load<Item>(iStack.itemResourcePath), iStack.amount);
		////    //}
		////    return dict;
		////}
		//////..//HG[2023.06.09] Item -> MyCraft.ItemBase
		////public void DeserializeInputs(SerializedItemStack[] inputs) => _inputs = DeserializeField(inputs.ToList());
		////public void DeserializeOutputs(SerializedItemStack[] inputs) => _outputs = DeserializeField(inputs.ToList());


		////HG[2023.06.09] Item -> MyCraft.ItemBase
		////private List<SerializedItemStack> SerializeField(Dictionary<Item, int> dict)
		////{
		////    List<SerializedItemStack> items = new List<SerializedItemStack>();
		////    foreach(KeyValuePair<Item, int> obj in dict)
		////    {
		////        items.Add(new SerializedItemStack(){ itemResourcePath = obj.Key.resourcesPath, amount = obj.Value});
		////    }
		////    return items;
		////}
		//private List<SerializedItemStack> SerializeField(List<MyCraft.BuildingPanel> dict)
		//{
		//	List<SerializedItemStack> items = new List<SerializedItemStack>();
		//	//foreach (KeyValuePair<MyCraft.ItemBase, int> obj in dict)
		//	//{
		//	//    items.Add(new SerializedItemStack() { itemResourcePath = obj.Key.resourcesPath, amount = obj.Value });
		//	//}
		//	return items;
		//}
		////..//HG[2023.06.09] Item -> MyCraft.ItemBase
		//public SerializedItemStack[] SerializeInputs() => SerializeField(base._panels).ToArray();
		//public SerializedItemStack[] SerializeOutputs() => SerializeField(base._panels).ToArray();

		////HG[2023.06.09] Item -> MyCraft.ItemBase
		////private Dictionary<Item, int> DeserializeField(List<SerializedItemStack> items)
		////{
		////    Dictionary<Item, int> dict = new Dictionary<Item, int>();
		////    foreach(var iStack in items)
		////    {
		////        dict.Add(Resources.Load<Item>(iStack.itemResourcePath), iStack.amount);
		////    }
		////    return dict;
		////}
		//private List<MyCraft.BuildingPanel> DeserializeField(List<SerializedItemStack> items)
		//{
		//	List<MyCraft.BuildingPanel> dict = new List<MyCraft.BuildingPanel>();
		//	//foreach (var iStack in items)
		//	//{
		//	//    dict.Add(Resources.Load<Item>(iStack.itemResourcePath), iStack.amount);
		//	//}
		//	return dict;
		//}
		////..//HG[2023.06.09] Item -> MyCraft.ItemBase
		//public void DeserializeInputs(SerializedItemStack[] inputs) => base._panels = DeserializeField(inputs.ToList());
		//public void DeserializeOutputs(SerializedItemStack[] inputs) => base._panels = DeserializeField(inputs.ToList());
		//#endregion

	}
}