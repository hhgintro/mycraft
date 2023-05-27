using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    // Item def
    /// <summary>
    /// an Item is a ScritpableObject to represent something passed around on 
    /// conveyor belts and through buildings.
    /// </summary>
    [CreateAssetMenu(menuName = "Factory Framework/Data/Item")]
    [System.Serializable]
    public class Item : SerializeableScriptableObject, IEqualityComparer<Item>
    {
        // Item Members here
        /// <summary>Icon used for diplay in GUI or inspectors.</summary>
        /// <value>icon value</value>
        public Sprite icon;
        /// <summary>Prefab of the model to show in place of the item</summary>
        /// <value>prefab value test</value>
        public GameObject prefab;
        /// <summary>Struct of item data that stores meta-information.</summary>
        public ItemData itemData;
        /// <value>Debug draw color when representing items in the world.</value>
        public Color DebugColor;

        public bool Equals(Item x, Item y)
        {
            return x.resourcesPath.Equals(y.resourcesPath);
        }

        public static bool Equals(Object x, Object y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Item obj)
        {
            return resourcesPath.GetHashCode();
        }

        public static bool operator ==(Item x, Item y)
        {
            if (x is null) return y is null;
            return x.Equals(y);
        }
        public static bool operator !=(Item x, Item y) => !(x == y);

        public override int GetHashCode()
        {
            return this.resourcesPath.GetHashCode();
        }

    }

    /// <summary>
    /// ItemData contains metadata about the Item class.
    /// The item description and the max amount you can stack in an inventory system.
    /// This needs to be a struct if we want to be compatible with Jobs System.
    /// </summary>
    [System.Serializable]
    public struct ItemData
    {
        /// <summary>Description of the item to show players.</summary>
        public string description;
        /// <summary>Max amount of items you can represent in a single inventory slot.</summary>
        public int maxStack;
    }

    /// <summary>
    /// A struct defining a stack of Items that can be used in inventory systems.
    /// </summary>
    [System.Serializable]
    public struct ItemStack
    {
        
        [SerializeField]
        private Item _item;
        /// <summary>
        /// Dual-Serializable reference to Item ScriptableObjects that reference the GUID
        /// in the asset database. Separate references for the Item Object and the GUID
        /// allow for more robust save/load.
        /// </summary>
        public Item item
        {
            get
            {
                return _item;
            }
            set 
            {
                _item = value;
                itemGUID = value?.Guid ?? null;
            } 
        }
        /// <summary>Asset Database GUID representing the Item ScriptableObject. </summary>
        public string itemGUID;
        /// <summary>
        /// Current quantity of items in the stack
        /// </summary>
        public int amount;
        /// <summary>Boolean check if the current amount is greater or equal to the max amount</summary>
        /// <returns>Boolean is there room to add items to the stack</returns>
        public bool IsFull { get { return item != null && amount >= item.itemData.maxStack; } }
    }

    [System.Serializable]
    public class SerializedItemStack
    {
        //@2023.06.26
        //public string itemResourcePath;
        //public int amount;
        public int panel;
        public int slot;
        public int itemid;
        public int amount;
    }

    /// <summary>
    /// Represents a single item on a conveyor belt with a world-space position and a 
    /// belt-length-relative position
    /// </summary>
    [System.Serializable]
    public struct ItemOnBelt
    {
        /// <summary>
        /// Reference to the Item ScriptableObject.
        /// </summary>
        //HG[2023.06.09] Item -> MyCraft.ItemBase
        //public Item item;
        public MyCraft.ItemBase _itembase;

        /// <summary>
        /// Position as a function of overall belt length.
        /// </summary>
        public float _position;
        /// <summary>
        /// Worldspace representation of the belt position to display the model.
        /// </summary>
        public Transform _model;
        /// <summary>
        /// Calculate the space that should be taken up by this item using the 
        /// global BELT_SPACING setting.
        /// </summary>
        public float EndPos { get { return _position - ConveyorLogisticsUtils.settings.BELT_SPACING; } }
    }


}