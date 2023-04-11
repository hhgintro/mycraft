using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class ChestScript : BlockScript
    {
        //private TerrainManager terrain_manager;

        //public ChestInven chestinven { get; set; }

        //public int slotAmount = 11;

        //HG_TODO : 인벤의 슬롯을 개체화 해서 가져다 써야 겠다.
        //public List<BlockSlot> slots = new List<BlockSlot>();

        // Use this for initialization
        void Start()
        {
            //_blocktype = BLOCKTYPE.CHEST;
            //if (null == base._itembase)
            //    base._itembase = GameManager.GetItemBase().FetchItemByID(this._itembase.id);

            base._panels.Add(new BlockSlotPanel(this._panels.Count, ((ChestItemBase)this._itembase).Slots));//input
            //base._bStart = true;

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

        //}

        public override void OnClicked()
        {
            //인벤이 활성화 되어있으면 열수 없다.
            if (true == GameManager.GetInventory().GetActive())
                return;

            GameManager.GetChestInven().LinkInven(this, base._panels, base._progresses);

            GameManager.GetInventory().SetActive(true);
            GameManager.GetChestInven().SetActive(true);
        }


    }//..class ChestScript

}