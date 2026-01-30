using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class LocationServiceStarter : MonoBehaviour
{
    [SerializeField] private float desiredAccuracyMeters = 10f;
    [SerializeField] private float updateDistanceMeters = 1f;

    public bool Ready { get; private set; }

    private IEnumerator Start()
    {
        // Request permission first (Android)
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            // Wait a moment for the permission dialog result
            yield return new WaitForSeconds(1f);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.LogWarning("Location permission denied by user.");
            yield break;
        }

        // Now check device location toggle
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location not enabled by user (GPS toggle off).");
            yield break;
        }

        Input.compass.enabled = true;
        Input.location.Start(desiredAccuracyMeters, updateDistanceMeters);

        int maxWait = 15;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogWarning("Location service failed: " + Input.location.status);
            yield break;
        }

        Ready = true;
        Debug.Log("Location + compass ready.");
    }
}
