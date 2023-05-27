using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public interface IInput
    {
        //HG[2023.06.09] Item -> MyCraft.ItemBase
        //public void TakeInput(Item item);
        //public bool CanTakeInput(Item item);
        public void TakeInput(int itemid);
        public bool CanTakeInput(int itemid);
        //..//HG[2023.06.09] Item -> MyCraft.ItemBase
    }
}