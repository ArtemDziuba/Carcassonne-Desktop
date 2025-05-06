using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 100f;
    public float minSize = 3f;
    public float maxSize = 15f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // „итаЇмо скрол через Input System
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0f)
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
        }
    }
}
