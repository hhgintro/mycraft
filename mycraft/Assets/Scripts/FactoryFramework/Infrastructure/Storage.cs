using MyCraft;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

namespace FactoryFramework
{
    public class Storage : Building, IInput, IOutput
    {
        void Start()
        {
            Init();
        }
        public override  void Init()
        {
            if (0 == base._panels.Count)
                base._panels.Add(new MyCraft.BuildingPanel(base._panels.Count, ((ChestItemBase)base._itembase).Slots));//input
        }

        public override void OnClicked()
        {
            Managers.Game.ChestInvens.LinkInven(this, base._panels, /*this._progresses*/null, true);
            //active
            Managers.Game.Inventories.gameObject.SetActive(true);
            Managers.Game.ChestInvens.gameObject.SetActive(true);
			//de-active
			Managers.Game.FactoryInvens.gameObject.SetActive(false);
			Managers.Game.ForgeInvens.gameObject.SetActive(false);
			Managers.Game.SkillInvens.gameObject.SetActive(false);
		}


		#region GIVE_OUTPUT
		//public bool CanGiveOutput(Item filter = null)
		//{
		//    foreach (ItemStack stack in storage)
		//    {
		//        if ((filter != null && stack.item == filter || stack.item != null) && stack.amount > 0) return true;
		//    }
		//    return false;
		//}
		public bool CanGiveOutput()
        {
            if (0 == base._panels.Count) return false;

            for (int p = 0; p < base._panels.Count; ++p)
            {
                for (int s = 0; s < base._panels[p]._slots.Count; ++s)
                    if (0 < base._panels[p]._slots[s]._amount) return true;
            }
            return false;
        }
        //public Item OutputType()
        //{
        //    foreach (ItemStack stack in storage)
        //    {
        //        if (stack.item == null && stack.amount > 0) return stack.item;
        //    }
        //    return null;
        //}
        public int OutputType()
        {
            if (0 == base._panels.Count) return 0;

            for (int p = 0; p < base._panels.Count; ++p)
            {
                for (int s = 0; s < base._panels[p]._slots.Count; ++s)
                    if (0 < base._panels[p]._slots[s]._amount) return base._panels[p]._slots[s]._itemid;
            }
            return 0;
        }
        //HG[2023.06.09] Item -> MyCraft.ItemBase
        //public Item GiveOutput(Item filter = null)
        //{
        //    for (int s = 0; s < storage.Length; s++)
        //    {
        //        ItemStack stack = storage[s];
        //        if (stack.item == null) continue;
        //        if ((filter != null && stack.item == filter || stack.item != null) && stack.amount > 0)
        //        {
        //            stack.amount -= 1;
        //            Item item = stack.item;
        //            storage[s] = stack;
        //            return item;
        //        }
        //    }
        //    return null;
        //}
        public int GiveOutput()
        {
            if (0 == base._panels.Count) return 0;

            int itemid = 0;
            for (int p = 0; p < base._panels.Count; ++p)
            {
                for (int s = 0; s < base._panels[p]._slots.Count; ++s)
                {
                    if (0 < base._panels[p]._slots[s]._amount)
                    {
                        itemid = base._panels[p]._slots[s]._itemid;
                        if (--base._panels[p]._slots[s]._amount <= 0) base._panels[p]._slots[s]._itemid = 0;
                        base.SetBlock2Inven(0, s, base._panels[p]._slots[s]._itemid, base._panels[p]._slots[s]._amount, true);
                        return itemid;
                    }
                }
            }
            return 0;
        }
        #endregion //..GIVE_OUTPUT

		#region TAKE_INPUT

		//HG[2023.06.09] Item -> MyCraft.ItemBase
		//public bool CanTakeInput(Item item)
		//{
		//    if (item == null) return false;
		//    foreach (ItemStack stack in storage)
		//    {
		//        if (stack.item == item && stack.amount < item.itemData.maxStack) return true;
		//        if (stack.item == null || stack.amount == 0) return true;
		//    }
		//    return false;
		//}
		public bool CanTakeInput(int itemid)
		{
			if (0 == itemid) return false;
			if (0 == base._panels.Count) return false;

			MyCraft.ItemBase itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(itemid);
			for (int p = 0; p < base._panels.Count; ++p)
			{
				for (int s = 0; s < base._panels[p]._slots.Count; ++s)
				{
					if (0 == base._panels[p]._slots[s]._itemid) return true;    //빈공간

					if (itemid != base._panels[p]._slots[s]._itemid) continue;       //다른아이템
					if (base._panels[p]._slots[s]._amount < itembase.Stackable) return true;    //여유가 있군.
				}
			}
			return false;   //공간부족
		}
		//public void TakeInput(Item item)
		//{
		//    for (int s = 0; s < storage.Length; s++)
		//    {
		//        ItemStack stack = storage[s];
		//        if (stack.item == item && stack.amount < item.itemData.maxStack)
		//        {
		//            stack.amount += 1;
		//            storage[s] = stack;
		//            return;
		//        }
		//        if (stack.item == null || stack.amount == 0)
		//        {
		//            stack.item = item;
		//            stack.amount = 1;
		//            storage[s] = stack;
		//            return;
		//        }
		//    }
		//}
		public void TakeInput(int itemid)
		{
			if (0 == base._panels.Count) return;

			MyCraft.ItemBase itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(itemid);
			for (int p = 0; p < base._panels.Count; ++p)
			{
				for (int s = 0; s < base._panels[p]._slots.Count; ++s)
				{
					if (0 == base._panels[p]._slots[s]._itemid)     //빈공간
					{
						base._panels[p]._slots[s]._itemid = itemid;
						base._panels[p]._slots[s]._amount = 1;
						base.SetBlock2Inven(p, s, base._panels[p]._slots[s]._itemid, base._panels[p]._slots[s]._amount, true);
						return;
					}

					if (itemid != base._panels[p]._slots[s]._itemid) continue;       //다른아이템
					if (base._panels[p]._slots[s]._amount < itembase.Stackable) //여유가 있군.
					{
						base._panels[p]._slots[s]._amount += 1;
						base.SetBlock2Inven(p, s, base._panels[p]._slots[s]._itemid, base._panels[p]._slots[s]._amount, true);
						return;
					}
				}
			}
		}
        #endregion //..TAKE_INPUT

        //#region SAVE
        //public override void Save(BinaryWriter writer)
        //{
        //    base.Save(writer);

        //}
        //public override void Load(BinaryReader reader)
        //{
        //    base.Load(reader);

        //}
        //#endregion //..SAVE


    }
}