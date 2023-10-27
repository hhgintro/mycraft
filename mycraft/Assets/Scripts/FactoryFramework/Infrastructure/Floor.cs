using UnityEngine;

namespace FactoryFramework
{
	public class Floor : Building
	{
		public FootingSocket[] Sockets;

		//��ġ������ collider�� disable ���ѵд�.(ī�޶� �Դٰ��� ����)
		public override void SetEnable_2(bool enable)
		{
			//this.enabled = enable;
			this.GetComponent<BoxCollider>().enabled = enable;

			foreach (var socket in Sockets)
				socket.GetComponent<BoxCollider>().enabled = enable;
		}

		//socket�� ���� ��ġ����
		public override bool LocationCorrectForSocket(RaycastHit hit, ref Vector3 groundPos, ref Vector3 groundDir)
		{
			if (hit.collider.gameObject.TryGetComponent(out FootingSocket target))
			{
				if (this.gameObject == target._logisticComponent.gameObject)
					return false; //�ڽ��� socket�̸� ����
				if (target.IsOpen())
				{
					//groundPos = target._logisticComponent.transform.position
					//	+ target.transform.forward * this.transform.localScale.z;
					groundPos = target.transform.position
						+ (this.transform.position - this.transform.GetChild(0).position);
					groundDir = target.transform.forward;
					return true;
				}
			}
			return false;
		}

	}
}