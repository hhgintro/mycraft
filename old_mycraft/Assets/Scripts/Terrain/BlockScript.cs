﻿using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace MyCraft
{
    public class BlockScript : MonoBehaviour
    {
        //block을 save할때 동일여부를 판단하기 위해 식별자로 활용합니다.
        private static int _static_idx = 0;
        public int _index { get; private set; }
        //..


        public List<BlockSlotPanel> _panels = new List<BlockSlotPanel>();
        public List<Progress> _progresses = new List<Progress>();

        public ItemBase _itembase { get; set; }

        public ItemInvenBase inven { get; private set; }
        public BlockManager manager { get; set; }

        //생성이 완료되지 않은 상태에서, 상화작용이 발생하면서 exception발생
        // _bOnTerrain이 true인 경우에 _bStart를 true로 설정합니다.
        public bool _bOnTerrain { get; set; }
        protected bool _bStart { get; set; }


        void Start()
        {
            this.inven = null;
        }
        protected virtual void Init() { }
        public virtual void Reset() { }
        public virtual void SetOutput(ItemBase itembase) { }

        public bool IsBelt()
        {
            if (null == this._itembase) return false;
            if (BLOCKTYPE.BELT == this._itembase.type)
                return true;
            return false;
        }
        public bool IsSpliter()
        {
            if (null == this._itembase) return false;
            if (BLOCKTYPE.SPLITER == this._itembase.type)
                return true;
            return false;
        }
        public bool IsTransport()
        {
            if (null == this._itembase) return false;
            switch(this._itembase.type)
            {
                case BLOCKTYPE.BELT:
                case BLOCKTYPE.SPLITER:
                    return true;
            }
            return false;
        }

        public virtual void SetInven(ItemInvenBase inven)
        {
            this.inven = inven;
        }

        public virtual void SetPos(int x, int y, int z)
        {
            this.transform.position = new Vector3(x, y, z);
        }

        public virtual void SetMeshRender()
        {
            if (true == this._bOnTerrain)   this.SetMeshRender(1.0f);
            //반투명하게...
            else                            this.SetMeshRender(0.5f);
        }

        //반투명처리: alpha가 0이면 투명처리됨.
        public virtual void SetMeshRender(float alpha)
        {
            MeshRenderer mesh = this.transform.GetComponentInChildren<MeshRenderer>();
            if (null == mesh) return;
            mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, alpha);
        }

        public virtual void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        public virtual BlockScript Clone()
        {
            //create object
            GameObject obj = UnityEngine.Object.Instantiate(this.gameObject);
            obj.SetActive(true);
            obj.GetComponent<Collider>().enabled = true;
            obj.layer = 9;

            //Hierarchy 위치설정
            obj.transform.SetParent(this.transform.parent.parent);
            BlockScript script = obj.GetComponent<BlockScript>();
            script.manager = this.manager;
            //script._itembase = this._itembase;
            script._index = ++_static_idx;
            if (int.MaxValue <= _static_idx) _static_idx = 0;
            return script;
        }


        ////1개만 처리
        //public virtual void Refresh(DIRECTION direct, bool active)
        //{ }
        ////주변모두 처리
        //public virtual void CreateBlock(DIRECTION direct, GameObject other)
        //{ }
        ////주변모두 처리
        ////public virtual void DeleteBlock(DIRECTION direct, GameObject other)
        ////{ }

        public virtual void DeleteBlock()
        {
            for(int p=0; p<this._panels.Count; ++p)
            {
                List<BlockSlot> slots = this._panels[p]._slots;
                if (null == slots) continue;

                for(int i=0; i<slots.Count; ++i)
                    GameManager.AddItem(slots[i]._itemid, slots[i]._amount);
            }
        }


        public virtual void OnClicked() { }
        public virtual void OnProgressCompleted(int id)
        { Debug.LogError("Error: You must re-define Function"); }

        //새로 생성된 script의 back/left/right에서 link를 걸어줍니다.
        public virtual void LinkedBelt() { }
        // [자신]을 기준으로 back / left / right 의 belt 위치에 따라 [자신의] 가중치를 결정합니다.
        public virtual int CheckWeightChainBelt() { return 0; }
        public virtual void LinkBeltSector(BELT_ROW row1, BELT_ROW row2, BlockScript next, BELT_ROW lrow, BELT_COL lcol, BELT_ROW rrow, BELT_COL rcol) { }
        //자신의 front(script)가 (외형)변경되어져야 하는지 체크합니다.
        public virtual BlockScript ChainBelt(BlockScript script) { return this; }

        public virtual BeltSector GetBeltSector(BeltScript script_front)
        { Debug.LogError("Error: You must re-define Function"); return null; }
        public virtual bool PutdownGoods(BELT_ROW row, BELT_COL col, BeltGoods goods)
        { Debug.LogError("Error: You must re-define Function"); return false; }

        protected virtual List<BlockSlot> GetPutdownSlot(int itemid)
        {
            if (this._panels.Count <= 0) return null;
            return this._panels[0]._slots;
        }
        protected virtual List<BlockSlot> GetOutputSlot()
        {
            if (this._panels.Count <= 0) return null;
            return this._panels[0]._slots;
        }

        public virtual bool PutdownGoods(BeltGoods goods)
        {
            //준비중입니다.
            if (false == this._bStart) return false;

            //HG_TODO : 꽉찬 경우 false를 리턴해야합니다.
            //..
            ItemBase itembase = GameManager.GetItemBase().FetchItemByID(goods.itemid);
            if (null == itembase) return false;

            if (true == PutdownGoods(goods.itemid, 1))
            {
                Destroy(goods.gameObject);
                return true;
            }
            return false;
        }

        public virtual bool PutdownGoods(int itemid, int amount)
        {
            //준비중입니다.
            if (false == this._bStart) return false;

            List<BlockSlot> slots = this.GetPutdownSlot(itemid);
            if (null == slots)
            {
                //Debug.Log("Can't putdown on slot: item[" + ID + "]");
                GameManager.AddItem(itemid, 1);
                return false;//ID는 slot에 넣을 수 없습니다..
            }

            return PutdownGoods(slots, itemid, amount);
        }

        public virtual bool PutdownGoods(List<BlockSlot> slots, int itemid, int amount)
        {
            ItemBase itembase = GameManager.GetItemBase().FetchItemByID(itemid);
            if (null == itembase)
            {
                Debug.LogError("error: not found database");
                return false;
            }

            //겹치기
            for (int i = 0; i < slots.Count; ++i)
            {
                if (false == slots[i].OnOverlapItem(itemid, amount, itembase.Stackable))
                    continue;

                //UI
                this.SetBlock2Inven(slots[i]._panel, i, slots[i]._itemid, slots[i]._amount);
                //Debug.Log("block slot" + i + ": " + slots[i].amount);
                return true;
            }

            //넣어준다.
            for (int i = 0; i < slots.Count; ++i)
            {
                if (false == slots[i].OnCreateItemData(itemid))
                    continue;

                //UI
                this.SetBlock2Inven(slots[i]._panel, i, slots[i]._itemid, slots[i]._amount);
                //Debug.Log("block slot" + i + ": " + slots[i].amount);
                return true;
            }
            return false;
        }
        
        //block에 id인 아이템을 넣을 수 있는지 체크
        public virtual bool CheckPutdownGoods(int panel, int slot, int itemid)
        {
            return true;//넣을 수 있다.
        }
        public virtual bool CheckPutdownGoods(int itemid)
        {
            //return false;//넣을 수 없다.
            return true;//넣을 수 있다.
        }
        //_panels[0](input-panel) 에 넣은수 있는 아이템 정보를 가져옵니다.
        //   return: true이면 무조건 가져올 수 있다.
        public virtual bool CheckPutdownGoods(ref List<int> putdowns) { return true; }

        public virtual BeltGoods PickupGoods(BlockScript script_front)
        {
            //준비중입니다.
            if (false == this._bStart) return null;

            if (null == script_front)
                return null;

            List<BlockSlot> slots = this.GetOutputSlot();
            if (null == slots)
            {
                Debug.Log("Can't pickup on slot");
                return null;//ID는 slot에서 가져올 수 없습니다..
            }

            //script_front에 넣을 수 있는 아이템
            List<int> putdowns = new List<int>();
            // return: true이면 앞쪽부터 하나씩 가져갈 수 있다.
            if(true == script_front.CheckPutdownGoods(ref putdowns))
            {
                //구지 뒤에서 뺄 필요가 있을까...테스트후 이상없으면 주석 삭제할 것
                ////뒤에서 체크하는 이유는 중간에 뺄때...crash방지차원.
                //for (int s = slots.Count - 1; 0 <= s; --s)
                for (int s = 0; s < slots.Count; ++s)
                {
                    if (0 == slots[s]._itemid) continue;

                    BeltGoods goods = GameManager.CreateMineral(slots[s]._itemid, this.transform);
                    //goods.transform.position = base.CheckDestPosFrontBlock(goods);

                    //자원하나를 빼준다.
                    if (--slots[s]._amount <= 0) slots[s]._itemid = 0;
                    //Debug.Log("block slot" + i + ": " + slots[i].amount);

                    //UI
                    this.SetBlock2Inven(slots[s]._panel, s, slots[s]._itemid, slots[s]._amount);
                    return goods;// obj.GetComponent<BeltGoods>();
                }
                return null;
            }
            // return: false이면, putdowns에 등록된 아이템만 가져갈 수 있다.
            for (int i = 0; i < putdowns.Count; ++i)
            {
                //구지 뒤에서 뺄 필요가 있을까...테스트후 이상없으면 주석 삭제할 것
                ////뒤에서 체크하는 이유는 중간에 뺄때...crash방지차원.
                //for (int s = slots.Count - 1; 0 <= s; --s)
                for (int s=0; s<slots.Count; ++s)
                {
                    if (0 == slots[s]._itemid)              continue;
                    if (putdowns[i] != slots[s]._itemid)    continue;

                    BeltGoods goods = GameManager.CreateMineral(slots[s]._itemid, this.transform);
                    //goods.transform.position = base.CheckDestPosFrontBlock(goods);

                    //자원하나를 빼준다.
                    if (--slots[s]._amount <= 0) slots[s]._itemid = 0;
                    //Debug.Log("block slot" + i + ": " + slots[i].amount);

                    //UI
                    this.SetBlock2Inven(slots[s]._panel, s, slots[s]._itemid, slots[s]._amount);
                    return goods;// obj.GetComponent<BeltGoods>();
                }
            }
            return null;
        }

        //goods를 내려놓을 위치를 가져옵니다.
        public virtual Vector3 CheckDestPosFrontBlock(BeltGoods goods)
        {
            BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.forward);
            if (null == script_front || null == script_front._itembase)
                return this.transform.position + this.transform.forward;//없으면 그냥 앞쪽으로 설정

            //if (BLOCKTYPE.BELT == script_front._itembase.type)
            if (true == script_front.IsBelt())
            {
                BeltSector sector = this.GetBeltSector((BeltScript)script_front);
                if (null == sector)
                    return this.transform.position + this.transform.forward;//없으면 그냥 앞쪽으로 설정

                //sector 설정
                goods.sector = sector;
                return sector.transform.position;   //sector의 위치
            }

            //그외는
            //return script_front.transform.position;
            return this.transform.position + this.transform.forward;//없으면 그냥 앞쪽으로 설정

        }

        //front block에 넣는다.
        protected virtual bool CheckPushGoods(BeltGoods goods)
        {
            BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.forward);
            if (null == script_front || null == script_front._itembase)
            {
                //HG_TODO : terrain에 올린다.
                //..

                return false;
            }

            switch (script_front._itembase.type)
            {
                case BLOCKTYPE.BELT:
                //case BLOCKTYPE.GROUND_BELT:
                    {
                        //goods가 먼저 생성된 후에, belt를 설치하는 경우에는 null==sector이므로 설정해 줘야합니다ㅏ.
                        if(null == goods.sector)
                            goods.sector = this.GetBeltSector((BeltScript)script_front);

                        //놓을자리가 이미 찼다면...
                        if (false == script_front.PutdownGoods(goods))
                            return false;
                    }
                    break;

                case BLOCKTYPE.CHEST:
                case BLOCKTYPE.MACHINE:
                case BLOCKTYPE.STONE_FURNACE:
                    {
                        //놓을자리가 이미 찼다면...
                        if (false == script_front.PutdownGoods(goods))
                            return false;
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        //Block에서 변경된 내용을 Inven에 반영합니다.
        public virtual void SetBlock2Inven(int panel, int slot, int itemid, int amount)
        {
            if (null == this.inven)
                return;
            this.inven.SetItem(panel, slot, itemid, amount);
        }

        public virtual void SetItem(int panel, int slot, int itemid, int amount)
        {
            if (this._panels[panel]._slots.Count <= slot)
                return;

            //if (0 != _panels[panel]._slots[slot].ID && _panels[panel]._slots[slot].ID != id)
            //{
            //    Debug.LogError("error: block different item id");
            //    return;
            //}

            if (amount <= 0)
            {
                this._panels[panel]._slots[slot]._itemid = 0;
                this._panels[panel]._slots[slot]._amount = 0;
                //Debug.Log("block slot" + slot + ": " + this._panels[panel]._slots[slot].amount);
                return;
            }

            this._panels[panel]._slots[slot]._itemid = itemid;
            this._panels[panel]._slots[slot]._amount = amount;
            //Debug.Log("block slot" + slot + ": " + this._panels[panel]._slots[slot].amount);
        }

        public virtual void Save(BinaryWriter writer)
        {
            //생성을 위해 먼저 처리되므로, 여기에서는 처리하지 않습니다.
            //writer.Write((int)this.blocktype);
            //writer.Write(this.transform.eulerAngles.y);

            byte count = 0;  //아이템개수
            for (int p = 0; p < this._panels.Count; p++)
            {
                for (int s = 0; s < this._panels[p]._slots.Count; ++s)
                {
                    if (0 != this._panels[p]._slots[s]._itemid)
                        ++count;
                }
            }

            writer.Write(count);    //아이템 개수
            for (int p=0; p<this._panels.Count; p++)
            {
                for(int s=0; s<this._panels[p]._slots.Count; ++s)
                {
                    if (0 == this._panels[p]._slots[s]._itemid)
                        continue;
                    writer.Write((byte)p);    //panel
                    writer.Write((byte)s);    //slot
                    writer.Write((short)this._panels[p]._slots[s]._itemid);
                    writer.Write((short)this._panels[p]._slots[s]._amount);
                }
            }
        }

        public virtual void Load(BinaryReader reader)
        {
            this.Init();

            //생성을 위해 먼저 처리되므로, 여기에서는 처리하지 않습니다.
            //int blocktype = reader.ReadInt32();
            //float angley = reader.ReadSingle();

            byte count = reader.ReadByte();     //아이템 개수
            for(int i=0; i<count; ++i)
            {
                byte p          = reader.ReadByte();     //panel
                byte s          = reader.ReadByte();     //slot
                short itemid    = reader.ReadInt16();     //itemid
                short amount    = reader.ReadInt16();     //amount

                this._panels[p]._slots[s]._itemid = itemid;
                this._panels[p]._slots[s]._amount = amount;
            }
        }


    }//..class BlockScript

}//..namespace MyCraft