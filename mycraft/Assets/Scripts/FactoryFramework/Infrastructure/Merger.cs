using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class Merger : Building, IOutput, IInput
    {
        [SerializeField] private int inputIndex = 0; // modulo this number by inputSockets.Length

		public override void ProcessLoop()
		{
			// *** splitter와 merge는 이전convayer와 다음conveyor를 연결해 준다. ***

			// only continue if we're ready to take input from one of the outputs and the output is ready to recieve
			if (false == CanGiveOutput()) return;

			if (false == GoToNextAvilable()) return;

			IOutput iout = GetInputConnection(inputIndex);
			IInput iin = GetOutputConnection(0);
			if (false == iin?.CanTakeInput(iout.OutputType())) return;
			iin?.TakeInput(iout.GiveOutput());
		}

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
		/// Find which next input stream ready to give output
		/// </summary>
		bool GoToNextAvilable()
        {
            for (int a = 0; a < inputSockets.Length; a++)
            {
                // loop through until we find the inputSockets ready for output
                inputIndex = (inputIndex + 1) % inputSockets.Length;
				//Debug.Log($"({inputIndex}) socket");

				IOutput iout = GetInputConnection(inputIndex);
				if (iout == null)
				{
					//Debug.Log($"({inputIndex}) socket is null");
					return false;
				}
				// if this iout is ready to give output then we are done
				if (iout.CanGiveOutput())
				{
					//Debug.Log($"({inputIndex}) socket is OK");
					return true;
				}
				//Debug.Log($"({inputIndex}) socket not Ready");
			}
			//Debug.Log($"All socket is fail");
			return false;
        }


		#region GIVE_OUTPUT
		/// <summary>
		/// Check if input and output streams are ready to go
		/// </summary>
		/// <param name="filter">Not Implemented</param>
		/// <returns>If any input stream can produce output right now</returns>
		//HG[2023.06.09] Item -> MyCraft.ItemBase
		//public bool CanGiveOutput(Item filter = null)
		//{
		//    // check if the connected output is ready to accept more input
		//    if (!CanTakeInput(filter)) return false;
		//    // Not Implemented
		//    if (filter != null) Debug.LogWarning("Merger does not Implement Item Filter Output");
		//    // return true if any input can give
		//    for (int i =0; i < inputSockets.Length; i++)
		//    {
		//        IOutput iout = GetInputConnection(i);
		//        if (iout == null) continue;
		//        if (iout.CanGiveOutput())
		//            return true;
		//    }
		//    return false;
		//}
		public bool CanGiveOutput(OutputSocket cs = null)
		{
			// check if the connected output is ready to accept more input
			//if (!CanTakeInput(0)) return false;
			// Not Implemented
			//if (filter != null) Debug.LogWarning("Merger does not Implement Item Filter Output");
			// return true if any input can give
			for (int i = 0; i < inputSockets.Length; i++)
			{
				IOutput iout = GetInputConnection(i);
				if (iout == null) continue;
				if (iout.CanGiveOutput(cs))
					return true;
			}
			return false;
		}

		// output type doesn't really matter
		//public Item OutputType() {
		//    IOutput iout = GetInputConnection(inputIndex);
		//    return iout?.OutputType() ?? null;
		//}
		public int OutputType(OutputSocket cs = null)
		{
			IOutput iout = GetInputConnection(inputIndex);
			return iout?.OutputType() ?? 0;
		}
		//...//HG[2023.06.09] Item -> MyCraft.ItemBase

		/// <summary>
		/// Take the output from the next available input and give to the output stream.
		/// </summary>
		/// <param name="filter">Not Implemented</param>
		/// <returns></returns>
		//HG[2023.06.09] Item -> MyCraft.ItemBase
		//public Item GiveOutput(Item filter = null)
		//{
		//    throw new System.NotImplementedException("Splitters and Mergers have Special Handling");
		//}
		public int GiveOutput(OutputSocket cs = null)
		{
			throw new System.NotImplementedException("Splitters and Mergers have Special Handling");
		}
		//..//HG[2023.06.09] Item -> MyCraft.ItemBase
		#endregion //..GIVE_OUTPUT

		#region TAKE_INPUT
		//HG[2023.06.09] Item -> MyCraft.ItemBase
		//public bool CanTakeInput(Item item)
		//{
		//    IInput iin = GetOutputConnection(0);
		//    if (iin == null) return false;
		//    return iin.CanTakeInput(item);
		//}
        public bool CanTakeInput(int itemid)
        {
            if (0 == itemid) return false;

            IInput iin = GetOutputConnection(0);
            if (iin == null) return false;
            return iin.CanTakeInput(itemid);
        }

		//public void TakeInput(Item item)
		//{
		//    throw new System.NotImplementedException("Splitters and Mergers have Special Handling");
		//}
		public void TakeInput(int itemid)
        {
            throw new System.NotImplementedException("Splitters and Mergers have Special Handling");
        }
		//..//HG[2023.06.09] Item -> MyCraft.ItemBase
		#endregion //..TAKE_INPUT

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