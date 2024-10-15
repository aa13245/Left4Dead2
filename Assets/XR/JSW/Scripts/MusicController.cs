using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour, IInteractObj_JSW
{
    LevelDesign levelDesign;
    public bool Interact()
    {
        if (levelDesign.MusicOn()) return true;
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
