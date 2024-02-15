using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadFaceCameraOffset : MonoBehaviour
{
    public float depthOffset = 5;

    // Update is called once per frame
    void Update()
    {
        Camera main = Camera.main;
        Vector3 dir = (main.transform.position - transform.position).normalized;

        transform.localPosition = Vector3.zero;
        transform.position += dir * depthOffset;
        //transform.right = main.transform.right;
        //transform.forward = -dir;
        //transform.up = main.transform.up;
        transform.LookAt(main.transform.position, main.transform.up);
        transform.forward *= -1;
    }
}
