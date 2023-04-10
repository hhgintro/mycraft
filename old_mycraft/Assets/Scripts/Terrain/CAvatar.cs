using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class CAvatar : MonoBehaviour
    {

        public float speed = 10f;
        public float rotSpeed = 1.0f;

        public float jumpSpeed = 8.0F;
        public float gravity = 20.0F;
        //private Vector3 moveDirection = Vector3.zero;

        
        //Rigidbody body;

        //Vector3 movemoent;

        void Awake()
        {
            //body = GetComponent<Rigidbody>();
        }
        // Use this for initialization
        //void Start () {

        //}

        // Update is called once per frame
        //void Update () {

        //}

        void Update()
        {
            //case 1.
            ////float h = Input.GetAxisRaw("Horizontal");
            ////float v = Input.GetAxisRaw("Vertical");

            ////movemoent.Set(h, 0, v);
            ////movemoent = movemoent.normalized * speed * Time.smoothDeltaTime;
            ////body.MovePosition(transform.position + movemoent);

            //Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0,
            //Input.GetAxis("Vertical"));

            //moveDirection = transform.TransformDirection(moveDirection);
            ////moveDirection *= Time.smoothDeltaTime;
            //moveDirection *= speed;
            //if (Input.GetButton("Jump"))
            //    moveDirection.y = jumpSpeed;

            //moveDirection.y -= gravity * Time.smoothDeltaTime;
            //moveDirection += transform.position * Time.smoothDeltaTime;
            //transform.position = moveDirection;

            //case 2.
            //CharacterController controller = GetComponent<CharacterController>();
            //if (controller.isGrounded)
            //{
            //    moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //    moveDirection = transform.TransformDirection(moveDirection);
            //    moveDirection *= speed;
            //    if (Input.GetButton("Jump"))
            //        moveDirection.y = jumpSpeed;

            //}
            //moveDirection.y -= gravity * Time.smoothDeltaTime;
            //controller.Move(moveDirection * Time.smoothDeltaTime);


            //case 3.
            //키보드 입력
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            //이동 거리 보정
            h *= speed * Time.smoothDeltaTime;
            v *= speed * Time.smoothDeltaTime;

            //실제 이동
            this.transform.Translate(Vector3.right * h);
            this.transform.Translate(Vector3.forward * v);

            //HG_TODO : block을 생성할때 마우스 회전은 장애라서 주석처리해 둡니다.
            //      이후에 전투모드에서 마우스 회전(FPS모드) 활성화를 고민해야 합니다.
            ////마우스 입력
            //float mouseX = Input.GetAxis("Mouse X");
            //this.transform.Rotate(Vector3.up * rotSpeed * mouseX);
        }
    }
}