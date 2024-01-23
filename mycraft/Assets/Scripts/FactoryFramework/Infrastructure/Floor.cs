using UnityEngine;

namespace FactoryFramework
{
	public class Floor : Building
	{
		public FootingSocket[] Sockets;

		//설치전에는 collider를 disable 시켜둔다.(카메라 왔다갔다 현상)
		public override void SetEnable_2(bool enable)
		{
			//this.enabled = enable;
			this.GetComponent<BoxCollider>().enabled = enable;

			foreach (var socket in Sockets)
				socket.GetComponent<BoxCollider>().enabled = enable;
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
					//높이를 맞워주고
					Vector3 tmpTarget = target.transform.position;
					tmpTarget.y = target._logisticComponent.transform.position.y;
					groundPos = tmpTarget + (tmpTarget - target._logisticComponent.transform.position);
					groundDir = target._logisticComponent.transform.forward;	//인접한 개체와 방향을 같이한다.
					return true;
				}
			}
			return false;
		}

	}
}