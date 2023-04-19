using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class CrudeOilManager : BlockManager
    {

        //private List<DrillScript> prefabs_drill;

        // Use this for initialization
        void Awake()
        {
            base.LoadPrefab("mineral/crude-oil", this.transform.GetChild(0));
        }
    }


}