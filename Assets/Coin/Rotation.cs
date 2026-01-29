using UnityEngine;

[DisallowMultipleComponent]
public class Rotation : MonoBehaviour
{
    [Tooltip("Degrees per second")]
    public float rotationSpeed = 180f;

    [Tooltip("Axis to rotate around (local or world space)")]
    public Vector3 rotationAxis = Vector3.up;

    [Tooltip("Rotate in local space when true, world space when false")]
    public bool useLocalSpace = true;

    void Update()
    {
        // Rotate by rotationSpeed degrees per second around the chosen axis
        Vector3 delta = rotationAxis.normalized * (rotationSpeed * Time.deltaTime);
        if (useLocalSpace)
            transform.Rotate(delta, Space.Self);
        else
            transform.Rotate(delta, Space.World);
    }
}
