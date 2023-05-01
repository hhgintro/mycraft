using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class DrillManager : BlockManager
    {

        //private List<DrillScript> prefabs_drill;

        // Use this for initialization
        void Awake()
        {
            base.LoadPrefab("blocks/drill", 1040, this.transform.GetChild(0));
        }

    }


}