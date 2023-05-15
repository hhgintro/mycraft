using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class Merger : Building, IOutput, IInput
    {
        [SerializeField] private int inputIndex = 0; // modulo this number by inputSockets.Length

        IInput GetOutputConnection(int outIdx = 0)
        {
            IInput iin = outputSockets[outIdx].inputConnection?._logisticComponent as IInput;
            return iin ?? null;
        }

        IOutput GetInputConnection(int inIdx)
        {
            IOutput iout = inputSockets[inIdx].outputConnection?._logisticComponent as IOutput;
            return iout ?? null;
        }

        /// <summary>
        /// Check if input and output streams are ready to go
        /// </summary>
        /// <param name="filter">Not Implemented</param>
        /// <returns>If any input stream can produce output right now</returns>
        public bool CanGiveOutput(Item filter = null)
        {
            // check if the connected output is ready to accept more input
            if (!CanTakeInput(filter)) return false;
            // Not Implemented
            if (filter != null) Debug.LogWarning("Merger does not Implement Item Filter Output");
            // return true if any input can give
            for (int i =0; i < inputSockets.Length; i++)
            {
                IOutput iout = GetInputConnection(i);
                if (iout == null) continue;
                if (iout.CanGiveOutput())
                    return true;
            }
            return false;
        }
        // output type doesn't really matter
        public Item OutputType() {
            IOutput iout = GetInputConnection(inputIndex);
            return iout?.OutputType() ?? null;
        }

        /// <summary>
        /// Take the output from the next available input and give to the output stream.
        /// </summary>
        /// <param name="filter">Not Implemented</param>
        /// <returns></returns>
        public Item GiveOutput(Item filter = null)
        {
            throw new System.NotImplementedException("Splitters and Mergers have Special Handling");
        }

        /// <summary>
        /// Find which next input stream ready to give output
        /// </summary>
        void GoToNextAvilable()
        {
            for (int a = 0; a < inputSockets.Length; a++)
            {
                // loop through until we find the inputSockets ready for output
                inputIndex = (inputIndex + 1) % inputSockets.Length;

                IOutput iout = GetInputConnection(inputIndex);
                if (iout == null) continue;
                // if this iout is ready to give output then we are done
                if (iout.CanGiveOutput())
                    return;
            }
        }

        

        public override void ProcessLoop()
        {
            base.ProcessLoop();

            // only continue if we're ready to take input from one of the outputs and the output is ready to recieve
            if (!CanTakeInput(null) || !CanGiveOutput(null)) return;

            GoToNextAvilable();

            IOutput iout = GetInputConnection(inputIndex);
            IInput iin = GetOutputConnection(0);

            iin.TakeInput(iout.GiveOutput());
        }

        public void TakeInput(Item item)
        {
            throw new System.NotImplementedException("Splitters and Mergers have Special Handling");
        }

        public bool CanTakeInput(Item item)
        {
            IInput iin = GetOutputConnection(0);
            if (iin == null) return false;
            return iin.CanTakeInput(item);
        }

        private void OnDrawGizmos()
        {
            // doesnt matter item type
            if (CanGiveOutput())
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