using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class JKYshoot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float _maximumForce;

    [SerializeField]
    private float _maximumForceTime;

    private float _timeMouseButtonDown;

    public Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _timeMouseButtonDown = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                JKYZombie1 zombie = hitInfo.collider.GetComponentInParent<JKYZombie1>();

                if (zombie != null)
                {
                    float mouseButtonDownDuration = Time.time - _timeMouseButtonDown;
                    float forcePercentage = mouseButtonDownDuration / _maximumForceTime;
                    float forceMagnitude = Mathf.Lerp(1, _maximumForce, forcePercentage);

                    Vector3 forceDirection = zombie.transform.position - _camera.transform.position;
                    forceDirection.y = 1;
                    forceDirection.Normalize();

                    Vector3 force = forceMagnitude * forceDirection;

                    zombie.TriggerRagdoll(force, hitInfo.point);
                }
            }
        }
    }
}
