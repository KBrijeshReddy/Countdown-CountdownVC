using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceableDragHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridFootprint footprint;
    [SerializeField] private Collider2D dragCollider;

    [Header("Drag Feel")]
    [SerializeField] private float pickupScale = 1.08f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color validColor = new Color(0.5f, 1f, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1f, 0.5f, 0.5f);

    private Purchasable purchasable;
    private Camera mainCamera;

    private Vector3 originalScale;
    private Vector3 buyAreaPosition;

    private bool isPlacedOnGrid;
    private Vector2Int gridPosition;

    private bool isDragging;
    private Vector3 dragStartPosition;
    private Vector2Int dragStartGridPosition;
    private bool dragStartedOnGrid;
    private Vector3 mouseOffset;

    private Vector2Int previewGridPosition;
    private bool previewValid;

    public bool IsPlacedOnGrid => isPlacedOnGrid;

    private void Awake()
    {
        purchasable = GetComponent<Purchasable>();
        mainCamera = Camera.main;
        originalScale = transform.localScale;
        buyAreaPosition = transform.position;

        if (dragCollider == null)
        {
            var found = transform.Find("DragCollider");
            if (found != null) dragCollider = found.GetComponent<Collider2D>();
        }

        if (dragCollider == null)
            Debug.LogError($"{name}: no DragCollider assigned or found as a child.");

        SetVisualColor(normalColor);
    }

    private void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
            TryStartDrag();

        if (isDragging)
        {
            FollowMouse();
            UpdatePlacementPreview();
        }

        if (mouse.leftButton.wasReleasedThisFrame && isDragging)
            FinishDrag();
    }

    private void TryStartDrag()
    {
        if (LevelManager.Instance != null && !LevelManager.Instance.IsBuyingPhase()) return;
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null || dragCollider == null) return;

        Vector3 worldPoint = ScreenToWorld(Mouse.current.position.ReadValue());
        if (!dragCollider.OverlapPoint(worldPoint)) return;

        StartDragging(worldPoint);
    }

    private void StartDragging(Vector3 mouseWorldPosition)
    {
        isDragging = true;
        dragStartPosition = transform.position;
        dragStartedOnGrid = isPlacedOnGrid;
        dragStartGridPosition = gridPosition;

        if (isPlacedOnGrid && GridManager.Instance != null)
            GridManager.Instance.RemoveObject(gridPosition, footprint);

        mouseWorldPosition.z = transform.position.z;
        mouseOffset = transform.position - mouseWorldPosition;

        transform.localScale = originalScale * pickupScale;
        SetVisualColor(normalColor);
    }

    private void FollowMouse()
    {
        if (Mouse.current == null || mainCamera == null) return;

        Vector3 mouseWorldPosition = ScreenToWorld(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = dragStartPosition.z;

        Vector3 targetPosition = mouseWorldPosition + mouseOffset;

        if (GridManager.Instance != null && footprint != null)
        {
            Vector2Int snapped = GridManager.Instance.WorldToObjectGrid(targetPosition, footprint);
            Vector2 snappedWorld = GridManager.Instance.ObjectGridToWorld(snapped, footprint);
            transform.position = new Vector3(snappedWorld.x, snappedWorld.y, dragStartPosition.z);
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    private void UpdatePlacementPreview()
    {
        if (GridManager.Instance == null || footprint == null) return;

        if (BuyArea.Instance != null && BuyArea.Instance.IsInsideBuyArea(transform.position))
        {
            previewValid = false;
            SetVisualColor(normalColor);
            return;
        }

        previewGridPosition = GridManager.Instance.WorldToObjectGrid(transform.position, footprint);
        previewValid = GridManager.Instance.CanPlaceObject(previewGridPosition, footprint);

        SetVisualColor(previewValid ? validColor : invalidColor);
    }

    private void FinishDrag()
    {
        isDragging = false;
        transform.localScale = originalScale;

        if (BuyArea.Instance != null && BuyArea.Instance.IsInsideBuyArea(transform.position))
        {
            ReturnToBuyArea();
            return;
        }

        if (!previewValid)
        {
            ReturnToPreviousPosition();
            return;
        }

        PlaceOnGrid();
    }

    private void PlaceOnGrid()
    {
        if (purchasable != null && !purchasable.IsPurchased && !purchasable.TryBuy())
        {
            ReturnToPreviousPosition();
            return;
        }

        gridPosition = previewGridPosition;
        isPlacedOnGrid = true;

        Vector2 snapped = GridManager.Instance.ObjectGridToWorld(gridPosition, footprint);
        transform.position = new Vector3(snapped.x, snapped.y, dragStartPosition.z);

        GridManager.Instance.PlaceObject(gridPosition, footprint);
        SetVisualColor(normalColor);
    }

    private void ReturnToBuyArea()
    {
        if (purchasable != null && purchasable.IsPurchased)
            purchasable.Sell();

        isPlacedOnGrid = false;
        gridPosition = Vector2Int.zero;
        transform.position = buyAreaPosition;

        SetVisualColor(normalColor);
    }

    private void ReturnToPreviousPosition()
    {
        transform.position = dragStartPosition;
        isPlacedOnGrid = dragStartedOnGrid;

        if (dragStartedOnGrid)
        {
            gridPosition = dragStartGridPosition;
            GridManager.Instance.PlaceObject(gridPosition, footprint);
        }

        SetVisualColor(normalColor);
    }

    private Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        return mainCamera.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, Mathf.Abs(mainCamera.transform.position.z)));
    }

    private void SetVisualColor(Color color)
    {
        if (visual != null) visual.color = color;
    }
}