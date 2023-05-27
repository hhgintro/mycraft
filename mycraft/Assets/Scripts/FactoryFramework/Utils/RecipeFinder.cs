using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace FactoryFramework {
    public class RecipeFinder
    {
        internal static Recipe[] _recipes;
        public static Recipe[] Recipes
        {
            get
            {
                if (_recipes == null)
                {
                    _recipes = Resources.LoadAll<Recipe>("");
                }
                return _recipes;
            }
        }
        public static Recipe[] FilterRecipes(Item[] inputs, int numOutputs=-1, Recipe[] whitelist=null, Recipe[] blacklist = null)
        {
            if (blacklist != null && whitelist != null)
            {
                Debug.LogWarning("You should not define both valid and invalid recipe lists for a building it can cause conflicts");
                if (blacklist.Intersect(whitelist).Count() > 0 )
                {
                    Debug.LogError("Conflict! valid and invalid items have overlapping subset");
                }
            }

            // find recipes that match the given inputs and outputs
            Recipe[] recipes = Recipes.Where(r => r.InputItems.Except(inputs).Count() == 0 && (numOutputs == -1 || r.OutputItems.Length == numOutputs)).ToArray();

            // filter recipes
            if (whitelist != null)
            {
                recipes = recipes.Where(r => whitelist.Contains(r)).ToArray();
            }
            if (blacklist != null)
            {
                recipes = recipes.Where(r => !blacklist.Contains(r)).ToArray();
            }

            return recipes;
        }

    } 
}
