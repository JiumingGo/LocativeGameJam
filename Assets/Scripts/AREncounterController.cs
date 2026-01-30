using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class AREncounterController : MonoBehaviour
{
    public enum State
    {
        NotOnCampus,
        NavigateToBase,
        Encounter,
        BaseCleared
    }
    
    [Header("References")]
    [SerializeField] private LocationServiceStarter locationStarter;
    [SerializeField] private AlienSpawner alienSpawner;
    [SerializeField] private PlayerShooter playerShooter;

    [Header("UI")]
    [SerializeField] private Transform arrow3D;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text distanceText;

    [Header("Campus Gate (Lat/Lon)")]
    [SerializeField] private double campusLat = 50.7427222;   
    [SerializeField] private double campusLon = -1.8961111; 
    [SerializeField] private float campusRadiusMeters = 250f;

    [Header("Bases (Lat/Lon)")]
    [SerializeField] private Vector2[] baseLatLon; // x=lat, y=lon
    [SerializeField] private float baseTriggerRadiusMeters = 25f;

    [Header("Encounter Settings")]
    [SerializeField] private int aliensToKillToClear = 10;

    private State state;
    private int baseIndex = 0;
    private int killsThisBase = 0;

    private void Start()
    {
        SetState(State.NotOnCampus);
        if (alienSpawner) alienSpawner.enabled = false;
        if (playerShooter) playerShooter.enabled = false;
    }

    private void Update()
    {
        if (locationStarter != null && !locationStarter.Ready)
        {
            SetUI("Starting GPS...", "");
            return;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            SetUI("GPS not running. Check permissions/settings.", "");
            return;
        }

        if (baseLatLon == null || baseLatLon.Length == 0)
        {
            SetUI("No bases configured.", "");
            return;
        }

        var myLat = Input.location.lastData.latitude;
        var myLon = Input.location.lastData.longitude;

        float distToCampus = (float)HaversineMeters(myLat, myLon, campusLat, campusLon);

        // Gate: must be near campus to play
        if (distToCampus > campusRadiusMeters)
        {
            SetState(State.NotOnCampus);
            SetUI("Go to campus to start", $"Campus: {distToCampus:0}m");
            DisableEncounter();
            return;
        }

        // We’re on campus
        Vector2 targetBase = baseLatLon[Mathf.Clamp(baseIndex, 0, baseLatLon.Length - 1)];
        float distToBase = (float)HaversineMeters(myLat, myLon, targetBase.x, targetBase.y);

        if (state != State.Encounter)
        {
            // Navigation mode
            SetState(State.NavigateToBase);
            EnableNavigationUI();
            DisableEncounter();

            UpdateArrow(myLat, myLon, targetBase.x, targetBase.y);
            SetUI($"Find Base #{baseIndex + 1}", $"Base: {distToBase:0}m");

            // Trigger encounter when close enough
            if (distToBase <= baseTriggerRadiusMeters)
            {
                StartEncounter();
            }
        }
        else
        {
            // Encounter running
            SetUI($"Base #{baseIndex + 1} - Fight!", $"Kills: {killsThisBase}/{aliensToKillToClear}");

            // If base cleared
            if (killsThisBase >= aliensToKillToClear)
            {
                ClearBase();
            }
        }
    }

    private void StartEncounter()
    {
        SetState(State.Encounter);
        killsThisBase = 0;

        if (alienSpawner) alienSpawner.enabled = true;
        if (playerShooter) playerShooter.enabled = true;

        if (arrow3D) arrow3D.gameObject.SetActive(false);

    }

    private void ClearBase()
    {
        SetState(State.BaseCleared);

        // Stop spawning, but you can optionally allow existing aliens to finish
        if (alienSpawner) alienSpawner.enabled = false;

        baseIndex++;
        if (baseIndex >= baseLatLon.Length)
        {
            SetUI("All bases cleared!", "GG");
            DisableEncounter();
            return;
        }

        SetState(State.NavigateToBase);
        if (arrow3D) arrow3D.gameObject.SetActive(true);
    }

    private void DisableEncounter()
    {
        if (alienSpawner) alienSpawner.enabled = false;
        if (playerShooter) playerShooter.enabled = false;
    }

    private void EnableNavigationUI()
    {
        if (arrow3D) arrow3D.gameObject.SetActive(true);
        
    }

    private void SetState(State s)
    {
        if (state == s) return;
        state = s;
        // Debug.Log("State: " + state);
    }

    private void SetUI(string status, string distance)
    {
        if (statusText) statusText.text = status;
        if (distanceText) distanceText.text = distance;
    }

    // Arrow points to base: (bearing to target) - (device heading)
    private void UpdateArrow(double myLat, double myLon, double targetLat, double targetLon)
    {
        if (!arrow3D) return;

        float bearing = (float)BearingDegrees(myLat, myLon, targetLat, targetLon); // 0=N
        float heading = Input.compass.trueHeading; // 0=N

        // Angle we need to turn relative to where the phone is facing
        float delta = Mathf.DeltaAngle(heading, bearing);

        // Rotate the arrow around the camera's up axis (yaw)
        arrow3D.localRotation = Quaternion.Euler(0f, delta, 0f);
    }

    // --- Math helpers ---
    private static double HaversineMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000.0;
        double dLat = Deg2Rad(lat2 - lat1);
        double dLon = Deg2Rad(lon2 - lon1);

        double a =
            Mathf.Sin((float)(dLat / 2)) * Mathf.Sin((float)(dLat / 2)) +
            Mathf.Cos((float)Deg2Rad(lat1)) * Mathf.Cos((float)Deg2Rad(lat2)) *
            Mathf.Sin((float)(dLon / 2)) * Mathf.Sin((float)(dLon / 2));

        double c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt((float)(1 - a)));
        return R * c;
    }

    private static double BearingDegrees(double lat1, double lon1, double lat2, double lon2)
    {
        // returns degrees where 0 = North, 90 = East
        double φ1 = Deg2Rad(lat1);
        double φ2 = Deg2Rad(lat2);
        double Δλ = Deg2Rad(lon2 - lon1);

        double y = System.Math.Sin(Δλ) * System.Math.Cos(φ2);
        double x = System.Math.Cos(φ1) * System.Math.Sin(φ2) -
                   System.Math.Sin(φ1) * System.Math.Cos(φ2) * System.Math.Cos(Δλ);

        double θ = System.Math.Atan2(y, x);
        double brng = (Rad2Deg(θ) + 360.0) % 360.0;
        return brng;
    }

    private static double Deg2Rad(double deg) => deg * System.Math.PI / 180.0;
    private static double Rad2Deg(double rad) => rad * 180.0 / System.Math.PI;

    // Call this from AlienHealth when an alien dies
    public void RegisterKill()
    {
        if (state == State.Encounter)
            killsThisBase++;
    }
}
