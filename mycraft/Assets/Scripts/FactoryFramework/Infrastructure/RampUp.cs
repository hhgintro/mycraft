using UnityEngine;

namespace FactoryFramework
{
	public class RampUp : Building
	{
		public FootingSocket[] Sockets;

		//설치전에는 collider를 disable 시켜둔다.(카메라 왔다갔다 현상)
		public override void SetEnable_2(bool enable)
		{
			//this.enabled = enable;
			this.transform.GetComponent<MeshCollider>().enabled = enable;

			foreach (var socket in Sockets) socket.GetComponent<BoxCollider>().enabled = enable;
			//foreach (var socket in Sockets)
			//	foreach( var col in socket.GetComponents<BoxCollider>())
			//		col.enabled = enable;
		}

		//socket에 의한 위치보정
		public override bool LocationCorrectForSocket(RaycastHit hit, ref Vector3 groundPos, ref Vector3 groundDir)
		{
			if (hit.collider.gameObject.TryGetComponent(out FootingSocket target))
			{
				if (this.gameObject == target._logisticComponent.gameObject)
					return false; //자신의 socket이면 무시
				if (target.IsOpen())
				{
					//groundPos = target.transform.position
					//	+ target.transform.forward * this.transform.localScale.z
					//	+ target.transform.up * this.transform.localScale.y;// * 1.2f;

					groundPos = target.transform.position
						+ (this.transform.position - this.transform.GetChild(0).position);
					groundDir = -target.transform.forward;
					return true;
				}
			}
			return false;
		}

	}
}