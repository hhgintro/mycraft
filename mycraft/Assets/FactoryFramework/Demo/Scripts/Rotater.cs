using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float speed;
    public Vector3 axis = Vector3.up;
    private Vector3 originalRot;

    // Start is called before the first frame update
    void Start()
    {
        originalRot = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axis, speed * Time.deltaTime);
    }
}
