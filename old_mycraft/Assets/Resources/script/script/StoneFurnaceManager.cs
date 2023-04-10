using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class StoneFurnaceManager : BlockManager
    {
        //private List<MachineScript> prefabs_machine;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            //this.prefabs_machine = new List<MachineScript>();
            //this.prefabs.Add(this.transform.Find("prefab/stone-furnace").GetComponent<StoneFurnaceScript>());
            BlockScript script = this.transform.Find("prefab/stone-furnace").GetComponent<StoneFurnaceScript>();
            script.manager = this;
            this.prefabs.Add(script);
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