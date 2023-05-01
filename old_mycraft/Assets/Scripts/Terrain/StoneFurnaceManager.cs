using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class StoneFurnaceManager : BlockManager
    {
        //private List<MachineScript> prefabs_machine;

        // Use this for initialization
        void Awake()
        {
            base.LoadPrefab("blocks/stone-furnace", 1050, this.transform.GetChild(0));
        }

    }
}