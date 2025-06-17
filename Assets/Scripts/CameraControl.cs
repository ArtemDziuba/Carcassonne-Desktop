using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ����, �� ������� �� ���������� ������ �� ����� ���
[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    public static CameraControl Instance;

    [Header("Zoom Settings")]
    [Tooltip("�������� ������������� ��������� ����")]
    public float zoomSpeed;

    private float minSize = 3f;
    private float maxSize = 15f;

    [Header("Pan Settings")]
    [Tooltip("�������� �������������")]
    public float panSpeed;

    [Header("Bounds Settings")]
    [Tooltip("SpriteRenderer ���� (background),\n����� ������� ����� ��������� ��� ������")]
    public SpriteRenderer backgroundRenderer;

    private Camera cam;

    public Slider zoomSpeedSlider;
    public Slider panSpeedSlider;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // ������� ��������
        }
        
        zoomSpeed = GameConfig.Instance.zoomSpeed;
        panSpeed = GameConfig.Instance.panSpeed;

        zoomSpeedSlider.value = zoomSpeed;
        panSpeedSlider.value = panSpeed;

            cam = GetComponent<Camera>();
        if (!cam.orthographic)
            Debug.LogWarning("CameraZoom ������������ �� ������������ ������.");
        if (backgroundRenderer == null)
            Debug.LogWarning("�� ������� backgroundRenderer � ������ �� ���� �������� �� ��������.");
    }

    void Update()
    {
        HandleZoom();
        HandleMousePan();
        HandleKeyboardPan();
        ClampCameraPosition();
    }    

    public void SetZoomSpeed()
    {
        zoomSpeed = zoomSpeedSlider.value;
        GameConfig.Instance.zoomSpeed = zoomSpeed;
    }

    public void SetPanSpeed()
    {
        panSpeed = panSpeedSlider.value;
        GameConfig.Instance.panSpeed = panSpeed;
    }

    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (!Mathf.Approximately(scroll, 0f))
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
        }
    }

    private void HandleMousePan()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            // ��������� ���, ��� ��� ��� �������� ���������� �������� ������ � �������
            Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * panSpeed * Time.deltaTime;
            cam.transform.Translate(move, Space.World);
        }
    }

    private void HandleKeyboardPan()
    {
        Vector2 dir = Vector2.zero;
        if (Keyboard.current.upArrowKey.isPressed) dir.y += 1;
        if (Keyboard.current.downArrowKey.isPressed) dir.y -= 1;
        if (Keyboard.current.leftArrowKey.isPressed) dir.x -= 1;
        if (Keyboard.current.rightArrowKey.isPressed) dir.x += 1;

        if (dir != Vector2.zero)
        {
            Vector3 move = new Vector3(dir.x, dir.y, 0f) * panSpeed * Time.deltaTime;
            cam.transform.Translate(move, Space.World);
        }
    }

    private void ClampCameraPosition()
    {
        if (backgroundRenderer == null)
            return;

        // �������� ����� ������� ������� ����
        Bounds bounds = backgroundRenderer.bounds;

        // ϳ������� �� �������� ���� ������ � ������� ��������
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        Vector3 pos = cam.transform.position;

        // �������� x �� y ���, ��� ������ �� �������� �� ��� ����
        pos.x = Mathf.Clamp(pos.x, bounds.min.x + horzExtent, bounds.max.x - horzExtent);
        pos.y = Mathf.Clamp(pos.y, bounds.min.y + vertExtent, bounds.max.y - vertExtent);

        cam.transform.position = pos;
    }
}
