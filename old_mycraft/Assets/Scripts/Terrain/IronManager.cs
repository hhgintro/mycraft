using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class IronManager : BlockManager
    {

        //private List<DrillScript> prefabs_drill;

        // Use this for initialization
        void Awake()
        {
            base.LoadPrefab("mineral/iron-ore", 40, this.transform.GetChild(0));
        }
    }


}