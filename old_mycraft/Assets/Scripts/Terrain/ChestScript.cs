using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        protected override void Init()
        {
            if (0 != base._panels.Count)
                return;

            base._panels.Add(new BlockSlotPanel(this._panels.Count, ((ChestItemBase)this._itembase).Slots));//input
        }

        void Start()
        {
            this.Init();
            //_blocktype = BLOCKTYPE.CHEST;
            //if (null == base._itembase)
            //    base._itembase = GameManager.GetItemBase().FetchItemByID(this._itembase.id);

            //base._bStart = true;

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

        public override void OnClicked()
        {
            GameManager.GetChestInven().LinkInven(this, base._panels, base._progresses);
            //active
            GameManager.GetInventory().gameObject.SetActive(true);
            GameManager.GetChestInven().gameObject.SetActive(true);
            GameManager.GetSkillInven().gameObject.SetActive(true);
            //de-active
            GameManager.GetMachineInven().gameObject.SetActive(false);
            GameManager.GetStoneFurnaceInven().gameObject.SetActive(false);
        }

    }//..class ChestScript

}