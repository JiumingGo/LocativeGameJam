using TMPro;
using UnityEngine;

public class LocationDebugUI : MonoBehaviour
{
    [SerializeField] private TMP_Text locationText;
    [SerializeField] private float updateInterval = 1.0f;

    private float timer;

    void Update()
    {
        if (Input.location.status != LocationServiceStatus.Running)
        {
            if (locationText)
                locationText.text = "GPS: Not running";
            return;
        }

        timer += Time.deltaTime;
        if (timer < updateInterval) return;
        timer = 0f;

        var data = Input.location.lastData;

        if (locationText)
        {
            locationText.text =
                $"Lat: {data.latitude:F6}\n" +
                $"Lon: {data.longitude:F6}\n";
        }
    }
}
