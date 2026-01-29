using System;
using UnityEngine;

public class AlienMoveToCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float damageRange = 2.1f;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float damageCooldown = 0.6f;
    [SerializeField] private float separationRadius = 0.8f;
    [SerializeField] private float separationStrength = 2.0f;
    [SerializeField] private LayerMask alienMask;
    [SerializeField] private bool faceMovement = true;
    
    private Transform target;
    private float damageTimer;
    private PlayerHealth playerHealth;

    private void Start()
    {
        // Main Camera should be your AR Camera in AR Foundation scene.
        if (Camera.main != null)
        {
            target = Camera.main.transform;
        }
        if (target != null)
            playerHealth = target.GetComponentInParent<PlayerHealth>() ?? target.GetComponent<PlayerHealth>();

    }

    private void Update()
    {
        damageTimer -= Time.deltaTime;

        if (!target) return;
        
        Vector3 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;
        // If close enough, hurt player (with cooldown)
        if (dist <= damageRange)
        {
            if (damageTimer <= 0f && playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                damageTimer = damageCooldown;
            }

            // Optional: stop pushing into the camera
            return;
        }

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
