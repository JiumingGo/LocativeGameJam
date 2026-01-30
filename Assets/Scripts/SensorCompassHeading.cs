using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;

public class SensorCompassHeading : MonoBehaviour
{
    [Header("Smoothing")]
    [SerializeField] private float smooth = 0.15f; // 0.05–0.25

    // Sensors (New Input System)
    private MagneticFieldSensor mag;
    private GravitySensor grav;
    private Accelerometer accel;

    private float smoothedHeading;
    public bool IsReady { get; private set; }

    private void OnEnable()
    {
        // Magnetometer (Android has AndroidMagneticFieldSensor, but we can use the base type)
        // Android-specific type exists: AndroidMagneticFieldSensor :contentReference[oaicite:1]{index=1}
        mag = MagneticFieldSensor.current;
        grav = GravitySensor.current;
        accel = Accelerometer.current;

        // Sensors are disabled by default: must enable them :contentReference[oaicite:2]{index=2}
        if (mag != null) InputSystem.EnableDevice(mag);
        if (grav != null) InputSystem.EnableDevice(grav);
        if (accel != null) InputSystem.EnableDevice(accel);

        IsReady = (mag != null) && (grav != null || accel != null);
    }

    private void OnDisable()
    {
        // Optional: leave enabled to avoid Android/Unity toggling issues.
        // If you do disable, do it consistently in one place.
        if (mag != null) InputSystem.DisableDevice(mag);
        if (grav != null) InputSystem.DisableDevice(grav);
        if (accel != null) InputSystem.DisableDevice(accel);
    }

    private void Update()
    {
        if (!IsReady) return;

        // Magnetic field in microtesla-ish units depending on platform; treat as a vector.
        Vector3 B = mag.magneticField.ReadValue();

        // Gravity vector (preferred). If not available, use accelerometer as approximation.
        Vector3 G = (grav != null) ? grav.gravity.ReadValue() : accel.acceleration.ReadValue();

        // Tilt-compensated heading:
        // East  = normalize(G x B)
        // North = normalize(East x G)
        Vector3 east = Vector3.Cross(G, B);
        if (east.sqrMagnitude < 1e-6f) return; // bad reading

        east.Normalize();
        Vector3 north = Vector3.Cross(east, G);
        if (north.sqrMagnitude < 1e-6f) return;

        north.Normalize();

        // Heading: 0 = North, 90 = East
        float heading = Mathf.Atan2(east.x, north.x) * Mathf.Rad2Deg;

        // Normalize to [0, 360)
        heading = (heading + 360f) % 360f;

        // Smooth for visor feel
        smoothedHeading = Mathf.LerpAngle(smoothedHeading, heading, smooth);
    }

    /// <summary>Degrees, 0 = North, 90 = East.</summary>
    public float HeadingDegrees => smoothedHeading;
}
