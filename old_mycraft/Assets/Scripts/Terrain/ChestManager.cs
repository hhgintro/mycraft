using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class ChestManager : BlockManager
    {
        //private List<ChestScript> prefabs_chest;

        void Awake()
        {
            base.LoadPrefab("blocks/chest", this.transform.GetChild(0));
        }

    }
}