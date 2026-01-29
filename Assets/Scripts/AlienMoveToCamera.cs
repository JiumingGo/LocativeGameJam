using System;
using UnityEngine;

public class AlienMoveToCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.2f;
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
        Vector3 step = toTarget.normalized * (moveSpeed * Time.deltaTime);
        
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
    }
}
