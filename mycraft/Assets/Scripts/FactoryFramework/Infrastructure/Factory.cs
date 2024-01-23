using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using MyCraft;
using UnityEngine.Windows;
using System.IO;
using UnityEngine.UI;


namespace FactoryFramework
{
	//Forge
	public class Factory : Building, IInput, IOutput
	{
		//private Dictionary<int/*itemid*/, int/*amount*/> _inputs    = new Dictionary<int, int>();
		private Dictionary<int/*itemid*/, MyCraft.BuildingItem> _inputs = new Dictionary<int, MyCraft.BuildingItem>();
		//private Dictionary<int/*itemid*/, int/*amount*/> _outputs	= new Dictionary<int, int>();

		//progress에 의해 만들어 지는 아이템 ID
		private MyCraft.ItemBase _recipe = null;
		//public Recipe[] validRecipes;
		//public Recipe[] invalidRecipes;


		Dictionary<int, MyCraft.BuildingItem> INPUT		=> this._inputs;
		MyCraft.BuildingPanel OUTPUT					=> base._panels[0];
		//Dictionary<int, int> OUTPUTS	=> this._outputs;

		MyCraft.Progress PROGRESS						=> _progresses[0];
		//MyCraft.Progress FUEL_PROGRESS  => _progresses[1];

		////outline
		//Material outline;
		//Renderer renderers;
		//List<Material> materials = new List<Material>();
		int cnt = 0;	//테스트용

		public override void fnStart()
		{
			////outline
			//this.outline = new Material(Shader.Find("Draw/OutlineShader"));
			//this.renderers = this.transform.GetComponent<Renderer>();

			if (0 == base._panels.Count)
			{
				////base._panels.Add(new MyCraft.BuildingPanel(this._panels.Count, 0));// base._itembase._assembling.inputs));//input
				//base._panels.Add(new MyCraft.BuildingPanel(this._panels.Count+1, 1));//output
				//base._panels.Add(new MyCraft.BuildingPanel(this._panels.Count+1, ((MyCraft.MachineItemBase)base._itembase)._assembling.chips));//chip
				base._panels.Add(new MyCraft.BuildingPanel(1, 0));//output
				base._panels.Add(new MyCraft.BuildingPanel(2, ((MyCraft.MachineItemBase)base._itembase)._assembling.chips));//chip
			}

			if (0 == base._progresses.Count)
			{
				this._progresses.Add(new MyCraft.Progress(this, MyCraft.PROGRESSID.Item, 1f, false));//progress
				//this._progresses.Add(new Progress(this, (MyCraft.PROGRESSID)1, 10, true));//progress-fuel
			}

			base.fnStart();
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
				MyCraft.Managers.Game.SkillInvens.LinkInven(this, _recipe, INPUT, base._panels, this._progresses, false);
				//active
				MyCraft.Managers.Game.SkillInvens.gameObject.SetActive(true);
				//de-active
				MyCraft.Managers.Game.Inventories.gameObject.SetActive(false);
				MyCraft.Managers.Game.ChestInvens.gameObject.SetActive(false);
				MyCraft.Managers.Game.FactoryInvens.gameObject.SetActive(false);
				MyCraft.Managers.Game.ForgeInvens.gameObject.SetActive(false);
				MyCraft.Managers.Game.LabInvens.gameObject.SetActive(false);
				MyCraft.Managers.Game.TurretInvens.gameObject.SetActive(false);
				return;
			}

			MyCraft.Managers.Game.FactoryInvens.LinkInven(this, this._recipe, INPUT, base._panels, this._progresses, false);
			//active
			MyCraft.Managers.Game.Inventories.gameObject.SetActive(true);
			MyCraft.Managers.Game.FactoryInvens.gameObject.SetActive(true);
			MyCraft.Managers.Game.SkillInvens.gameObject.SetActive(true);
			//de-active
			MyCraft.Managers.Game.ChestInvens.gameObject.SetActive(false);
			MyCraft.Managers.Game.ForgeInvens.gameObject.SetActive(false);
			MyCraft.Managers.Game.LabInvens.gameObject.SetActive(false);
			MyCraft.Managers.Game.TurretInvens.gameObject.SetActive(false);
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

