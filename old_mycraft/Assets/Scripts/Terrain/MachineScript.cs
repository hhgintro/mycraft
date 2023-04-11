using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyCraft
{
    public class MachineScript : BlockScript
    {
        //true이면 생산 진행중입니다.
        //public bool _build { get; set; }

      
        //true이면 진행중, false이면 잠시멈출
        public bool _bRunning { get; set; }
        //progress에 의해 만들어 지는 아이템 ID
        //private AssemblingOutput _output = null;
        private SkillBase _output = null;

        //public MachineScript()
        private void Init()
        {
            //_itembase가 null이 아니면
            //이미 초기화 되었으니. 또 실행할 필요없다.
            //if (null != _itembase)
            //    return;

            //_blocktype = BLOCKTYPE.MACHINE;
            //if (null == base._itembase)
            //    base._itembase = GameManager.GetItemBase().FetchItemByID(this._itembase.id);

            //for(int i=0; i<base._itembase._assembling.inputs; ++i)
            //    base._panels.Add(new BlockSlotPanel(this._panels.Count, 1));//input
            base._panels.Add(new BlockSlotPanel(this._panels.Count, 0));// base._itembase._assembling.inputs));//input
            base._panels.Add(new BlockSlotPanel(this._panels.Count, 1));//output
            base._panels.Add(new BlockSlotPanel(this._panels.Count, ((MachineItemBase)base._itembase)._assembling.chips));//chip


            base._progresses.Add(new Progress(this, 0, 1f, false));//progress
            //this._progresses.Add(new Progress(this, 1, 10, true));//progress-fuel

        }

        // Use this for initialization
        void Start()
        {
            //_blocktype = BLOCKTYPE.MACHINE;
            //if (null == base._itembase)
            //    base._itembase = GameManager.GetItemBase().FetchItemByID(this._itembase.id);

            ////for(int i=0; i<base._itembase._assembling.inputs; ++i)
            ////    base._panels.Add(new BlockSlotPanel(this._panels.Count, 1));//input
            //base._panels.Add(new BlockSlotPanel(this._panels.Count, 0));// base._itembase._assembling.inputs));//input
            //base._panels.Add(new BlockSlotPanel(this._panels.Count, 1));//output
            //base._panels.Add(new BlockSlotPanel(this._panels.Count, ((MachineItemBase)base._itembase)._assembling.chips));//chip


            //base._progresses.Add(new Progress(this, 0, 1f, false));//progress
            ////this._progresses.Add(new Progress(this, 1, 10, true));//progress-fuel
            this.Init();

            //progress
            //this._build = false;
            this._bRunning = false;

            //HG_TEST :임으로 output을 설정해서 테스트합니다.
            //this._output = base._itembase._assembling.outputs[0];
            //StartCoroutine(CheckAssembling());

            if (true == base._bOnTerrain)
            {
                base.SetMeshRender(1.0f);
                base._bStart = true;
            }
            else
            {
                //반투명하게...
                base.SetMeshRender(0.5f);
            }

        }

        //// Update is called once per frame
        //void Update()
        //{
        //    //HG_TODO : 아이템이 추가되었을 때 coroutine으로 처리하도록 합니다.

        //    //for(int i=0; i< this._progressList.Count; ++i)
        //    //    this._progressList[i].Update();

        //}

        public override void Reset()
        {
            //Debug.Log("machine reset");
            this._output = null;
            ///this._outputskill = null;

            //아이템을 인벤으로 다시 넣어줍니다.
            for(int p=0; p<base._panels.Count; ++p)
            {
                List<BlockSlot> slots = base._panels[p]._slots;
                for (int i = 0; i < slots.Count; ++i)
                {
                    if (0 == slots[i]._itemid) continue;

                    GameManager.GetInventory().AddItem(slots[i]._itemid, slots[i]._amount);
                    slots[i]._itemid = 0;
                }
            }
        }

        //public override void SetOutput(int itemid)
        //{
        //    for (int i = 0; i < ((MachineItemBase)base._itembase)._assembling.outputs.Count; ++i)
        //    {
        //        if (((MachineItemBase)base._itembase)._assembling.outputs[i].itemid == itemid)
        //        {
        //            this._output = ((MachineItemBase)base._itembase)._assembling.outputs[i];
        //            //input slot
        //            this._panels[0].SetAmount(this._output.inputs.Count);
        //            //Debug.Log("set output: " + itemid);
        //            return;
        //        }
        //    }
        //    Debug.Log("not found output: " + itemid);
        //}
        public override void SetOutput(SkillBase skillbase)
        {
            if (null == skillbase)
                return;

            //load할때 아이템 설정이 안되어 있어서.
            this.Init();


            ////불러오기할때는 Start()를 먼저 거치지 못하기 때문에 null값이 들어옵니다.
            //if(null == base._itembase)
            //  base._itembase = GameManager.GetItemBase().FetchItemByID(this._itembase.id);

            //(items.json)machine에 outputskill항목이 너무 커져 버리므로 삭제합니다.
            ////skill id가 존재하는지 체크한다.
            //Dictionary<int, SkillBase> skills = ((MachineItemBase)base._itembase)._assembling.outputs;
            //if (false == skills.ContainsKey(skillbase.id))
            //{
            //    Debug.Log("not found machine output: " + skillbase.id);
            //    return;
            //}

            this._output = skillbase;
            this._panels[0].SetAmount(skillbase.cost.items.Count);
        }


        IEnumerator CheckAssembling()
        {
            while (true)
            {
                bool end = CheckAssembling_Func();
                if (true == end)
                    break;
                yield return 0;
            }

            //yield return CheckAssembling_Func();
        }

        bool CheckAssembling_Func()
        {
            if (false == this._bRunning)
                return true;

            for (int i = 0; i < base._progresses.Count; ++i)
                base._progresses[i].Update();
            return false;
        }

        bool CheckStartAssembling()
        {
            //이미 작동중...
            if (true == this._bRunning)
                return false;

            //output을 설정후에 진행가능합니다.
            if (null == this._output)
                return false;

            //check...자원
            List<BlockSlot> slots = this._panels[0]._slots;
            List<SkillCostItem> inputs = this._output.cost.items;
            for(int i=0; i< inputs.Count; ++i)
            {
                if (slots[i].GetItemAmount() < inputs[i].amount)
                    return false;
            }


            //output이 가득 찼으면...중단.
            if (true == this._panels[1].GetIsFull(this._output.outputs[0].itemid))
                return false;


            //자원소모
            for (int i = 0; i < inputs.Count; ++i)
            {
                //아이템 차감
                slots[i]._amount -= inputs[i].amount;
                if (slots[i]._amount <= 0) slots[i]._itemid = 0;
                //UI
                this.SetBlock2Inven(slots[i]._panel, slots[i]._slot, slots[i]._itemid, slots[i]._amount);
            }

            //output에 따라 bunning-time을 설정합니다.
            this._progresses[0].SetTime(this._output.cost.time);


            this._bRunning = true;
            //Debug.Log(no + ": machine running(" + this._bRunning + ")");
            StartCoroutine(CheckAssembling());
            return true;
        }

        ////연료 progress가 진행중이거나, 연료가 있는지 체크합니다.
        //BlockSlot CheckProgressFuel()
        //{
        //    //check fuel
        //    BlockSlot slotFuel;
        //    List<BlockSlot> slots = base._panels[1]._slots;
        //    for (int i = 0; i < slots.Count; ++i)
        //    {
        //        if (0 != slots[i].ID)
        //        {
        //            slotFuel = slots[i];
        //            check = true;//연료가 있다.
        //        }
        //    }

        //    return null;
        //}

        public override void DeleteBlock()
        {
            for (int p = 0; p < base._panels.Count; ++p)
            {
                List<BlockSlot> slots = base._panels[p]._slots;
                for (int i = 0; i < slots.Count; ++i)
                {
                    if (0 == slots[i]._itemid) continue;
                    GameManager.AddItem(slots[i]._itemid, slots[i]._amount);
                }
            }
            //UI
            if (null != base.inven) base.inven.Clear();
        }

        public override void SetInven(ItemInvenBase inven)
        {
            base.SetInven(inven);

            //for(int i=0; i< this._progressList.Count; ++i)
            //    this._progressList[i].SetInven(inven);

            for(int i=0; i<base._progresses.Count; ++i)
                    this._progresses[i].SetInven(inven);
        }

        public override void OnClicked()
        {
            //인벤이 활성화 되어있으면 열수 없다.
            if (true == GameManager.GetInventory().GetActive())
                return;

            if (null == this._output)
            {
                //setactive(true)이후에 LinkIven()을 호출합니다.(setactive에서 block정보를 초기화)
                GameManager.GetSkillInven().LinkInven(this, null ,null);
                GameManager.GetSkillInven().SetActive(true);
                return;
            }

            GameManager.GetMachineInven().LinkInven(this, base._panels, base._progresses);

            GameManager.GetInventory().SetActive(true);
            GameManager.GetMachineInven().SetActive(true);
        }

        //id: progress id
        public override void OnProgressCompleted(int id)
        {
            //progress가 completed상태일때
            //자원/연료가 없다면
            //주변 progress에 잠시멈춤을 요청합니다.
            //(자신은 bunning=false, running=false)
            //(other는 bunning=무관, running=false)

            if(0 == id)
            {
                //if (null == this._output)
                //    return;//reset되어 만들 output이 없는 경우.

                ////아이템을 생성해 준다.
                //if(false == base.PutdownGoods(this.GetOutputSlot(), this._output.itemid))
                //{
                //    this._bRunning = false;//들어갈 자리가 없으면 실패...
                //    //Debug.Log(no + ": machine running(" + this._bRunning + ")");
                //    return;
                //}
                if (null == this._output)
                    return;//reset되어 만들 output이 없는 경우.

                //아이템을 생성해 준다.
                for(int i=0; i<this._output.outputs.Count; ++i)
                {
                    if (false == base.PutdownGoods(this.GetOutputSlot(), this._output.outputs[i].itemid))
                    {
                        this._bRunning = false;//들어갈 자리가 없으면 실패...
                                               //Debug.Log(no + ": machine running(" + this._bRunning + ")");
                        return;
                    }
                }
            }

            //check...자원
            List<BlockSlot> slots = this._panels[0]._slots;
            List<SkillCostItem> inputs = this._output.cost.items;
            for (int i = 0; i < inputs.Count; ++i)
            {
                if (slots[i].GetItemAmount() < inputs[i].amount)
                {
                    this._bRunning = false;//자원/연료가 없으면...중지
                    //Debug.Log(no + ": machine running(" + this._bRunning + ")");
                    return;
                }
            }


            //자원소모
            for (int i = 0; i < inputs.Count; ++i)
            {
                //아이템 차감
                slots[i]._amount -= inputs[i].amount;
                if (slots[i]._amount <= 0) slots[i]._itemid = 0;
                //UI
                this.SetBlock2Inven(slots[i]._panel, slots[i]._slot, slots[i]._itemid, slots[i]._amount);
            }
        }

        public override bool PutdownGoods(BeltGoods goods)
        {
            //Debug.Log("PutdownGoods : goods");
            if(true == PutdownGoods(goods.itemid))
            {
                Destroy(goods.gameObject);
                return true;
            }
            return false;
        }

        public override bool PutdownGoods(int itemid)
        {
            if (false == this._bStart)
                return false;

            ////Debug.Log("PutdownGoods : ID");
            //bool ret = base.PutdownGoods(itemid);
            //if(false == ret)
            //    return false;
            ItemBase itembase = GameManager.GetItemBase().FetchItemByID(_itembase.id);
            if (null == itembase) return false;


            if (null == this._output) return false;
            List<SkillCostItem> inputs = this._output.cost.items;
            for (int i = 0; i < inputs.Count; ++i)
            {
                if (inputs[i].itemid != itemid)
                    continue;

                List<BlockSlot> slots = base.GetPutdownSlot(itemid);

                //겹치기
                if (true == slots[i].OnOverlapItem(itemid, itembase.Stackable))
                {
                    //UI
                    this.SetBlock2Inven(slots[i]._panel, i, slots[i]._itemid, slots[i]._amount);
                    break;
                }

                //넣어준다.
                if (true == slots[i].OnCreateItemData(itemid))
                {
                    //UI
                    this.SetBlock2Inven(slots[i]._panel, i, slots[i]._itemid, slots[i]._amount);
                    break;
                }
            }


            //아이템이 등록되면 시작 여부를 판단합니다.
            this.CheckStartAssembling();
            return true;
        }

        public override BeltGoods PickupGoods(BlockScript script_front)
        {
            BeltGoods goods = base.PickupGoods(script_front);
            if (null == goods) return null;

            //아이템이 등록되면 시작 여부를 판단합니다.
            this.CheckStartAssembling();
            return goods;
        }

        public override void SetItem(int panel, int slot, int itemid, int amount)
        {
            //Debug.Log("PutdownGoods : setitem");
            base.SetItem(panel, slot, itemid, amount);

            ////아이템이 등록되면 시작 여부를 판단합니다.
            ////if(0 < amount)
            //switch(panel)
            //{
            //    case 0://자원이 있어야 진행가능합니다.
            //        for(int i=0; i< this._output.inputs.Count; ++i)
            //        {
            //            if (false == this._panels[panel].GetFillSlot(slot, this._output.inputs[i].amount))

            //        }
            //        if (true == this._panels[panel].GetFillSlot(slot, this._output.inputs[slot].amount))
            //            this.CheckStartAssembling();
            //        break;

            //    case 1://가득찬 경우에는 더이상 진행할 수 없습니다.
            //        if(false == this._panels[panel].GetIsFull(itemid))
            //            this.CheckStartAssembling();
            //        break;
            //}
            this.CheckStartAssembling();
        }


        ////goods를 넣을 slot index를 가져옵니다.
        //protected override List<BlockSlot> GetPutdownSlot(int itemid)
        //{
        //    //생성이 완료되지 않았다면...
        //    if (base._panels.Count <= 0) return null;

        //    //ItemBase itembase = GameManager.GetItemBase().FetchItemByID(id);
        //    if (null == base._item || null == base._itembase._furnace) return null;

        //    //input
        //    List<FurnaceInputItem> inputs = base._itembase._furnace.input.items;
        //    for (int i = 0; i < inputs.Count; ++i)
        //    {
        //        if (inputs[i].itemid == itemid)
        //            return base._panels[0]._slots;
        //    }

        //    //fuel
        //    List<FurnaceFuelItem> fuels = base._itembase._furnace.fuel.items;
        //    for (int i = 0; i < fuels.Count; ++i)
        //    {
        //        if (fuels[i].itemid == itemid)
        //            return base._panels[1]._slots;
        //    }
        //    return null;
        //}

        //goods을 가져올 slot index를 가져옵니다.
        protected override List<BlockSlot> GetOutputSlot()
        {
            if (this._panels.Count <= 0) return null;
            return base._panels[1]._slots;
        }

        //block에 id인 아이템을 넣을 수 있는지 체크
        public override bool CheckPutdownGoods(int itemid)
        {
            if (false == this._bStart)
                return false;

            //for(int p=0; p<base._panels.Count; ++p)
            //{
            //    for (int i = 0; i < base._itembase._assembling.inputs; ++i)
            //    {
            //        if (true == CheckPutdownGoods(p, i, itemid))
            //            return true;
            //    }
            //}
            int panel = 0;//input
            for (int i = 0; i < base._panels[panel]._slots.Count; ++i)
            {
                if (true == CheckPutdownGoods(panel, i, itemid))
                    return true;
            }
            ////input
            //if (true == CheckPutdownGoods(0, itemid)) return true;
            ////output
            //if (true == CheckPutdownGoods(1, itemid)) return true;
            ////chip
            //if (true == CheckPutdownGoods(1, itemid)) return true;

            return false;//넣을 수 없다.
        }

        public override bool CheckPutdownGoods(int panel, int slot, int itemid)
        {
            if (null == this._output || null == this._output.cost.items) return false;

            switch (panel)
            {
                case 0://input
                    {
                        List<SkillCostItem> inputs = this._output.cost.items;
                        if (inputs[slot].itemid != itemid)
                            return false;
                        //자동겹침 제한
                        if (this._panels[panel]._slots[slot]._amount < 5)
                            return true;
                    }
                    break;

                case 1://output
                    {
                        //넣을 수 없습니다.
                    }
                    break;

                case 2://chip
                    {
                        //HG_TODO : 아직 chip이 없어서. 추후에 넣도록 합니다.

                        //List<FurnaceFuelItem> fuels = base._itembase._furnace.fuel.items;
                        //for (int i = 0; i < fuels.Count; ++i)
                        //{
                        //    if (fuels[i].itemid == itemid)
                        //        return true;
                        //}
                    }
                    break;
            }
            return false;
        }


        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);

            //skill
            int skillid = 0;
            if (null != this._output) skillid = this._output.id;
            writer.Write(skillid);
            //...

        }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);

            //skill
            int skillid = reader.ReadInt32();
            if (0 != skillid)
                this.SetOutput(GameManager.GetSkillBase().FetchItemByID(skillid));
            //....

        }

    }//..class ChestScript

}