using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class FollowCam : MonoBehaviour
    {
        public Transform target;    //추적할 타겟 오브젝트
        public float dist = 10.0f;  //카메라와의 일정거리
        public float height = 10.0f; //카메라의 높이 설정
        public float smoothRotate = 5.0f;   //부드러운 회전을 위한 변수
        public float dampTrace = 20f;


        //********************************************************************************//
        // 만약 카메라의 위치를 계산하는 로직을 Update 함수에서 구현했다면
        // player의 움직임이 완료되기 전에 카메라가 이동할 경우 떨림 현상이 발생할 수 있다.
        // 따라서 카메라의 위치 계산은 Player가 이동 및 회전을 모두 끝마친 이후에 실행해야 하므로
        // LateUpdate 함수에서 실행한다.
        //********************************************************************************//
        // Update is called once per frame
        void LateUpdate()
        {
            ////부드러운 회전을 위한 Mathf.LerpAngle
            //float curYAngle = Mathf.LerpAngle(this.transform.eulerAngles.y, target.eulerAngles.y, smoothRotate * Time.smoothDeltaTime);

            ////오일러 타입을 쿼터니언으로 바꾸기
            //Quaternion rot = Quaternion.Euler(0, curYAngle, 0);

            ////카메라 위치를 타겟 회전각도만큼 회전 후 dist만큼 띄우고, 높이를 올리기
            //this.transform.position = target.position - (rot * Vector3.forward * dist)
            //    + (Vector3.up * height);

            this.transform.position = Vector3.Lerp(this.transform.position
                , this.target.position - (this.target.forward * dist) + (Vector3.up * height)
                , Time.smoothDeltaTime * dampTrace);

            //타켓을 바라보게 하기
            this.transform.LookAt(target);

        }
    }
}