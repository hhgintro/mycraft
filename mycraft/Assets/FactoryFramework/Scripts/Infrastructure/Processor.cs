using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FactoryFramework
{
    //Forge
    public class Processor : Building, IInput, IOutput
    {
        public Recipe recipe;

        public int numInputs;
        public int numOutputs;

        //HG_TEST:private->public
        public Dictionary<Item, int> _inputs = new Dictionary<Item, int>();
        public Dictionary<Item, int> _outputs = new Dictionary<Item, int>();

        public Recipe[] validRecipes;
        public Recipe[] invalidRecipes;

        private IEnumerator currentRoutine;

        public override void ProcessLoop()
        {            
            if (CanStartProduction())
            {
                IsWorking = true;
                currentRoutine = MakeOutput();
                StartCoroutine(currentRoutine);
            }
            else
            {
                IsWorking = currentRoutine != null;
            }
        }

        public void ClearInternalStorage()
        {
            _inputs = new Dictionary<Item, int>();
        }
        public bool AssignRecipe(Recipe recipe, bool clearStorage =false)
        {
            this.recipe = recipe;
            if (clearStorage)
                ClearInternalStorage();
            return true;
        }

        protected bool CanStartProduction()
        {
            // need a recipe to make!
            if (recipe == null)
            {
                if (_inputs.Keys.Count == 0) return false;
                // we can try to find a recipe
                Recipe[] matchedRecipes = RecipeFinder.FilterRecipes(_inputs.Keys.ToArray(), numOutputs, validRecipes, invalidRecipes);
                if (matchedRecipes.Length <= 0) return false;
                AssignRecipe(matchedRecipes[0]);
            }
            // cannot start a new production cycle while one is running
            if (currentRoutine != null) return false;
            //check for outputs being full
            foreach (Item item in recipe.OutputItems)
            {
                _outputs.TryGetValue(item, out int amount);
                if (amount >= item.itemData.maxStack) return false;
            }
            // check that we have enough input ingredients
            for (int i = 0; i < recipe.inputs.Length; i++)
            {
                Item item = recipe.InputItems[i];
                _inputs.TryGetValue(item, out int amount);
                if (amount < recipe.inputs[i].amount) return false;
            }
            return true;
        }
        public bool CanStartProductionTest { get { return CanStartProduction(); } }
        private void ConsumeInputIngredients()
        {
            for (int i = 0; i < recipe.inputs.Length; i++)
            {
                Item item = recipe.InputItems[i];
                _inputs[item] -= recipe.inputs[i].amount;
            }
        }
        private void CreateOutputProducts()
        {
            for (int i = 0; i < recipe.outputs.Length; i++)
            {
                Item item = recipe.OutputItems[i];
                if (_outputs.ContainsKey(item))
                {
                    _outputs[item] = Mathf.Min(_outputs[item] + recipe.outputs[i].amount, item.itemData.maxStack);
                } else
                {
                    _outputs.Add(item, 1);
                }
            }
        }

        public bool CanGiveOutput(Item filter = null)
        {
            if (filter != null)
            {
                _outputs.TryGetValue(filter, out int amount);
                if (amount > 0) return true;
            } else
            {
                foreach (KeyValuePair<Item, int> availableOutput in _outputs)
                {
                    if (availableOutput.Value > 0) return true;
                }

            }
            return false;
        }
        public Item OutputType() {
            foreach (KeyValuePair<Item, int> availableOutput in _outputs)
            {
                if (availableOutput.Value > 0) return availableOutput.Key;
            }
            return null;
        }
        public Item GiveOutput(Item filter = null)
        {

            if (filter != null)
            {
                _outputs.TryGetValue(filter, out int amount);
                if (amount > 0)
                {
                    _outputs[filter] -= 1;
                    return filter;
                }
            }
            else
            {
                foreach (KeyValuePair<Item, int> availableOutput in _outputs)
                {
                    if (availableOutput.Value > 0)
                    {
                        _outputs[availableOutput.Key] -= 1;
                        return availableOutput.Key;
                    }
                }

            }
            return null;
        }

        public void TakeInput(Item item)
        {
            if (_inputs.ContainsKey(item))
                _inputs[item] += 1;
            else
                _inputs.Add(item, 1);
        }
        public bool CanTakeInput(Item item)
        {
            if (item == null) return false;

            if (_inputs.ContainsKey(item))  return _inputs[item] < item.itemData.maxStack;
            else                            return _inputs.Keys.Count < numInputs;
        }

        IEnumerator MakeOutput()
        {
            ConsumeInputIngredients();
            float _t = 0f;
            while (_t < recipe.tickCost)
            {
                //FIXME custom tick?
                _t += Time.deltaTime;
                yield return null;
            }
            CreateOutputProducts();

            // do we need to un-assign the current recipe? Check if we can make any more
            bool isEmpty = true;
            foreach(KeyValuePair<Item,int> pair in _inputs)
                isEmpty &= pair.Value == 0;

            if (isEmpty) AssignRecipe(null,true);
            currentRoutine = null;
        }

        #region SERIALIZATION_HELPERS
        private List<SerializedItemStack> SerializeField(Dictionary<Item, int> dict)
        {
            List<SerializedItemStack> items = new List<SerializedItemStack>();
            foreach(KeyValuePair<Item, int> obj in dict)
            {
                items.Add(new SerializedItemStack(){ itemResourcePath = obj.Key.resourcesPath, amount = obj.Value});
            }
            return items;
        }
        public SerializedItemStack[] SerializeInputs() => SerializeField(_inputs).ToArray();
        public SerializedItemStack[] SerializeOutputs() => SerializeField(_outputs).ToArray();

        private Dictionary<Item, int> DeserializeField(List<SerializedItemStack> items)
        {
            Dictionary<Item, int> dict = new Dictionary<Item, int>();
            foreach(var iStack in items)
            {
                dict.Add(Resources.Load<Item>(iStack.itemResourcePath), iStack.amount);
            }
            return dict;
        }
        public void DeserializeInputs(SerializedItemStack[] inputs) => _inputs = DeserializeField(inputs.ToList());
        public void DeserializeOutputs(SerializedItemStack[] inputs) => _outputs = DeserializeField(inputs.ToList());
        #endregion

    }
}