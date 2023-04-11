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
            base.LoadPrefab("blocks/drill", this.transform.GetChild(0));
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}

        //public override BlockScript GetChoicePrefab(TURN_WEIGHT weight)
        //{
        //    if (this.prefabs_drill.Count <= 0)
        //        return null;

        //    BlockScript prefab = this.prefabs_drill[0];
        //    if (null == prefab) return null;
        //    prefab.GetComponent<Collider>().enabled = false;
        //    return prefab;
        //}

        //public override void CreateBlock(BlockScript script)
        //{
        //    if (null == script || BLOCKTYPE.DRILL != script._blocktype)
        //        return;

        //    base.CreateBlock(script);
        //}
    }


}