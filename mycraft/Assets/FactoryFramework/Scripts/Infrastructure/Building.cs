using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FactoryFramework
{
    public class Building : LogisticComponent
    {
        public UnityEvent<Building> OnBuildingDestroyed;

        public InputSocket[] inputSockets;
        public OutputSocket[] outputSockets;

        private void Update()
        {
            IsWorking = true;
            this.ProcessLoop();
        }

        /// <summary>
        /// Get a List of all recipes from Resources
        /// </summary>
        /// <returns></returns>
        protected Recipe[] GetAllRecipes()
        {
            return Resources.LoadAll<Recipe>("");
        }

        private void OnDestroy()
        {
            OnBuildingDestroyed?.Invoke(this);
        }

        /// <summary>
        /// Return the input socket at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public InputSocket GetInputSocketByIndex(int index)
        {
            if (index >= inputSockets.Length)
                return null;

            return inputSockets[index];
        }

        /// <summary>
        /// Return the output socket at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public OutputSocket GetOutputSocketByIndex(int index)
        {
            if (index >= outputSockets.Length)
                return null;

            return outputSockets[index];
        }

        /// <summary>
        /// Get the index of a specific input socket if it exists, else return -1
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public int GetInputIndexBySocket(InputSocket cs)
        {
            for (int i = 0; i < inputSockets.Length; i++)
                if (cs.Equals(inputSockets[i]))
                    return i;

            return -1;
        }

        /// <summary>
        /// Get the index of a specific output socket if it exists, else return -1
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public int GetOutputIndexBySocket(OutputSocket cs)
        {
            for (int i = 0; i < outputSockets.Length; i++)
                if (cs.Equals(outputSockets[i]))
                    return i;

            return -1;
        }

    }
}