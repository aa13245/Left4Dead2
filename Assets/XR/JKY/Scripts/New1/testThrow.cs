using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class testThrow : MonoBehaviour
{
    public GameObject bomb;
    public float force;
    public float explosionDelay;
    public List<GameObject> vfx;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            Throw();
        }

    }
    void Throw()
    {
        GameObject bombPrefabe = Instantiate(bomb, transform.position, Quaternion.identity);
        Rigidbody rb = bombPrefabe.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force);
        StartCoroutine(GetExplosion(bombPrefabe));
    }

    IEnumerator GetExplosion(GameObject bombPrefabe)
    {
        //GameObject bombPrefabe = Instantiate(bomb, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(explosionDelay);
        JKYBomb explosion = bombPrefabe.GetComponent<JKYBomb>();
        explosion.ExplosionBomb();
        foreach ( GameObject fx in vfx)
        {
            Instantiate(fx, bombPrefabe.transform.position, Quaternion.identity);
        }
        Destroy(bombPrefabe, 3);
    }
   
    
}
