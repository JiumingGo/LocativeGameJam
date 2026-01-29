using UnityEngine;

public class AlienSpawner : MonoBehaviour
{
    [Header("Prefab")] 
    [SerializeField] private GameObject alienPrefab;
    
    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 1.5f;
    
    [Header("Spawn Distance")]
    [SerializeField] private float minDistance = 4f;
    [SerializeField] private float maxDistance = 8f;
    
    [Header("Height")]
    [Tooltip("Offset relative to camera height. 0 = same height as camera.")]
    [SerializeField] private float heightOffset  = -0.3f;
    
    [Header("Direction")]
    [Tooltip("If true, spawn mostly around you horizontally (Face Raiders vibe).")]
    [SerializeField] private bool horizontalOnly = true;

    private Transform cam;
    private float timer;
    private Camera _camera;

    private void Start()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
    }

    private void Update()
    {
        if (!alienPrefab) return;

        if (!cam)
        {
            if (Camera.main != null) cam = Camera.main.transform;
            return;
        }
        
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    private void SpawnOne()
    {
        float dist = Random.Range(minDistance, maxDistance);

        Vector3 dir;
        if (horizontalOnly)
        {
            // Random direction on XZ plane
            Vector2 v = Random.insideUnitCircle.normalized;
            dir = new Vector3(v.x, 0f, v.y);
        }
        else
        {
            // Random direction on sphere
            dir = Random.onUnitSphere;
        }

        Vector3 spawnPos = cam.position + dir * dist;
        spawnPos.y = cam.position.y + heightOffset;

        Quaternion rot = Quaternion.LookRotation((cam.position - spawnPos).normalized, Vector3.up);

        Instantiate(alienPrefab, spawnPos, rot);
    }
}
