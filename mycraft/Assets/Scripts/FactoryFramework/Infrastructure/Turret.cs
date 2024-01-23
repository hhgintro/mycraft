//using MyCraft;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace FactoryFramework
{
    public class Turret : Building, IInput//, IOutput
    {
        public override  void fnStart()
        {
            if (0 == base._panels.Count)
			{
				int slots = 3;
				if (null != base._itembase) slots = ((MyCraft.TurretItemBase)base._itembase).Slots;
				base._panels.Add(new MyCraft.BuildingPanel(base._panels.Count, slots));//input
			}

			base.fnStart();
        }

		//public override void ProcessLoop()
		//{
		//	if (false == StartAssembling(Time.deltaTime))
		//	{
		//		base._IsWorking = false;
		//	}
		//}

		public override void OnClicked()
        {
			MyCraft.Managers.Game.TurretInvens.LinkInven(this, null, base._panels, /*this._progresses*/null, true);
			//active
			MyCraft.Managers.Game.Inventories.gameObject.SetActive(true);
			MyCraft.Managers.Game.SkillInvens.gameObject.SetActive(true);
			MyCraft.Managers.Game.TurretInvens.gameObject.SetActive(true);
			//de-active
			MyCraft.Managers.Game.ChestInvens.gameObject.SetActive(false);
			MyCraft.Managers.Game.FactoryInvens.gameObject.SetActive(false);
			MyCraft.Managers.Game.ForgeInvens.gameObject.SetActive(false);
			MyCraft.Managers.Game.LabInvens.gameObject.SetActive(false);
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
		public bool CanGiveOutput(OutputSocket cs = null)
        {
            if (0 == base._panels.Count) return false;

            for (int p = 0; p < base._panels.Count; ++p)
            {
                for (int s = 0; s < base._panels[p]._slots.Count; ++s)
                    if (0 < base._panels[p]._slots[s]._item._amount) return true;
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
        public int OutputType(OutputSocket cs = null)
        {
            if (0 == base._panels.Count) return 0;

            for (int p = 0; p < base._panels.Count; ++p)
            {
                for (int s = 0; s < base._panels[p]._slots.Count; ++s)
                    if (0 < base._panels[p]._slots[s]._item._amount) return base._panels[p]._slots[s]._item._itemid;
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
        public int GiveOutput(OutputSocket cs = null)
        {
            if (0 == base._panels.Count) return 0;

            int itemid = 0;
            for (int p = 0; p < base._panels.Count; ++p)
            {
                for (int s = 0; s < base._panels[p]._slots.Count; ++s)
                {
                    if (0 < base._panels[p]._slots[s]._item._amount)
                    {
                        itemid = base._panels[p]._slots[s]._item._itemid;
                        if (--base._panels[p]._slots[s]._item._amount <= 0) base._panels[p]._slots[s]._item._itemid = 0;
                        base.SetBlock2Inven(0, s, base._panels[p]._slots[s]._item._itemid, base._panels[p]._slots[s]._item._amount, base._panels[p]._slots[s]._item._fillAmount, true);
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
					if (0 == base._panels[p]._slots[s]._item._itemid) return true;    //빈공간

					if (itemid != base._panels[p]._slots[s]._item._itemid) continue;       //다른아이템
					if (base._panels[p]._slots[s]._item._amount < itembase.Stackable) return true;    //여유가 있군.
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

			//덮어쓰기 먼저하고, 빈공간에 넣어준다.
			//여유공간
			MyCraft.ItemBase itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(itemid);
			for (int p = 0; p < base._panels.Count; ++p)
			{
				for (int s = 0; s < base._panels[p]._slots.Count; ++s)
				{
					if (itemid != base._panels[p]._slots[s]._item._itemid)
						continue;       //(비었거나)다른아이템

					if (base._panels[p]._slots[s]._item._amount < itembase.Stackable) //여유가 있군.
					{
						base._panels[p]._slots[s]._item._amount += 1;
						base.SetBlock2Inven(p, s, base._panels[p]._slots[s]._item._itemid, base._panels[p]._slots[s]._item._amount, base._panels[p]._slots[s]._item._fillAmount, true);
						return;
					}
				}
			}
			//빈공간
			for (int p = 0; p < base._panels.Count; ++p)
			{
				for (int s = 0; s < base._panels[p]._slots.Count; ++s)
				{
					if (0 != base._panels[p]._slots[s]._item._itemid)
						continue;       //다른아이템

					//빈공간
					base._panels[p]._slots[s]._item._itemid = itemid;
					base._panels[p]._slots[s]._item._amount = 1;
					base.SetBlock2Inven(p, s, base._panels[p]._slots[s]._item._itemid, base._panels[p]._slots[s]._item._amount, base._panels[p]._slots[s]._item._fillAmount, true);
					return;
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