using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MyCraft;
//using MyCraft;


namespace FactoryFramework
{
	//Lab
	public class Lab : Building, IInput//, IOutput
	{
		//private Dictionary<int/*itemid*/, int/*amount*/> _inputs    = new Dictionary<int, int>();
		private Dictionary<int/*itemid*/, MyCraft.BuildingItem> _inputs = new Dictionary<int, MyCraft.BuildingItem>();
		//private Dictionary<int/*itemid*/, int/*amount*/> _outputs	= new Dictionary<int, int>();

		//progress에 의해 만들어 지는 아이템 ID
		//private MyCraft.TechBase _recipe = null;
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

		public override void fnStart()
		{
			////outline
			//this.outline = new Material(Shader.Find("Draw/OutlineShader"));
			//this.renderers = this.transform.GetComponent<Renderer>();

			foreach (var itemid in ((MyCraft.MachineItemBase)base._itembase)._assembling.inputs)
			{
                if(false == INPUT.ContainsKey(itemid))
					INPUT.Add(itemid, new BuildingItem());
			}

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
			if(false == StartAssembling(Time.deltaTime))
			{
				base._IsWorking = false;
			}


			//if (false == base._IsWorking) return;

			//for (int i = 0; i < this._progresses.Count; ++i)
			//	this._progresses[i].Update();
		}

		public override void OnClicked()
		{
			MyCraft.Managers.Game.LabInvens.LinkInven(this, null, INPUT, base._panels, this._progresses, false);
			//active
			MyCraft.Managers.Game.Inventories.gameObject.SetActive(true);
			MyCraft.Managers.Game.LabInvens.gameObject.SetActive(true);
			MyCraft.Managers.Game.SkillInvens.gameObject.SetActive(true);
			//de-active
			MyCraft.Managers.Game.ChestInvens.gameObject.SetActive(false);
			MyCraft.Managers.Game.FactoryInvens.gameObject.SetActive(false);
			MyCraft.Managers.Game.ForgeInvens.gameObject.SetActive(false);
			MyCraft.Managers.Game.TurretInvens.gameObject.SetActive(false);
		}

		////bOnOff: true이면 ON, false이면 OFF
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

		////machine의 output=null설정
		//public override void OnReset()
		//{
		//	this._recipe = null;

		//	//input
		//	foreach(int itemid in INPUT.Keys)
		//		if (0 < INPUT[itemid]._amount) MyCraft.Managers.Game.AddItem(itemid, INPUT[itemid]._amount, INPUT[itemid]._fillAmount);
		//	INPUT.Clear();

		//	base.OnReset();
		//}

		//public override bool AssignRecipe(MyCraft.JSonDatabase jsondata)
		//{
		//	MyCraft.TechBase techbase = (MyCraft.TechBase)jsondata;
		//	if (null == techbase || null == techbase.cost)
		//	{
		//		Debug.LogError("Fail: AssignRecipe is null");
		//		return false;
		//	}

		//	if (null != this._recipe)		return false;
		//	if (techbase == this._recipe)	return false;

		//	//등록
		//	this._recipe = techbase;

		//	StartAssembling();
		//	return true;
		//}

		//public override void OnProgressCompleted(MyCraft.PROGRESSID id)
		//{
		//	if (MyCraft.PROGRESSID.Item != id) return;

		//	//아이템을 생성해 준다.
		//	if (false == CreateOutputProducts())
		//	{
		//		this._IsWorking = false;
		//		return;
		//	}
		//	//재료아이템,연료 충분한지 체크
		//	if (false == CanStartProduction())
		//	{
		//		this._IsWorking = false;
		//		return;
		//	}
		//	ConsumeInputs();    //재료아이템 소모
		//}

		//public override void OnProgressReaching(MyCraft.PROGRESSID id)
		//{
		//	if (MyCraft.PROGRESSID.Item != id) return;
		//}


		bool StartAssembling(float deltaTime)
		{
			//연구 등록되어 있는가?
			TechItemData itemdata = Managers.Game.TechInvens.GetResearch();
			if (itemdata == null) return false;
			TechBase techbase = (TechBase)itemdata.database;
			if (null == techbase) return false;

			////이미 작동중...
			//if (true == base._IsWorking) return false;

			//check pregress
			if (this._progresses.Count < 1)
			{
				Debug.Log($"연구소 progress 개체가 확인되지 않습니다.");
				return false;
			}

			//시간대비-소진률을 계산산다.
			if (techbase.cost.time < 0.01)
			{
				Debug.Log($"연구({techbase.id})의 시간({techbase.cost.time})을 확인해 주세요(너무 적다)");
				return false;
			}
			//시간에 따른 소진율을 계산한다.(개별 아이템의 개수는 아래 함수에서 따로 계산한다.)
			float fillAmount = deltaTime / techbase.cost.time;

			//재료아이템,연료 충분한지 체크
			if (false == CanStartProduction(techbase, ref fillAmount)) return false;

			//ConsumeFuels();     //연료소모
			ConsumeInputs(techbase, fillAmount);    //재료아이템 소모

			//PROGRESS.SetMultiple(this._recipe.multiple, this._recipe.cost.mulitple);
			base._IsWorking = true;
			return true;
		}

