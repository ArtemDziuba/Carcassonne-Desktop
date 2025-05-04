using UnityEngine;
using UnityEngine.InputSystem;

public class MeepleSpawner : MonoBehaviour
{
    public GameObject meeplePrefab;
    public Sprite[] meepleSprites; // 5 ������� ����
    public int currentPlayerIndex = 0;

    private GameObject currentMeeple;
    private Camera mainCamera;
    private bool isPlacing = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void StartPlacingMeeple()
    {
        if (isPlacing || meeplePrefab == null) return;

        currentMeeple = Instantiate(meeplePrefab);
        isPlacing = true;

        // ���������� ���������� ������ ��� ����
        SpriteRenderer sr = currentMeeple.GetComponent<SpriteRenderer>();
        if (sr != null && meepleSprites.Length > currentPlayerIndex)
        {
            sr.sprite = meepleSprites[currentPlayerIndex];
        }
    }

    private void Update()
    {
        if (!isPlacing || currentMeeple == null) return;

        // �������� ������� ���� � Input System
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        currentMeeple.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlaceMeeple(mouseWorldPos);
        }
    }

    private void TryPlaceMeeple(Vector2 mouseWorldPos)
    {
        // ��������� ������� �� ���� � ���
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        int layerMask = LayerMask.GetMask("Default"); // ����� Default � ��� Meeple
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);

        if (hit.collider == null)
        {
            Debug.Log($"[MeepleSpawner] ͳ���� �� �������� � �����: {mouseWorldPos}");
            return;
        }

        Debug.Log($"[MeepleSpawner] ��������� �� ��'���: {hit.collider.name}");

        MeeplePlacementSlot slot = hit.collider.GetComponent<MeeplePlacementSlot>();
        if (slot == null)
        {
            Debug.Log("[MeepleSpawner] ��'��� �� � MeeplePlacementSlot.");
            return;
        }

        if (slot.IsOccupied)
        {
            Debug.Log("[MeepleSpawner] ���� ��� ��������.");
            return;
        }

        // ������ ��������� ����
        currentMeeple.transform.position = slot.transform.position;
        slot.IsOccupied = true;
        var sr = slot.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;
        slot.CurrentMeeple = currentMeeple;
        isPlacing = false;
        currentMeeple = null;

        Debug.Log($"[MeepleSpawner] ̳�� �������� �� ����: {slot.transform.position}");
    }

}