		//machine의 output=null설정(bReturn:true이면 인벤으로 회수)
		public override void OnReset(bool bReturn)
		{
			this._recipe = null;

			//input
			if (bReturn)
			{
				foreach (int itemid in INPUT.Keys)
					if (0 < INPUT[itemid]._amount) MyCraft.Managers.Game.AddItem(itemid, INPUT[itemid]._amount, INPUT[itemid]._fillAmount);
			}
			INPUT.Clear();

			base.OnReset(bReturn);
		}

		public override bool AssignRecipe(MyCraft.JSonDatabase jsondata)
		{
			MyCraft.ItemBase itembase = (MyCraft.ItemBase)jsondata;
			if (null == itembase || null == itembase.cost)
			{
				Debug.LogError("Fail: AssignRecipe is null");
				return false;
			}

			if (null != this._recipe)		return false;
			if (itembase == this._recipe)	return false;

			//등록
			this._recipe = itembase;

			//input
			foreach(MyCraft.BuildCostItem cost in this._recipe.cost.items)
				INPUT.Add(cost.itemid, new MyCraft.BuildingItem());
			//output
			//OUTPUT._slots[0]._item._itemid = this._recipe.id;
			//OUTPUT._slots[0]._item._amount = 0;
			OUTPUT.Clear();
			OUTPUT.SetSlots(this._recipe.cost.outputs.Count);	//결과물 등록 slot개수
			for(int i=0; i<this._recipe.cost.outputs.Count; ++i)
			{
				OUTPUT._slots[i]._item._itemid = this._recipe.cost.outputs[i].itemid;
				OUTPUT._slots[i]._item._amount = 0;
			}
			return true;
		}

		public override void OnProgressCompleted(MyCraft.PROGRESSID id)
		{
			if (MyCraft.PROGRESSID.Item != id) return;

			//아이템을 생성해 준다.
			if (false == CreateOutputProducts())
			{
				this._IsWorking = false;
				return;
			}
            //재료아이템,연료 충분한지 체크
            if (false == CanStartProduction())
            {
                this._IsWorking = false;
                return;
            }
            ConsumeInputs();    //재료아이템 소모
        }

        bool StartAssembling()
		{
			//이미 작동중...
			if (true == base._IsWorking) return false;

			//check pregress
			if (this._progresses.Count < 1) return false;

			//재료아이템,연료 충분한지 체크
			if (false == CanStartProduction()) return false;

			//ConsumeFuels();     //연료소모
			ConsumeInputs();    //재료아이템 소모

			base._IsWorking = true;
			return true;
		}

		protected bool CanStartProduction()
		{
			if (null == this._recipe) return false;

			//check...자원
			foreach(var cost in this._recipe.cost.items)
			{
				if (false == INPUT.ContainsKey(cost.itemid)) return false;	//미등록된 아이템
				if (INPUT[cost.itemid]._amount < cost.amount) return false;			//자원이 부족
			}

			//output이 가득 찼으면...중단.
			if (OUTPUT.IsFull()) return false;

			return true;
			//need a recipe to make!
		}
		//재료 소모
		private void ConsumeInputs()
		{
			PROGRESS.SetFillUp(0);   //가득채움

			//아이템 차감
			int s = 0;
			foreach (MyCraft.BuildCostItem cost in this._recipe.cost.items)
			{
				INPUT[cost.itemid]._amount -= cost.amount;
				//UI
				this.SetBlock2Inven(0, s, cost.itemid, INPUT[cost.itemid]._amount, INPUT[cost.itemid]._fillAmount, false);
				++s;
			}
		}
		private bool CreateOutputProducts()
		{
			if (base._panels.Count < 1) return false;
			//OUTPUT._slots[0]._item._amount += 1;
			////UI
			//this.SetBlock2Inven(OUTPUT._slots[0]._panel, OUTPUT._slots[0]._slot, OUTPUT._slots[0]._item._itemid, OUTPUT._slots[0]._item._amount, false);

			if(this._recipe.id != OUTPUT._slots[0]._item._itemid)
			{
				//OUTPUT에서 아이템을 수동으로 빼면, slot의 아이템값이 사라지는 거 같다.
				//Debug.LogError($"등록된 recip({this._recipe.id})과 OUTPUT({OUTPUT._slots[0]._item._itemid})이 서로다른 값입니다.");
				OUTPUT._slots[0]._item._itemid = this._recipe.id;
			}
			for(int i=0; i<OUTPUT._slots.Count; ++i)
			{
				OUTPUT._slots[i]._item._amount += 1;
				//UI
				this.SetBlock2Inven(OUTPUT._slots[i]._panel, OUTPUT._slots[i]._slot, OUTPUT._slots[i]._item._itemid, OUTPUT._slots[i]._item._amount, OUTPUT._slots[i]._item._fillAmount, false);
			}
			return true;
		}