		protected bool CanStartProduction(TechBase techbase, ref float fillAmount)
		{
			if (null == techbase) return false;
			if(true == techbase.Learned) return false;	//이미 연구완료

			//check...자원
			foreach (var cost in techbase.cost.items)
			{
				if (false == INPUT.ContainsKey(cost.itemid)) return false;	//미등록된 아이템
				if (INPUT[cost.itemid]._amount <= 0) return false;  //자원이 없다(물약은 다 있을 필요없다(cost.amount)
				//남은 소진률만큼만
				//fillAmount는 아이템개수가 반영되지 않은 값이므로, 아이템개수만큼 곱해줘야, 단위시간당 소진률이다.
				//소진률을 확인했으면, 아이템개수가 반영되지 않는 값으로 돌려준다.(나누기)
				fillAmount = Mathf.Min(fillAmount*cost.amount, INPUT[cost.itemid]._fillAmount) / cost.amount;
			}

			//TechInven에 연구에 필요한 소진률을 확인 받는다.
			fillAmount = Managers.Game.TechInvens.GetFillAmount(fillAmount);
			return true;
		}
		//재료 소모
		private void ConsumeInputs(TechBase techbase, float fillAmount)
		{
			//아이템 차감
			int s = 0;
			foreach (var cost in techbase.cost.items)
			{
				//아이템 소모률 갱신
				INPUT[cost.itemid]._fillAmount -= fillAmount * cost.amount;
				if (INPUT[cost.itemid]._fillAmount < 0.001f)
				{//너무 적은값이면 아이템을 1개 감소시켜준다.
					INPUT[cost.itemid]._amount -= 1;
					INPUT[cost.itemid]._fillAmount = MyCraft.Global.FILLAMOUNT_DEFAULT;
				}
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
			if (0 == panel)	INPUT[itemid]._amount = amount;
			//output은 BuildingPanel 방식 적용
			else			base.SetItem(panel-1, slot, itemid, amount, fillAmount);

			////아이템이 등록되면 시작 여부를 판단합니다.
			//this.StartAssembling();	//HG_TODO: 이벤트방식으로 전환고려할 것(Update()에서 처리중)
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
		//public bool CanGiveOutput(OutputSocket cs = null)
		//{
		//	if(null == cs) return false;
		//	int slot = GetOutputIndexBySocket(cs);
		//	if (0 == OUTPUT._slots[slot]._item._itemid || OUTPUT._slots[slot]._item._amount <= 0) return false;
		//	return true;
		//}

		////public Item OutputType()
		////{
		////    foreach (KeyValuePair<Item, int> availableOutput in _outputs)
		////    {
		////        if (availableOutput.Value > 0) return availableOutput.Key;
		////    }
		////    return null;
		////}
		//public int OutputType(OutputSocket cs = null)
		//{
		//	if (null == cs) return 0;
		//	int slot = GetOutputIndexBySocket(cs);
		//	return OUTPUT._slots[slot]._item._itemid;
		//}

		//public int GiveOutput(OutputSocket cs = null)
		//{
		//	if (null == cs) return 0;
		//	int slot = GetOutputIndexBySocket(cs);
		//	//아이템 차감
		//	OUTPUT._slots[slot]._item._amount -= 1;
		//	//UI
		//	this.SetBlock2Inven(OUTPUT._slots[slot]._panel, OUTPUT._slots[slot]._slot, OUTPUT._slots[slot]._item._itemid, OUTPUT._slots[slot]._item._amount, OUTPUT._slots[slot]._item._fillAmount, false);

		//	//아이템이 등록되면 시작 여부를 판단합니다.
		//	this.StartAssembling();	//HG_TODO: 이벤트방식으로 전환고려할 것(Update()에서 처리중)

		//	return OUTPUT._slots[slot]._item._itemid;
		//}
		#endregion //..GIVE_OUTPUT

		#region TAKE_INPUT
		public void TakeInput(int itemid)
		{
			if (0 == itemid) return;
			if (false == TakeInputItem(itemid)) return;

			////아이템이 등록되면 시작 여부를 판단합니다.
			//this.StartAssembling();	//HG_TODO: 이벤트방식으로 전환고려할 것(Update()에서 처리중)
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
			//if (this._recipe.Stackable <= INPUT[itemid]._amount) return false;  //최대량 초과

			if (MyCraft.Common.INPUT_ALLOW_CNT < INPUT[itemid]._amount) return false;      //INPUT은 3개 까지만
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

			//writer.Write((null!=this._recipe) ? this._recipe.id : 0);
		}
		public override void Load(BinaryReader reader)
		{
			base.Load(reader);

			//AssignRecipe(MyCraft.Managers.Game.ItemBases.FetchItemByID(reader.ReadInt32()));
		}
		#endregion //..SAVE

	}
}