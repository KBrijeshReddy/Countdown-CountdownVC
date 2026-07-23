using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableObject : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public GridObject gridObject;
    public LevelManager levelManager;
    public BuyArea buyArea;

    [Header("Dragging")]
    public Collider2D dragCollider;

    [Header("Visuals")]
    public float pickupScale = 1.08f;

    public Color normalColor = Color.white;

    public Color validColor =
        new Color(
            0.5f,
            1f,
            0.5f
        );

    public Color invalidColor =
        new Color(
            1f,
            0.5f,
            0.5f
        );

    // =========================================================
    // COMPONENTS
    // =========================================================

    private BuyableObject buyableObject;

    private SpriteRenderer spriteRenderer;

    private Camera mainCamera;

    // =========================================================
    // BUY AREA
    // =========================================================

    private Vector3 buyAreaPosition;

    // =========================================================
    // SCALE
    // =========================================================

    private Vector3 originalScale;

    // =========================================================
    // GRID STATE
    // =========================================================

    private bool isPlacedOnGrid;

    private Vector2Int currentGridPosition;

    // =========================================================
    // DRAG STATE
    // =========================================================

    private bool isDragging;

    private Vector3 dragStartPosition;

    private Vector2Int dragStartGridPosition;

    private bool dragStartedOnGrid;

    // =========================================================
    // PREVIEW
    // =========================================================

    private Vector2Int currentPreviewPosition;

    private bool currentPreviewValid;

    // =========================================================
    // MOUSE OFFSET
    // =========================================================

    private Vector3 mouseOffset;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        mainCamera =
            Camera.main;

        spriteRenderer =
            GetComponent<SpriteRenderer>();

        buyableObject =
            GetComponent<BuyableObject>();

        originalScale =
            transform.localScale;

        buyAreaPosition =
            transform.position;

        isPlacedOnGrid =
            false;

        isDragging =
            false;

        currentPreviewValid =
            false;

        // =====================================================
        // AUTO FIND DRAG COLLIDER
        // =====================================================

        if (dragCollider == null)
        {
            Transform dragTransform =
                transform.Find(
                    "DragCollider"
                );

            if (
                dragTransform != null
            )
            {
                dragCollider =
                    dragTransform.GetComponent<Collider2D>();
            }
        }

        // =====================================================
        // DEBUG
        // =====================================================

        if (
            dragCollider == null
        )
        {
            Debug.LogError(
                gameObject.name +
                " has no DragCollider assigned!"
            );
        }

        SetColor(
            normalColor
        );
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // =====================================================
        // START DRAG
        // =====================================================

        if (
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame
        )
        {
            CheckForMouseClick();
        }

        // =====================================================
        // DRAG
        // =====================================================

        if (isDragging)
        {
            DragObject();

            UpdatePlacementPreview();
        }

        // =====================================================
        // RELEASE
        // =====================================================

        if (
            Mouse.current != null &&
            Mouse.current.leftButton.wasReleasedThisFrame
        )
        {
            if (isDragging)
            {
                FinishDrag();
            }
        }
    }

    // =========================================================
    // CHECK MOUSE CLICK
    // =========================================================

    private void CheckForMouseClick()
    {
        // =====================================================
        // BUY PHASE
        // =====================================================

        if (
            levelManager != null &&
            !levelManager.IsBuyingPhase()
        )
        {
            return;
        }

        // =====================================================
        // CAMERA
        // =====================================================

        if (
            mainCamera == null
        )
        {
            mainCamera =
                Camera.main;
        }

        if (
            mainCamera == null
        )
        {
            return;
        }

        // =====================================================
        // MOUSE WORLD POSITION
        // =====================================================

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    Mathf.Abs(
                        mainCamera.transform.position.z
                    )
                )
            );

        // =====================================================
        // CHECK DRAG COLLIDER
        // =====================================================

        if (
            dragCollider == null
        )
        {
            return;
        }

        bool clicked =
            dragCollider.OverlapPoint(
                mouseWorldPosition
            );

        if (!clicked)
        {
            return;
        }

        // =====================================================
        // START DRAG
        // =====================================================

        StartDragging(
            mouseWorldPosition
        );
    }

    // =========================================================
    // START DRAGGING
    // =========================================================

    private void StartDragging(
        Vector3 mouseWorldPosition
    )
    {
        if (isDragging)
        {
            return;
        }

        // =====================================================
        // SAVE STATE
        // =====================================================

        isDragging =
            true;

        dragStartPosition =
            transform.position;

        dragStartedOnGrid =
            isPlacedOnGrid;

        dragStartGridPosition =
            currentGridPosition;

        // =====================================================
        // REMOVE FROM GRID
        // =====================================================

        if (
            isPlacedOnGrid &&
            gridManager != null
        )
        {
            gridManager.RemoveObject(
                currentGridPosition,
                gridObject
            );
        }

        // =====================================================
        // MOUSE OFFSET
        // =====================================================

        mouseWorldPosition.z =
            transform.position.z;

        mouseOffset =
            transform.position -
            mouseWorldPosition;

        // =====================================================
        // SCALE UP
        // =====================================================

        transform.localScale =
            originalScale *
            pickupScale;

        SetColor(
            normalColor
        );
    }

    // =========================================================
    // DRAG
    // =========================================================

    private void DragObject()
    {
        if (
            Mouse.current == null ||
            mainCamera == null
        )
        {
            return;
        }

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    Mathf.Abs(
                        mainCamera.transform.position.z
                    )
                )
            );

        mouseWorldPosition.z =
            dragStartPosition.z;

        // =====================================================
        // FOLLOW MOUSE
        // =====================================================

        transform.position =
            mouseWorldPosition +
            mouseOffset;
    }

    // =========================================================
    // UPDATE PREVIEW
    // =========================================================

    private void UpdatePlacementPreview()
    {
        if (
            gridManager == null ||
            gridObject == null
        )
        {
            return;
        }

        // =====================================================
        // INSIDE BUY AREA
        // =====================================================

        bool insideBuyArea =
            buyArea != null &&
            buyArea.IsInsideBuyArea(
                transform.position
            );

        if (insideBuyArea)
        {
            currentPreviewValid =
                false;

            SetColor(
                normalColor
            );

            return;
        }

        // =====================================================
        // GRID POSITION
        // =====================================================

        currentPreviewPosition =
            gridManager.WorldToObjectGrid(
                transform.position,
                gridObject
            );

        // =====================================================
        // VALIDATION
        // =====================================================

        currentPreviewValid =
            gridManager.CanPlaceObject(
                currentPreviewPosition,
                gridObject
            );

        // =====================================================
        // COLOR
        // =====================================================

        if (currentPreviewValid)
        {
            SetColor(
                validColor
            );
        }
        else
        {
            SetColor(
                invalidColor
            );
        }
    }

    // =========================================================
    // FINISH DRAG
    // =========================================================

    private void FinishDrag()
    {
        isDragging =
            false;

        transform.localScale =
            originalScale;

        // =====================================================
        // BUY AREA
        // =====================================================

        if (
            buyArea != null &&
            buyArea.IsInsideBuyArea(
                transform.position
            )
        )
        {
            ReturnToBuyArea();

            return;
        }

        // =====================================================
        // INVALID
        // =====================================================

        if (
            !currentPreviewValid
        )
        {
            ReturnToPreviousPosition();

            return;
        }

        // =====================================================
        // PLACE
        // =====================================================

        PlaceOnGrid();
    }

    // =========================================================
    // PLACE ON GRID
    // =========================================================

    private void PlaceOnGrid()
    {
        // =====================================================
        // BUY
        // =====================================================

        if (
            buyableObject != null &&
            !buyableObject.IsPurchased
        )
        {
            bool bought =
                buyableObject.TryBuy();

            if (!bought)
            {
                ReturnToPreviousPosition();

                return;
            }
        }

        // =====================================================
        // GRID STATE
        // =====================================================

        currentGridPosition =
            currentPreviewPosition;

        isPlacedOnGrid =
            true;

        // =====================================================
        // SNAP
        // =====================================================

        Vector2 snappedPosition =
            gridManager.ObjectGridToWorld(
                currentGridPosition,
                gridObject
            );

        transform.position =
            new Vector3(
                snappedPosition.x,
                snappedPosition.y,
                dragStartPosition.z
            );

        // =====================================================
        // REGISTER
        // =====================================================

        gridManager.PlaceObject(
            currentGridPosition,
            gridObject
        );

        SetColor(
            normalColor
        );
    }

    // =========================================================
    // RETURN TO BUY AREA
    // =========================================================

    private void ReturnToBuyArea()
    {
        // Refund.
        if (
            buyableObject != null &&
            buyableObject.IsPurchased
        )
        {
            buyableObject.Sell();
        }

        isPlacedOnGrid =
            false;

        currentGridPosition =
            Vector2Int.zero;

        transform.position =
            buyAreaPosition;

        SetColor(
            normalColor
        );
    }

    // =========================================================
    // RETURN TO PREVIOUS
    // =========================================================

    private void ReturnToPreviousPosition()
    {
        transform.position =
            dragStartPosition;

        if (dragStartedOnGrid)
        {
            currentGridPosition =
                dragStartGridPosition;

            isPlacedOnGrid =
                true;

            gridManager.PlaceObject(
                currentGridPosition,
                gridObject
            );
        }
        else
        {
            isPlacedOnGrid =
                false;
        }

        SetColor(
            normalColor
        );
    }

    // =========================================================
    // COLOR
    // =========================================================

    private void SetColor(
        Color color
    )
    {
        if (
            spriteRenderer != null
        )
        {
            spriteRenderer.color =
                color;
        }
    }
}