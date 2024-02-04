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
					//���̸� �¿��ְ�
					Vector3 tmpTarget = target.transform.position;
					tmpTarget.y = target._logisticComponent.transform.position.y;
					groundPos = tmpTarget + (tmpTarget - target._logisticComponent.transform.position);
					groundDir = target._logisticComponent.transform.forward;	//������ ��ü�� ������ �����Ѵ�.
					return true;
				}
			}
			return false;
		}

	}
}