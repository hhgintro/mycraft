using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public interface IOutput
    {
        //HG[2023.06.09] Item -> MyCraft.ItemBase
        //Item OutputType();
        //Item GiveOutput(Item filter = null);
        //bool CanGiveOutput(Item filter = null);
        int OutputType(OutputSocket cs = null);
        int GiveOutput(OutputSocket cs = null);
        bool CanGiveOutput(OutputSocket cs = null);
        //..//HG[2023.06.09] Item -> MyCraft.ItemBase
    }
}