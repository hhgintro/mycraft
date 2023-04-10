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

            base.LoadPrefab("blocks/inserter", this.transform.GetChild(0));
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