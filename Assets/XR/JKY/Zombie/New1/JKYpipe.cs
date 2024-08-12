using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKYpipe : MonoBehaviour
{
    public GameObject bomb;
    float currTime = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject pipe = Instantiate(bomb);
            print("bomb 생성");
            pipe.transform.position = transform.position;

        }
        if (currTime > 6f)
        {
            Destroy(gameObject);
        }
    }
}
