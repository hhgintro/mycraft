using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class OutputSocket : Socket
    {
        public InputSocket inputConnection;

        public override void Connect(Socket sock) {
            if (!(sock is InputSocket))
            {
                throw new System.Exception("You need to connect an InputSocket to an OutputSocket");
            }
            if (!IsOpen())
            {
                Debug.LogWarning("You are overriding the connected input socket");
                inputConnection.Disconnect();
            }
            inputConnection = sock as InputSocket;
            inputConnection.outputConnection = this;      
        }
        public override void Disconnect() => inputConnection = null;
        public override bool IsOpen() { return inputConnection==null; }

        void HandleVisualIndicator()
        {
            // show the visual indicator only if we are an open socket
            _visualIndicator?.SetActive(IsOpen());
        }

        private void Update()
        {
            HandleVisualIndicator();
        }

        private void OnValidate()
        {
            if (_logisticComponent != null)
            {
                if (!(_logisticComponent is IOutput))
                    throw new System.Exception("Output Sockets should always connect to IOutput Components");
            }
        }
    }
}