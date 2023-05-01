using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class TreeManager : BlockManager
    {

        //private List<DrillScript> prefabs_drill;

        // Use this for initialization
        void Awake()
        {
            base.LoadPrefab("mineral/tree", 10, this.transform.GetChild(0));
        }
    }


}