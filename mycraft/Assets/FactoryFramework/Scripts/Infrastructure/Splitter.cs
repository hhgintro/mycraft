using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class Splitter : Building, IInput, IOutput
    {
        [SerializeField] private int outputIndex = 0; // modulo this number by outputSockets.Length

        IInput GetOutputConnection(int outIdx)
        {
            IInput iin = outputSockets[outIdx].inputConnection?._logisticComponent as IInput;
            return iin ?? null;
        }

        IOutput GetInputConnection(int inIdx = 0)
        {
            IOutput iout = inputSockets[inIdx].outputConnection?._logisticComponent as IOutput;
            return iout ?? null;
        }

        /// <summary>
        /// Check if any of the outputs are ready to accept an input
        /// </summary>
        /// <param name="item">Item to be accepted by an output stream</param>
        /// <returns>If any output stream can accept input right now</returns>
        public bool CanTakeInput(Item item)
        {
            // if nothing is in the input connection we can't take input
            IOutput iout = GetInputConnection(0);
            if (iout == null || !iout.CanGiveOutput(item))
                return false;
            // check if any output links are accepting input
            for (int i =0; i < outputSockets.Length; i++)
            {
                IInput iin = GetOutputConnection(i);
                if (iin == null) continue;
                if (iin.CanTakeInput(item))
                    return true;
            }
            return false;
             
        }

        /// <summary>
        /// Send item to the current selected (or next available) output stream
        /// </summary>
        /// <param name="item">Item to be accepted by an output stream</param>
        public void TakeInput(Item item)
        {
            throw new System.NotImplementedException("Splitters and Mergers have Special Handling");
        }

        public override void ProcessLoop()
        {
            base.ProcessLoop();

            // only continue if we're ready to take input and give output
            if (!CanTakeInput(null) || !CanGiveOutput(null)) return;

            GoToNextAvilable(null);

            IOutput iout = GetInputConnection(0);
            IInput iin = GetOutputConnection(outputIndex);

            iin.TakeInput(iout.GiveOutput());

        }

        void GoToNextAvilable(Item item)
        {
            for (int a = 0; a < outputSockets.Length; a++)
            {
                // loop through until we find the output ready to take input
                outputIndex = (outputIndex + 1) % outputSockets.Length;

                IInput iin = GetOutputConnection(outputIndex);
                if (iin == null) continue;
                // check if the current input is ready to pass its item on to the output stream
                if (iin.CanTakeInput(item))
                {
                    return;
                }
            }

        }

        /// <summary>
        /// Type of item that will be output next to one of the streams
        /// </summary>
        /// <returns>Type of Item produced by input stream</returns>
        public Item OutputType()
        {
            IOutput iout = GetInputConnection(0);
            return iout?.OutputType() ?? null;
        }

        /// <summary>
        /// Unused?
        /// </summary>
        /// <param name="filter">Not Implemented</param>
        /// <returns>Item from the input stream</returns>
        public Item GiveOutput(Item filter = null)
        {
            //IOutput iout = GetInputConnection(0);
            //return iout?.GiveOutput(filter) ?? null;
            throw new System.NotImplementedException("Splitters and Mergers have Special Handling");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Can the input stream produce output</returns>
        public bool CanGiveOutput(Item filter = null)
        {
            // if no conveyors have room, return false
            //if (!CanTakeInput(filter)) {            
            //    return false; 
            //}
            IOutput iout = GetInputConnection(0);
            return iout?.CanGiveOutput(filter) ?? false;
        }

        private void OnDrawGizmos()
        {
            // doesnt matter item type
            if (CanTakeInput(null))
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 1f);
        }
    }
}