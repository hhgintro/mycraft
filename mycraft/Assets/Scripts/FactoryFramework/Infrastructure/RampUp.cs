using UnityEngine;

namespace FactoryFramework
{
	public class RampUp : Building
	{
		public FootingSocket[] Sockets;

		//��ġ������ collider�� disable ���ѵд�.(ī�޶� �Դٰ��� ����)
		public override void SetEnable_2(bool enable)
		{
			//this.enabled = enable;
			this.transform.GetComponent<MeshCollider>().enabled = enable;

			foreach (var socket in Sockets) socket.GetComponent<BoxCollider>().enabled = enable;
			//foreach (var socket in Sockets)
			//	foreach( var col in socket.GetComponents<BoxCollider>())
			//		col.enabled = enable;
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