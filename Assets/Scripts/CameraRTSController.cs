using UnityEngine;

public class CameraRTSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 20f;
    public float edgeSize = 20f;

    [Header("Zoom")]
    public float zoomSpeed = 200f;
    public float minZoom = 10f;
    public float maxZoom = 80f;

    [Header("Limits")]
    public float minX = -50;
    public float maxX = 50;
    public float minZ = -50;
    public float maxZ = 50;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleKeyboardMovement();
        HandleEdgeScroll();
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

    void HandleEdgeScroll()
    {
        Vector3 move = Vector3.zero;

        if (Input.mousePosition.y >= Screen.height - edgeSize)
            move += Vector3.forward;

        if (Input.mousePosition.y <= edgeSize)
            move += Vector3.back;

        if (Input.mousePosition.x >= Screen.width - edgeSize)
            move += Vector3.right;

        if (Input.mousePosition.x <= edgeSize)
            move += Vector3.left;

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
