using UnityEngine;

public class CameraRTSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 20f;

    [Header("Zoom")]
    public float zoomSpeed = 200f;
    public float minZoom = 10f;
    public float maxZoom = 80f;

    // Terrain’den otomatik hesaplanacak
    private float minX, maxX, minZ, maxZ;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // TERRAIN'İ BUL → sınırları hesapla
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            float width = terrain.terrainData.size.x;
            float length = terrain.terrainData.size.z;

            minX = terrain.transform.position.x;
            maxX = terrain.transform.position.x + width;

            minZ = terrain.transform.position.z;
            maxZ = terrain.transform.position.z + length;
        }
        else
        {
            Debug.LogWarning("Terrain bulunamadı, kamera limitleri çalışmayacak!");
        }
    }

    void Update()
    {
        HandleKeyboardMovement();
        HandleZoom();
        ClampPosition();
    }

    void HandleKeyboardMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) move += Vector3.back;

        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;

        pos.y -= scroll * zoomSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);

        transform.position = pos;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }
}
