﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class ChestManager : BlockManager
    {
        //private List<ChestScript> prefabs_chest;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            //this.prefabs_chest = new List<ChestScript>();
            //this.prefabs.Add(this.transform.Find("prefab/chest").GetComponent<ChestScript>());
            BlockScript script = this.transform.Find("prefab/chest").GetComponent<ChestScript>();
            script.manager = this;
            this.prefabs.Add(script);
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}

        //public override BlockScript GetChoicePrefab(TURN_WEIGHT weight)
        //{
        //    if (this.prefabs_chest.Count <= 0)
        //        return null;

        //    BlockScript prefab = this.prefabs_chest[0];
        //    if (null == prefab) return null;
        //    prefab.GetComponent<Collider>().enabled = false;
        //    return prefab;
        //}


    }
}