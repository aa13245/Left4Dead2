using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpTest_JSW : MonoBehaviour
{
    Inventory_JSW inventory;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<Inventory_JSW>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo))
            {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Item_JSW"))
                {
                    inventory.PickUp(hitInfo.transform.gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            inventory.Drop(0);
        }
    }
}
