using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class FootingSocket : MonoBehaviour//:Socket
	{
		public LogisticComponent _logisticComponent;
		[SerializeField] protected GameObject _visualIndicator;
        //public InputSocket inputConnection;

        private void Start()
        {
            _visualIndicator.SetActive(false);
        }

		// Update is called once per frame
		void Update()
        {
			//HandleVisualIndicator();

		}

        public bool IsOpen() { return true; }

		void HandleVisualIndicator()
		{
			// show the visual indicator only if we are an open socket
			_visualIndicator?.SetActive(IsOpen());
		}

        //public override void Connect(Socket sock)
        //{
        //	//if (!(sock is InputSocket))
        //	//{
        //	//	throw new System.Exception("You need to connect an InputSocket to an OutputSocket");
        //	//}
        //	//if (!IsOpen())
        //	//{
        //	//	Debug.LogWarning("You are overriding the connected input socket");
        //	//	inputConnection.Disconnect();
        //	//}
        //	//inputConnection = sock as InputSocket;
        //	//inputConnection.outputConnection = this;
        //}
        //public override void Disconnect() => inputConnection = null;
        ////public override void Disconnect() { if (null == inputConnection) return; inputConnection.Disconnect(); inputConnection = null; }
        //public override bool IsOpen() { return inputConnection == null; }

	}
}