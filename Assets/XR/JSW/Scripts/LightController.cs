﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour, IInteractObj
{
    LevelDesign levelDesign;
    // 무대 조명 On
    public bool Interact()
    {
        if (levelDesign.LightOn()) return true;
        else return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        levelDesign = GameObject.Find("LevelDesign").GetComponent<LevelDesign>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
