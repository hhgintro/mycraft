using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class MachineManager : BlockManager
    {
        //private List<MachineScript> prefabs_machine;

        // Use this for initialization
        void Awake()
        {
            base.LoadPrefab("blocks/machine", this.transform.GetChild(0));
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}

        //public override BlockScript GetChoicePrefab(TURN_WEIGHT weight)
        //{
        //    if (this.prefabs_machine.Count <= 0)
        //        return null;

        //    BlockScript prefab = this.prefabs_machine[0];
        //    if (null == prefab) return null;
        //    prefab.GetComponent<Collider>().enabled = false;
        //    return prefab;
        //}


    }
}