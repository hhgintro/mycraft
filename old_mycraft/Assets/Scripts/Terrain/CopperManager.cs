using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class CopperManager : BlockManager
    {

        //private List<DrillScript> prefabs_drill;

        // Use this for initialization
        void Awake()
        {
            base.LoadPrefab("mineral/copper-ore", this.transform.GetChild(0));
        }
    }


}