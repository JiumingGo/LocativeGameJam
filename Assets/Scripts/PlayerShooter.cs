using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerShooter : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private Rigidbody projectilePrefab;

    [Header("Tuning")]
    [SerializeField] private float projectileSpeed = 18f;
    [SerializeField] private float fireCooldown = 0.2f;

    private float cooldownTimer;

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer > 0f) return;

        if (PressedThisFrame())
        {
            Fire();
            cooldownTimer = fireCooldown;
        }
    }

    bool PressedThisFrame()
    {
        // Touch (Android)
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        // Mouse (Editor)
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

    void Fire()
    {
        if (muzzle == null || projectilePrefab == null) return;

        Vector3 dir = transform.forward;

        Rigidbody proj = Instantiate(
            projectilePrefab,
            muzzle.position,
            Quaternion.LookRotation(dir)
        );

        proj.linearVelocity = dir * projectileSpeed;
        // If your Unity version errors here, use:
        // proj.velocity = dir * projectileSpeed;

        Destroy(proj.gameObject, 4f);
    }
}