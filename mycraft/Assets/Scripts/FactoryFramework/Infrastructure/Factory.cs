using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyCraft;
using UnityEngine.Windows;
using System.IO;
using UnityEngine.UI;

namespace FactoryFramework
{
	//Forge
	public class Factory : Building, IInput, IOutput
	{
		private Dictionary<int/*itemid*/, int/*amount*/> _inputs    = new Dictionary<int, int>();

		//progress�� ���� ����� ���� ������ ID
		private ItemBase _recipe = null;
		//public Recipe[] validRecipes;
		//public Recipe[] invalidRecipes;


		Dictionary<int, int> INPUT		=> this._inputs;
		MyCraft.BuildingPanel OUTPUT	=> base._panels[0];

		MyCraft.Progress PROGRESS		=> _progresses[0];
		//MyCraft.Progress FUEL_PROGRESS  => _progresses[1];

		////outline
		//Material outline;
		//Renderer renderers;
		//List<Material> materials = new List<Material>();
		int cnt = 0;

		void Start()
		{
			Init();
		}
		public override void Init()
		{
			////outline
			//this.outline = new Material(Shader.Find("Draw/OutlineShader"));
			//this.renderers = this.transform.GetComponent<Renderer>();

			if (0 == base._panels.Count)
			{
				//base._panels.Add(new MyCraft.BuildingPanel(this._panels.Count, 0));// base._itembase._assembling.inputs));//input
				base._panels.Add(new MyCraft.BuildingPanel(this._panels.Count+1, 1));//output
				base._panels.Add(new MyCraft.BuildingPanel(this._panels.Count+1, ((MachineItemBase)base._itembase)._assembling.chips));//chip
			}

			if (0 == base._progresses.Count)
			{
				this._progresses.Add(new Progress(this, (MyCraft.PROGRESSID)0, 1f, false));//progress
				//this._progresses.Add(new Progress(this, (MyCraft.PROGRESSID)1, 10, true));//progress-fuel
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
			if(null == this._recipe)
			{
				Managers.Game.SkillInvens.LinkInven(this, INPUT, base._panels, this._progresses, false);
				//active
				Managers.Game.SkillInvens.gameObject.SetActive(true);
				//de-active
				Managers.Game.Inventories.gameObject.SetActive(false);
				Managers.Game.ChestInvens.gameObject.SetActive(false);
				Managers.Game.FactoryInvens.gameObject.SetActive(false);
				Managers.Game.ForgeInvens.gameObject.SetActive(false);
				return;
			}

			Managers.Game.FactoryInvens.LinkInven(this, INPUT, base._panels, this._progresses, false);
			//active
			Managers.Game.Inventories.gameObject.SetActive(true);
			Managers.Game.FactoryInvens.gameObject.SetActive(true);
			Managers.Game.SkillInvens.gameObject.SetActive(true);
			//de-active
			Managers.Game.ChestInvens.gameObject.SetActive(false);
			Managers.Game.ForgeInvens.gameObject.SetActive(false);

			if(0 == (++cnt %2))
			{
				//outline OFF
				base.OutLine(false);
				////this.renderers = this.transform.GetComponent<Renderer>();
				//this.materials.Clear();
				//this.materials.AddRange(this.renderers.sharedMaterials);
				//this.materials.Remove(outline);
				//this.renderers.materials = this.materials.ToArray();
			}
			else
			{
				//outline ON
				base.OutLine(true);
				////this.renderers = this.transform.GetComponent<Renderer>();
				//this.materials.Clear();
				//this.materials.AddRange(this.renderers.sharedMaterials);
				//this.materials.Add(outline);
				//this.renderers.materials = this.materials.ToArray();
			}
		}

		////bOnOff: true�̸� ON, false�̸� OFF
		//public virtual void OutLine(bool bOnOff)
		//{
		//	this.materials.Clear();
		//	this.materials.AddRange(this.renderers.sharedMaterials);
		//	if (true == bOnOff)		this.materials.Add(outline);
		//	else					this.materials.Remove(outline);
		//	this.renderers.materials = this.materials.ToArray();
		//}

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

		//machine�� output=null����
		public override void OnReset()
		{
			this._recipe = null;

			//input
			foreach(int itemid in INPUT.Keys)
				if (0 < INPUT[itemid]) Managers.Game.AddItem(itemid, INPUT[itemid]);
			INPUT.Clear();

			base.OnReset();
		}

		public override bool AssignRecipe(ItemBase itembase)
		{
			if (null == itembase || null == itembase.cost)
			{
				Debug.LogError("Fail: AssignRecipe is null");
				return false;
			}

			if (null != this._recipe)		return false;
			if (itembase == this._recipe)	return false;

			//���
			this._recipe = itembase;

			//input
			foreach(BuildCostItem cost in this._recipe.cost.items)
				INPUT.Add(cost.itemid, 0);
			//output
			OUTPUT._slots[0]._itemid = this._recipe.id;
			OUTPUT._slots[0]._amount = 0;
			return true;
		}

		public override void OnProgressCompleted(MyCraft.PROGRESSID id)
		{
			if (MyCraft.PROGRESSID.Progress != id) return;

			//�������� ������ �ش�.
			if (false == CreateOutputProducts())
			{
				this._IsWorking = false;
				return;
			}
            //��������,���� ������� üũ
            if (false == CanStartProduction())
            {
                this._IsWorking = false;
                return;
            }
            ConsumeInputs();    //�������� �Ҹ�
        }

        bool StartAssembling()
		{
			//�̹� �۵���...
			if (true == base._IsWorking) return false;

			//��������,���� ������� üũ
			if (false == CanStartProduction()) return false;

			//check pregress
			if (this._progresses.Count < 1) return false;

			//ConsumeFuels();     //����Ҹ�
			ConsumeInputs();    //�������� �Ҹ�

			base._IsWorking = true;
			return true;
		}

		protected bool CanStartProduction()
		{
			if (null == this._recipe) return false;

			//check...�ڿ�
			foreach(BuildCostItem cost in this._recipe.cost.items)
			{
				if (false == INPUT.ContainsKey(cost.itemid)) return false;	//�̵�ϵ� ������
				if (INPUT[cost.itemid] < cost.amount) return false;			//�ڿ��� ����
			}

			if (OUTPUT.IsFull()) return false;  //output�� ���� á����...�ߴ�.
			return true;
			//need a recipe to make!
		}
		//��� �Ҹ�
		private void ConsumeInputs()
		{
			//������ ����
			int s = 0;
			foreach (BuildCostItem cost in this._recipe.cost.items)
			{
				INPUT[cost.itemid] -= cost.amount;
				//UI
				this.SetBlock2Inven(0, s, cost.itemid, INPUT[cost.itemid], false);
				++s;
			}
		}
		private bool CreateOutputProducts()
		{
			if (base._panels.Count < 1) return false;
			OUTPUT._slots[0]._amount += 1;
			//UI
			this.SetBlock2Inven(OUTPUT._slots[0]._panel, OUTPUT._slots[0]._slot, OUTPUT._slots[0]._itemid, OUTPUT._slots[0]._amount, false);
			return true;
		}

		public override void SetItem(int panel, int slot, int itemid, int amount)
		{
			//input�� Dictionary ������� ó���Ѵ�.
			if (0 == panel)	INPUT[itemid] = amount;
			//output�� BuildingPanel ��� ����
			else			base.SetItem(panel-1, slot, itemid, amount);

			//�������� ��ϵǸ� ���� ���θ� �Ǵ��մϴ�.
			this.StartAssembling();
		}

		//block�� id�� �������� ���� �� �ִ��� üũ
		public override bool CheckPutdownGoods(int panel, int slot, int itemid)
		{
			switch (panel)
			{
				case 0:
					if (INPUT.ContainsKey(itemid)) return true;    //���� �� �ִ�.
					break;
			}
			return false;//���� �� ����.
		}

		#region GIVE_OUTPUT
		public bool CanGiveOutput()
		{
			if (0 == OUTPUT._slots[0]._itemid || OUTPUT._slots[0]._amount <= 0) return false;
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
			if (false == TakeInputItem(itemid)) return;

			//�������� ��ϵǸ� ���� ���θ� �Ǵ��մϴ�.
			this.StartAssembling();
		}

		private bool TakeInputItem(int itemid)
		{
			if (false == CanTakeInput(itemid)) return false;
			if (false == INPUT.ContainsKey(itemid)) return false;

			int s = 0;
			foreach (var key in INPUT.Keys)
			{
				if (itemid == key) break;
				++s;
			}
			INPUT[itemid] += 1;
			//UI
			this.SetBlock2Inven(0, s, itemid, INPUT[itemid], false);
			return true;
		}

		public bool CanTakeInput(int itemid)
		{
			if (0 == itemid) return false;
			if (base._panels.Count < 1) return false;

			//input ������ �������� üũ
			if(false == INPUT.ContainsKey(itemid)) return false;
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

		#region SAVE
		public override void Save(BinaryWriter writer)
		{
			base.Save(writer);

			writer.Write((null!=this._recipe) ? this._recipe.id : 0);
		}
		public override void Load(BinaryReader reader)
		{
			base.Load(reader);

			AssignRecipe(MyCraft.Managers.Game.ItemBases.FetchItemByID(reader.ReadInt32()));
		}
		#endregion //..SAVE

	}
}