﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKYCamRotate : MonoBehaviour
{
    public float rotSpeed = 200f;
    float mx = 0;
    float my = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouse_X = Input.GetAxis("Mouse X");
        float mouse_Y = Input.GetAxis("Mouse Y");

        mx += mouse_X * rotSpeed * Time.deltaTime;
        my += mouse_Y * rotSpeed * Time.deltaTime;

        my = Mathf.Clamp(my, -70f, 70f);

        Vector3 angle = new Vector3(-my, mx, 0);
        transform.eulerAngles = angle;

     
    }
}
