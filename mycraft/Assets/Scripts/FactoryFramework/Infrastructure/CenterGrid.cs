using UnityEngine;

public class CenterGrid : MonoBehaviour
{
    public void Play(Vector3 target, Vector3 forward, float length)
	{
		this.transform.forward = forward;
		this.transform.position = target
			+ Vector3.up * 0.1f				//floor�� ������ ���� ���� �ø�
			+ (forward * length * 0.5f);	//�� �ǹ����� �߾�.
		this.transform.gameObject.SetActive(true);
	}

	public void Stop()
	{
		this.transform.gameObject.SetActive(false);
	}
}
