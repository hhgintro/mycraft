using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class AvatarCtrl : MonoBehaviour
    {
        //플레이어 이동속도
        public float moveSpeed = 10f;
        //플레이어 회전속도
        public float rotationSpeed = 7f;
        ////건들지 마시오
        //float animSpeed = 1.5f;

        //Animator animator;

        void Start()
        {
            //this.animator = GetComponent<Animator>();
        }
        // Update is called once per frame
        void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            //move
            this.transform.Translate(new Vector3(h, 0, v) * this.moveSpeed * Time.smoothDeltaTime);
            //rotate
            if (Input.GetMouseButton(1))
                this.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotationSpeed);

            //float h = Input.GetAxis("Horizontal") * this.rotationSpeed * Time.smoothDeltaTime;
            //float v = Input.GetAxis("Vertical") * this.moveSpeed * Time.smoothDeltaTime;

            ////rotate
            //this.transform.Rotate(0, h, 0);
            ////move
            //this.transform.Translate(0, 0, v);

            ////애니메이터 파라미터
            //this.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        }
    }
}