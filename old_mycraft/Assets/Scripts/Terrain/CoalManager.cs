using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class CoalManager : BlockManager
    {

        //private List<DrillScript> prefabs_drill;

        // Use this for initialization
        void Awake()
        {
            base.LoadPrefab("mineral/coal", 30, this.transform.GetChild(0));
        }
    }


}