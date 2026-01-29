using System;
using UnityEngine;

public class AlienMoveToCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private float separationRadius = 0.8f;
    [SerializeField] private float separationStrength = 2.0f;
    [SerializeField] private LayerMask alienMask;
    [SerializeField] private bool faceMovement = true;
    private Transform target;

    private void Start()
    {
        // Main Camera should be your AR Camera in AR Foundation scene.
        if (Camera.main != null)
        {
            target = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (!target) return;
        
        Vector3 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;
        if (dist <= stopDistance) return;
        
        Vector3 step = (toTarget / dist) * (moveSpeed * Time.deltaTime);
        transform.position += step;

        if (faceMovement && toTarget.magnitude > 0.001f)
        {
            //Look at the camera, but keep upright to avoid weird tilting.
            Vector3 lookDir = new Vector3(toTarget.x, 0f, toTarget.z);
            if (lookDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            }
        }
        
        Vector3 push = Vector3.zero;
        Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius, alienMask);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform == transform)continue;

            Vector3 away = transform.position - hits[i].transform.position;
            float d = away.magnitude;
            if (d > 0.001f)
            {
                push += away / d;
            }
        }

        if (push.sqrMagnitude > 0.0001f)
        {
            transform.position += push.normalized * (separationStrength * Time.deltaTime);
        }
    }
}
