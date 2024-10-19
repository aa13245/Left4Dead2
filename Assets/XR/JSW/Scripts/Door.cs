using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractObj
{
    bool isOpend;
    float speed = 90;
    public bool Interact()
    {
        return Open();
    }
    float degree;
    bool Open()
    {
        if (isOpend)
        {   // 닫기
            degree = 0;
            isOpend = false;
        }
        else
        {   // 열기
            degree = 90;
            isOpend = true;
        }
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpend)
        {
            if (Mathf.DeltaAngle(0, transform.eulerAngles.y) < 90)
            {
                transform.Rotate(new Vector3(0, Time.deltaTime * speed, 0));
            }
        }
        else
        {
            if (Mathf.DeltaAngle(0, transform.eulerAngles.y) > 0)
            {
                transform.Rotate(new Vector3(0, -Time.deltaTime * speed, 0));
            }
        }
    }

}
