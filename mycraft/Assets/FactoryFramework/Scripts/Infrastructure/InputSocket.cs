using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class InputSocket : Socket
    {
        public OutputSocket outputConnection;

        public override void Connect(Socket sock)
        {
            if (!(sock is OutputSocket))
            {
                throw new System.Exception("You need to connect an OutputSocket to an InputSocket");
            }
            if (!IsOpen())
            {
                Debug.LogWarning("You are overriding the connected output socket");
                outputConnection.Disconnect();
            }
            outputConnection = sock as OutputSocket;
            outputConnection.inputConnection = this;
        }
        public override void Disconnect() => outputConnection = null;
        public override bool IsOpen() { return outputConnection == null; }

        void HandleVisualIndicator()
        {
            // show the visual indicator only if we are an open socket
            _visualIndicator?.SetActive(IsOpen());
        }

        public void HandleItemTransfer()
        {
            if (outputConnection == null) return;

            // take the outputConnection item and give it to the logistic component
            IInput input = _logisticComponent as IInput;
            IOutput output = outputConnection._logisticComponent as IOutput;

            // special handling
            if (input is Splitter || input is Merger || output is Splitter || output is Merger)
                return;

            if (input.CanTakeInput(output.OutputType()) && output.CanGiveOutput())
            {
                // do the transfer
                input.TakeInput(output.GiveOutput());
            }

        }

        private void Update()
        {
            HandleVisualIndicator();
            HandleItemTransfer();
        }

        private void OnValidate()
        {
            if (_logisticComponent != null)
            {
                if (!(_logisticComponent is IInput))
                    throw new System.Exception("Input Sockets should always connect to IInput Components");
            }
        }
    }
}