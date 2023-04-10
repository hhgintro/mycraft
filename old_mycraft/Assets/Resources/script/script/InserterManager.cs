using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class InserterManager : BlockManager
    {
        //private TerrainManager terrain_manager;

        //private List<InserterScript> prefabs_insert;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            //this.terrain_manager = this.transform.GetComponentInParent<TerrainManager>();


            //this.prefabs_insert = new List<InserterScript>();
            //this.prefabs.Add(this.transform.Find("prefab/robotic-arm").GetComponent<InserterScript>());
            BlockScript script = this.transform.Find("prefab/robotic-arm").GetComponent<InserterScript>();
            script.manager = this;
            this.prefabs.Add(script);
        }

        //   // Update is called once per frame
        //   void Update () {

        //}

        //public override BlockScript GetChoicePrefab(TURN_WEIGHT weight)
        //{
        //    if (this.prefabs_insert.Count <= 0)
        //        return null;

        //    BlockScript prefab = this.prefabs_insert[0];
        //    if (null == prefab) return null;
        //    prefab.GetComponent<Collider>().enabled = false;
        //    return prefab;
        //}


        //public override void CreateBlock(BlockScript script)
        //{
        //    if (null == script || BLOCKTYPE.INSERTER != script._blocktype)
        //        return;

        //    base.CreateBlock(script);
        //}
    }
}