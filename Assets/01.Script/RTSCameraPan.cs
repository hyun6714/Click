using UnityEngine;
using UnityEngine.InputSystem;

public class RTSCameraPan : MonoBehaviour
{
    private Vector3 dragStartPosition;
    private Camera cam;


    [Header("줌 설정")]
    public float zoomSpeed = 5f; 
    public float minZoom = 3f;
    public float maxZoom = 20f;


    [Header("카메라 이동 속도")]
    public float panSpeed = 0.5f;

    [Header("마름모(Rhombus) 맵 경계 설정")]
    public Vector2 mapCenter = Vector2.zero;
    public float halfWidth = 45f;            
    public float halfHeight = 25f;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (Mouse.current == null)
        {
            return;
        }

        HandleCameraPan();

        HandleCameraZoom();
    }

    private void HandleCameraPan()
    {
        if (!Mouse.current.rightButton.isPressed)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        if (mouseDelta == Vector2.zero)
        {
            return;
        }

        float moveX = -mouseDelta.x * (cam.orthographicSize / Screen.height) * panSpeed * 2f;
        float moveY = -mouseDelta.y * (cam.orthographicSize / Screen.height) * panSpeed * 2f;

        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0f);

        Vector2 clampedPos = ClampToRhombus(newPosition, mapCenter, halfWidth, halfHeight);
        transform.position = new Vector3(clampedPos.x, clampedPos.y, transform.position.z);
    }

    private void HandleCameraZoom()
    {
        float scrollValue = Mouse.current.scroll.ReadValue().y;
        if (scrollValue == 0f)
        {
            return;
        }

        cam.orthographicSize -= scrollValue * zoomSpeed * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    Vector2 ClampToRhombus(Vector2 pos, Vector2 center, float hw, float hh)
    {
        float dx = pos.x - center.x;
        float dy = pos.y - center.y;

        float normX = Mathf.Abs(dx) / hw;
        float normY = Mathf.Abs(dy) / hh;
        float sum = normX + normY;

        if (sum > 1f)
        {
            dx /= sum;
            dy /= sum;
        }

        return new Vector2(center.x + dx, center.y + dy);
    }
}
