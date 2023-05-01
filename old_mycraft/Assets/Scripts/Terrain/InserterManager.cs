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
        void Awake()
        {
            base.LoadPrefab("blocks/inserter", 1030, this.transform.GetChild(0));
        }
    }
}