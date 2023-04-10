using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class Basic_Move : MonoBehaviour
    {

        //public int speed = 10;

        //public float senstive = 5f;//마우스 감도
        //public float speedH = 2.0f;
        //public float speedV = 2.0f;

        //private float yaw = 0.0f;
        //private float pitch = 0.0f;

        //// Use this for initialization
        //void Start()
        //{
        //    pitch = this.transform.localEulerAngles.x;
        //    yaw = this.transform.localEulerAngles.y;
        //    //Debug.Log(pitch.ToString() + " / " + yaw.ToString());

        //    StartCoroutine(CheckKey());
        //    StartCoroutine(CheckMouse());
        //}

        //// Update is called once per frame
        ////void Update() {

        ////}


        //IEnumerator CheckMouse()
        //{
        //    //마우스 이동에 따른 카메라의 회전로직
        //    while (true)
        //    {
        //        CheckMouse_Func();
        //        yield return 0;
        //    }
        //}

        //void CheckMouse_Func()
        //{
        //    //0:left, 1:right, 2:middle
        //    if (Input.GetMouseButton(1))
        //    {
        //        yaw += senstive * Input.GetAxis("Mouse X");
        //        pitch += senstive * Input.GetAxis("Mouse Y");

        //        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        //        //Debug.Log(pitch.ToString() + " / " + yaw.ToString());
        //    }

        //    //mouse wheel
        //    //transform.Translate(0, 0, senstive * Input.GetAxis("Mouse ScrollWheel"));
        //}

        //IEnumerator CheckKey()
        //{
        //    while (true)
        //    {
        //        CheckKey_Func();
        //        yield return 0;
        //    }

        //}

        //void CheckKey_Func()
        //{
        //    if (Input.GetKey(KeyCode.W))
        //    {
        //        //transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime);//상대좌표계
        //        Vector3 forward = new Vector3(transform.forward.x, 0f, transform.forward.z);
        //        forward.Normalize();
        //        transform.position += forward * speed * Time.smoothDeltaTime;
        //    }
        //    if (Input.GetKey(KeyCode.S))
        //    {
        //        //transform.Translate(Vector3.back * speed * Time.smoothDeltaTime);//상대좌표계
        //        Vector3 forward = new Vector3(transform.forward.x, 0f, transform.forward.z);
        //        forward.Normalize();
        //        transform.position -= forward * speed * Time.smoothDeltaTime;
        //    }
        //    if (Input.GetKey(KeyCode.A))
        //    {
        //        //transform.Translate(Vector3.left * speed * Time.smoothDeltaTime);//상대좌표계
        //        Vector3 right = new Vector3(transform.right.x, 0f, transform.right.z);
        //        right.Normalize();
        //        transform.position -= right * speed * Time.smoothDeltaTime;
        //    }
        //    if (Input.GetKey(KeyCode.D))
        //    {
        //        //transform.Translate(Vector3.right * speed * Time.smoothDeltaTime);//상대좌표계
        //        Vector3 right = new Vector3(transform.right.x, 0f, transform.right.z);
        //        right.Normalize();
        //        transform.position += right * speed * Time.smoothDeltaTime;
        //    }
        //    if (Input.GetKey(KeyCode.Q))
        //    {
        //        //transform.Translate(Vector3.down * speed * Time.smoothDeltaTime);//상대좌표계
        //        Vector3 up = new Vector3(0f, transform.up.y, 0f);
        //        up.Normalize();
        //        transform.position -= up * speed * Time.smoothDeltaTime;
        //    }
        //    if (Input.GetKey(KeyCode.E))
        //    {
        //        //transform.Translate(Vector3.up * speed * Time.smoothDeltaTime);//상대좌표계
        //        Vector3 up = new Vector3(0f, transform.up.y, 0f);
        //        up.Normalize();
        //        transform.position += up * speed * Time.smoothDeltaTime;
        //    }
        //}
    }//..class
}//..namespace MyCraft