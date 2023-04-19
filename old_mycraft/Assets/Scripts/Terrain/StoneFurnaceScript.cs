using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyCraft
{
    public class StoneFurnaceScript : BlockScript
    {
        //private TerrainManager terrain_manager;

        //public MachineInven inven { get; set; }

        //public int slotAmount = 11;

        //HG_TODO : 인벤의 슬롯을 개체화 해서 가져다 써야 겠다.
        //public List<List<BlockSlot>> _slots = new List<List<BlockSlot>>();
        //public List<BlockSlot> input_slots = new List<BlockSlot>();
        //public List<BlockSlot> fuel_slots = new List<BlockSlot>();
        //public List<BlockSlot> output_slots = new List<BlockSlot>();

        //true이면 생산 진행중입니다.
        //public bool _build { get; set; }

        //private List<Progress> _progressList = new List<Progress>();
       
        //true이면 진행중, false이면 잠시멈출
        public bool _bRunning { get; set; }
        //progress에 의해 만들어 지는 아이템 ID
        private int _output { get; set; }

        protected override void Init()
        {
            if (0 != base._panels.Count)
                return;
            //_blocktype = BLOCKTYPE.STONE_FURNACE;
            //if (null == base._itembase)
            //    base._itembase = GameManager.GetItemBase().FetchItemByID(this._itembase.id);

            this._panels.Add(new BlockSlotPanel(this._panels.Count, ((FurnaceItemBase)base._itembase)._furnace.inputs));//input
            this._panels.Add(new BlockSlotPanel(this._panels.Count, ((FurnaceItemBase)base._itembase)._furnace.fuels));//fuel
            this._panels.Add(new BlockSlotPanel(this._panels.Count, ((FurnaceItemBase)base._itembase)._furnace.outputs));//output


            this._progresses.Add(new Progress(this, 0, 1f, false));//progress
            this._progresses.Add(new Progress(this, 1, 10, true));//progress-fuel
        }

        void Start()
        {
            this.Init();

            //progress
            //this._build = false;
            this._bRunning = false;
            this._output = 0;
            //StartCoroutine(CheckAssembling());

            //if (true == base._bOnTerrain)
            //{
            //    //base.SetMeshRender(1.0f);
            //    base._bStart = true;
            //}
            //else
            //{
            //    //반투명하게...
            //    //base.SetMeshRender(0.5f);
            //}

        }

        //// Update is called once per frame
        //void Update()
        //{
        //    //HG_TODO : 아이템이 추가되었을 때 coroutine으로 처리하도록 합니다.

        //    //for(int i=0; i< this._progressList.Count; ++i)
        //    //    this._progressList[i].Update();

        //}

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

            for (int i = 0; i < this._progresses.Count; ++i)
                this._progresses[i].Update();

            return false;
        }

        bool CheckStartAssembling()
        {
            //이미 작동중...
            if (true == this._bRunning)
                return false;

            //check fuel
            BlockSlot slotFuel = null;
            if (false == this._progresses[1].GetIsBunning())
            {
                slotFuel = this._panels[1].GetFillSlot(1);
                if (null == slotFuel)
                    return false;//연료가 없으면...중단
            }

            //check...자원
            //BlockSlot slotMineral = null;
            //if (false == this._progresses[0].GetIsBunning())
            //{
            //    slotMineral = this._panels[0].GetFillSlot(1);
            //    if (null == slotMineral)
            //        return false;//자원이 없으면...중단
            //}
            //else
            //{
            //    Debug.LogError("mieral is null");
            //    slotMineral = this._panels[0].GetFillSlot(1);
            //    if (null == slotMineral)
            //        return false;//자원이 없으면...중단
            //}
            //check...자원
            BlockSlot slotMineral = this._panels[0].GetFillSlot(1);
            if (null == slotMineral)
                return false;//자원이 없으면...중단


            //output에 등록된 아이템과 비교하기 위해 먼저 체크합니다.
            int output_tmp = 0;
            float build_time_tmp = 0f;
            List<FurnaceInputItem> inputs = ((FurnaceItemBase)base._itembase)._furnace.input;
            for (int i = 0; i < inputs.Count; ++i)
            {
                if (slotMineral._itemid == inputs[i].itemid)
                {
                    //소모하는 자원에 따라 output을 결정합니다.
                    output_tmp = inputs[i].output;
                    //HG_TODO : 소모하는 자원에 따라 bunning-time을 설정합니다.
                    build_time_tmp = inputs[i].build_time;
                }
            }


            //output이 가득 찼으면...중단.
            if (true == this._panels[2].GetIsFull(output_tmp))
                return false;



            //연료소모
            if (null != slotFuel)
            {
                if(false == this._progresses[1].GetIsBunning())
                {
                    //소모하는 자원에 따라 bunning-time을 설정합니다.
                    List<FurnaceFuelItem> fuels = ((FurnaceItemBase)base._itembase)._furnace.fuel;
                    for (int i = 0; i < fuels.Count; ++i)
                    {
                        if (slotFuel._itemid == fuels[i].itemid)
                            this._progresses[1].SetTime(fuels[i].burning_time);
                    }
                    //아이템 차감
                    if (--slotFuel._amount <= 0) slotFuel._itemid = 0;
                    //UI
                    this.SetBlock2Inven(slotFuel._panel, slotFuel._slot, slotFuel._itemid, slotFuel._amount);
                }
            }
            //자원소모
            if (null != slotMineral)
            {
                //소모하는 자원에 따라 output을 결정합니다.
                this._output = output_tmp;
                //소모하는 자원에 따라 bunning-time을 설정합니다.
                this._progresses[0].SetTime(build_time_tmp);
                //아이템 차감
                if (--slotMineral._amount <= 0) slotMineral._itemid = 0;
                //UI
                this.SetBlock2Inven(slotMineral._panel, slotMineral._slot, slotMineral._itemid, slotMineral._amount);
            }


            this._bRunning = true;
            //Debug.Log(no + ": furnace running(" + this._bRunning + ")");
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

            for(int i=0; i<base._progresses.Count; ++i)
                this._progresses[i].SetInven(inven);
        }

        public override void OnClicked()
        {
            GameManager.GetStoneFurnaceInven().LinkInven(this, base._panels, base._progresses);
            //active
            GameManager.GetInventory().gameObject.SetActive(true);
            GameManager.GetStoneFurnaceInven().gameObject.SetActive(true);
            //de-active
            GameManager.GetSkillInven().gameObject.SetActive(false);
            GameManager.GetChestInven().gameObject.SetActive(false);
            GameManager.GetMachineInven().gameObject.SetActive(false);
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
                //아이템을 생성해 준다.
                if(false == base.PutdownGoods(this.GetOutputSlot(), this._output, 1))
                {
                    this._bRunning = false;//들어갈 자리가 없으면 실패...
                    //Debug.Log(no + ": furnace 1 running(" + this._bRunning + ")");
                    return;
                }
            }


            //자원/연료가 없으면 null, 있으면 해당 slot을 리턴합니다.
            BlockSlot s = this._panels[id].GetFillSlot(1);
            if (null == s)
            {
                this._bRunning = false;//자원/연료가 없으면...중지
                //Debug.Log(no + ": furnace 2 running(" + this._bRunning + ")");
                return;
            }

            //1개를 소모합니다.
            if (--s._amount <= 0)   s._itemid = 0;
            //Debug.Log("block slot" + slot.amount);
            //UI
            this.SetBlock2Inven(s._panel, s._slot, s._itemid, s._amount);
        }

        public override bool PutdownGoods(BeltGoods goods)
        {
            //Debug.Log("PutdownGoods : goods");
            if(true == PutdownGoods(goods.itemid, 1))
            {
                Destroy(goods.gameObject);
                return true;
            }
            return false;
        }

        public override bool PutdownGoods(int itemid, int amount)
        {
            //Debug.Log("PutdownGoods : ID");
            bool ret = base.PutdownGoods(itemid, amount);
            if(false == ret)
                return false;

            //아이템이 등록되면 시작 여부를 판단합니다.
            this.CheckStartAssembling();
            return true;
        }

        //inserter: 물건을 가져가는 로봇팔
        //dutdonws: block_front 에 넣을 수 있는 itemid(null인 경우도 있다. 필요한 경우에 사용하세요.)
        public override BeltGoods PickupGoods(BlockScript inserter, List<int> putdowns/*null*/)
        {
            BeltGoods goods = base.PickupGoods(inserter, putdowns);

            //아이템이 등록되면 시작 여부를 판단합니다.
            this.CheckStartAssembling();
            return goods;
        }
        //public override BeltGoods PickupGoods(BlockScript script_front)
        //{
        //    BeltGoods goods = base.PickupGoods(script_front);
        //    if (null == goods) return null;

        //    //아이템이 등록되면 시작 여부를 판단합니다.
        //    this.CheckStartAssembling();
        //    return goods;
        //}

        public override void SetItem(int panel, int slot, int itemid, int amount)
        {
            //Debug.Log("PutdownGoods : setitem");
            base.SetItem(panel, slot, itemid, amount);

            //아이템이 등록되면 시작 여부를 판단합니다.
            //if(0 < amount)
            switch(panel)
            {
                case 0://자원이 있어야 진행가능합니다.
                case 1://자원이 있어야 진행가능합니다.
                    if(null != this._panels[panel].GetFillSlot(1))
                        this.CheckStartAssembling();
                    break;

                case 2://가득찬 경우에는 더이상 진행할 수 없습니다.
                    if(false == this._panels[panel].GetIsFull(itemid))
                        this.CheckStartAssembling();
                    break;
            }
            this.CheckStartAssembling();
        }


        //goods를 넣을 slot index를 가져옵니다.
        protected override List<BlockSlot> GetPutdownSlot(int itemid)
        {
            //생성이 완료되지 않았다면...
            if (base._panels.Count <= 0) return null;

            //ItemBase itembase = GameManager.GetItemBase().FetchItemByID(id);
            if (null == base._itembase || null == ((FurnaceItemBase)base._itembase)._furnace) return null;

            //input
            List<FurnaceInputItem> inputs = ((FurnaceItemBase)base._itembase)._furnace.input;
            for (int i = 0; i < inputs.Count; ++i)
            {
                if (inputs[i].itemid == itemid)
                    return base._panels[0]._slots;
            }

            //fuel
            List<FurnaceFuelItem> fuels = ((FurnaceItemBase)base._itembase)._furnace.fuel;
            for (int i = 0; i < fuels.Count; ++i)
            {
                if (fuels[i].itemid == itemid)
                    return base._panels[1]._slots;
            }
            return null;
        }

        //goods을 가져올 slot index를 가져옵니다.
        protected override List<BlockSlot> GetOutputSlot()
        {
            if (this._panels.Count <= 0) return null;
            return base._panels[2]._slots;
        }

        ////block에 id인 아이템을 넣을 수 있는지 체크
        //public override bool CheckPutdownGoods(int itemid)
        //{
        //    for (int p = 0; p < base._panels.Count; ++p)
        //    {
        //        for (int i = 0; i < base._panels[p]._slots.Count; ++i)
        //        {
        //            if (true == CheckPutdownGoods(p, i, itemid))
        //                return true;
        //        }
        //    }

        //    ////input
        //    //if (true == CheckPutdownGoods(0, itemid)) return true;
        //    ////fuel
        //    //if (true == CheckPutdownGoods(1, itemid)) return true;

        //    return false;//넣을 수 없다.
        //}

        public override bool CheckPutdownGoods(int panel, int slot, int itemid)
        {
            if (null == base._itembase || null == ((FurnaceItemBase)base._itembase)._furnace) return false;

            switch(panel)
            {
                case 0://input
                    {
                        List<FurnaceInputItem> inputs = ((FurnaceItemBase)base._itembase)._furnace.input;
                        //for (int i = 0; i < inputs.Count; ++i)
                        //{
                        //    if (inputs[i].itemid == itemid)
                        //        return true;
                        //}
                        if (inputs[slot].itemid != itemid)
                            return false;
                        //자동겹침 제한
                        if (this._panels[panel]._slots[slot]._amount < inputs[slot].limit)
                            return true;
                    }
                    break;

                case 1://fuel
                    {
                        List<FurnaceFuelItem> fuels = ((FurnaceItemBase)base._itembase)._furnace.fuel;
                        //for (int i = 0; i < fuels.Count; ++i)
                        //{
                        //    if (fuels[i].itemid == itemid)
                        //        return true;
                        //}
                        if (fuels[slot].itemid != itemid)
                            return false;
                        //자동겹침 제한
                        if (this._panels[panel]._slots[slot]._amount < fuels[slot].limit)
                            return true;
                    }
                    break;

                case 2://output
                    {
                        //넣을 수 없습니다.
                    } break;
            }
            return false;
        }

        private void GetPutdownItems<T>(List<T> inputs, List<BlockSlot> slots, int Limit, ref List<int> putdowns) where T : FurnaceInputBase   
        {
            for (int i = 0; i < inputs.Count; ++i)
            {
                for (int s = 0; s < slots.Count; ++s)
                {
                    //비어있거나 limit 미달이면 putdown가능하다.
                    if (0 == slots[s]._itemid
                        || slots[s]._amount < inputs[i].limit * Limit)
                    {
                        putdowns.Add(inputs[i].itemid);
                        break;
                    }
                }
            }
        }

        //_panels[0](input-panel) 에 넣은수 있는 아이템 정보를 가져옵니다.
        public override bool CheckPutdownGoods(ref List<int> putdowns)
        {
            if (null == base._itembase || null == ((FurnaceItemBase)base._itembase)._furnace) return false;

            List<FurnaceInputItem> inputs = ((FurnaceItemBase)base._itembase)._furnace.input;
            //0: input-panel
            GetPutdownItems<FurnaceInputItem>(inputs, this._panels[0]._slots, 1, ref putdowns);
            //1: fuel-panel
            List<FurnaceFuelItem> fuels = ((FurnaceItemBase)base._itembase)._furnace.fuel;
            GetPutdownItems<FurnaceFuelItem>(fuels, this._panels[1]._slots, 1, ref putdowns);

            //여기서 이미 아이템이 담겼다면 *2 할 필요없으니...리턴
            if (0 > putdowns.Count) return false;

            //limit * 2 까지 저장합니다.
            //0: input-panel
            GetPutdownItems<FurnaceInputItem>(inputs, this._panels[0]._slots, 2, ref putdowns);
            //1: fuel-panel
            GetPutdownItems<FurnaceFuelItem>(fuels, this._panels[0]._slots, 2, ref putdowns);
            return false;   //putdowns에 포함된 아이템만 가져올 수 있다.
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);

            ////mineral
            //int itemid = 0;
            //if (null != this.mineral) itemid = (int)this.mineral.itemid;
            //writer.Write(itemid);
            ////...

        }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);

            ////mineral
            //int itemid = reader.ReadInt32();
            //if (0 != itemid)
            //    this.mineral = GameManager.CreateMineral(itemid, this.transform);//, this.transform.position);
            ////....

        }


    }//..class ChestScript

}