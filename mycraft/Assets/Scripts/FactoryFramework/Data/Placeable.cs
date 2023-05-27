using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    [CreateAssetMenu(menuName ="Factory Framework/Placeable")]
    public class Placeable : ScriptableObject, IComparable<Placeable>
    {
        // could be changed for addressables ref, or resources file name
        public GameObject prefab;
        public Sprite icon;
        /// <summary>
        /// lower value == displayed earlier in list when Sorted
        /// </summary>
        public int displayPriority;

        public int CompareTo(Placeable other)
        {
            if (other == null) return 1;
            return this.displayPriority.CompareTo(other.displayPriority);
        }

        // cost? An exercise for the reader
        //public Item costItem;
        //public int costamount;
    }
}