		public override void SetItem(int panel, int slot, int itemid, int amount, float fillAmount)
		{
			//input은 Dictionary 방식으로 처리한다.
			if (0 == panel)
			{
				if (INPUT.ContainsKey(itemid)) INPUT[itemid]._amount = amount;
			}
			//output은 BuildingPanel 방식 적용
			else base.SetItem(panel - 1, slot, itemid, amount, fillAmount);

			//아이템이 등록되면 시작 여부를 판단합니다.
			this.StartAssembling();
		}

		//block에 id인 아이템을 넣을 수 있는지 체크
		public override bool CheckPutdownGoods(int panel, int slot, int itemid)
		{
			switch (panel)
			{
				case 0:
					if (INPUT.ContainsKey(itemid)) return true;    //넣을 수 있다.
					break;
			}
			return false;//넣을 수 없다.
		}

		#region GIVE_OUTPUT
		public bool CanGiveOutput(OutputSocket cs = null)
		{
			if(null == cs) return false;
			int slot = GetOutputIndexBySocket(cs);
			if (0 == OUTPUT._slots[slot]._item._itemid || OUTPUT._slots[slot]._item._amount <= 0) return false;
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
		public int OutputType(OutputSocket cs = null)
		{
			if (null == cs) return 0;
			int slot = GetOutputIndexBySocket(cs);
			return OUTPUT._slots[slot]._item._itemid;
		}

		public int GiveOutput(OutputSocket cs = null)
		{
			if (null == cs) return 0;
			int slot = GetOutputIndexBySocket(cs);
			//아이템 차감
			OUTPUT._slots[slot]._item._amount -= 1;
			//UI
			this.SetBlock2Inven(OUTPUT._slots[slot]._panel, OUTPUT._slots[slot]._slot, OUTPUT._slots[slot]._item._itemid, OUTPUT._slots[slot]._item._amount, OUTPUT._slots[slot]._item._fillAmount, false);

			//아이템이 등록되면 시작 여부를 판단합니다.
			this.StartAssembling();

			return OUTPUT._slots[slot]._item._itemid;
		}
		#endregion //..GIVE_OUTPUT

		#region TAKE_INPUT
		public void TakeInput(int itemid)
		{
			if (0 == itemid) return;
			if (false == TakeInputItem(itemid)) return;

			//아이템이 등록되면 시작 여부를 판단합니다.
			this.StartAssembling();
		}

		private bool TakeInputItem(int itemid)
		{
			if (false == CanTakeInput(itemid)) return false;
			//if (false == INPUT.ContainsKey(itemid)) return false;

			int s = 0;
			foreach (var key in INPUT.Keys)
			{
				if (itemid == key) break;
				++s;
			}
			INPUT[itemid]._amount += 1;
			//UI
			this.SetBlock2Inven(0, s, itemid, INPUT[itemid]._amount, INPUT[itemid]._fillAmount, false);
			return true;
		}

		public bool CanTakeInput(int itemid)
		{
			if (0 == itemid) return false;
			if (base._panels.Count < 1) return false;

			//input 투입이 가능한지 체크
			if(false == INPUT.ContainsKey(itemid)) return false;
			if(this._recipe.Stackable <= INPUT[itemid]._amount) return false;	//최대량 초과

			foreach(var item in this._recipe.cost.items)
			{
				if (item.itemid != itemid) continue;
				if (item.amount * MyCraft.Common.INPUT_ALLOW_RATE < INPUT[itemid]._amount) return false;		//INPUT은 n배수 까지만
			}
			return true;
		}
		//..//HG[2023.06.09] Item -> MyCraft.ItemBase
		#endregion //..TAKE_INPUT

		#region SERIALIZATION_HELPERS
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
		#endregion

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