using UnityEngine;

public class CenterGrid : MonoBehaviour
{
    public void Play(Vector3 target, Vector3 forward, float length)
	{
		this.transform.forward = forward;
		this.transform.position = target
			+ Vector3.up * 0.1f				//floor에 뭍혀서 조금 위로 올림
			+ (forward * length * 0.5f);	//두 건물사이 중앙.
		this.transform.gameObject.SetActive(true);
	}

	public void Stop()
	{
		this.transform.gameObject.SetActive(false);
	}
}
