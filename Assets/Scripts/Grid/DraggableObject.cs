using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableObject : MonoBehaviour
{
    [Header("Grid")]
    public GridManager gridManager;

    [Header("Pickup Animation")]
    public float pickupScaleMultiplier = 1.1f;
    public float pickupAnimationSpeed = 8f;

    [Header("Placement Colors")]
    public Color normalColor = Color.white;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

    private Camera mainCamera;
    private GridObject gridObject;

    private SpriteRenderer spriteRenderer;

    private bool isDragging;

    private Vector2Int currentGridPosition;

    private Vector2Int previousValidGridPosition;

    // Original scale of the object.
    private Vector3 originalScale;

    // Target scale used for smooth animation.
    private Vector3 targetScale;

    private void Awake()
    {
        mainCamera = Camera.main;

        gridObject =
            GetComponent<GridObject>();

        spriteRenderer =
            GetComponent<SpriteRenderer>();

        // Remember original size.
        originalScale =
            transform.localScale;

        targetScale =
            originalScale;
    }

    private void Start()
    {
        // Find starting grid position.
        currentGridPosition =
            gridManager.WorldToObjectGrid(
                transform.position,
                gridObject
            );

        previousValidGridPosition =
            currentGridPosition;

        // Check starting position.
        if (
            gridManager.CanPlaceObject(
                currentGridPosition,
                gridObject
            )
        )
        {
            gridManager.PlaceObject(
                currentGridPosition,
                gridObject
            );

            SnapToGrid(
                currentGridPosition
            );
        }
        else
        {
            Debug.LogWarning(
                gameObject.name +
                " is outside the grid or overlapping another object."
            );
        }
    }

    private void Update()
    {
        // Smoothly animate scale.
        transform.localScale =
            Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime *
                pickupAnimationSpeed
            );

        if (!isDragging)
            return;

        DragObject();
    }

    // =========================================================
    // START DRAGGING
    // =========================================================

    private void OnMouseDown()
    {
        isDragging = true;

        // Remove object from grid.
        gridManager.RemoveObject(
            currentGridPosition,
            gridObject
        );

        // -----------------------------------------------------
        // PICKUP SCALE EFFECT
        // -----------------------------------------------------

        // Immediately make it slightly bigger.
        transform.localScale =
            originalScale *
            pickupScaleMultiplier;

        // Then smoothly return to normal size.
        targetScale =
            originalScale;

        // Start with normal color.
        SetColor(
            normalColor
        );
    }

    // =========================================================
    // STOP DRAGGING
    // =========================================================

    private void OnMouseUp()
    {
        isDragging = false;

        // Find the grid position where
        // the object was released.
        Vector2Int releasedGridPosition =
            gridManager.WorldToObjectGrid(
                transform.position,
                gridObject
            );

        // Check entire object.
        bool validPosition =
            gridManager.CanPlaceObject(
                releasedGridPosition,
                gridObject
            );

        if (validPosition)
        {
            // =================================================
            // VALID PLACEMENT
            // =================================================

            currentGridPosition =
                releasedGridPosition;

            previousValidGridPosition =
                currentGridPosition;

            SnapToGrid(
                currentGridPosition
            );

            gridManager.PlaceObject(
                currentGridPosition,
                gridObject
            );
        }
        else
        {
            // =================================================
            // INVALID PLACEMENT
            // =================================================

            // Return to previous valid position.
            currentGridPosition =
                previousValidGridPosition;

            SnapToGrid(
                currentGridPosition
            );

            gridManager.PlaceObject(
                currentGridPosition,
                gridObject
            );
        }

        // Return to normal color.
        SetColor(
            normalColor
        );
    }

    // =========================================================
    // DRAG OBJECT
    // =========================================================

    private void DragObject()
    {
        if (Mouse.current == null)
            return;

        // Get mouse position.
        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        // Convert to world position.
        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    -mainCamera.transform.position.z
                )
            );

        // Find grid position.
        Vector2Int gridPosition =
            gridManager.WorldToObjectGrid(
                mouseWorldPosition,
                gridObject
            );

        // Snap to grid.
        SnapToGrid(
            gridPosition
        );

        // Check whether the ENTIRE object
        // can be placed here.
        bool validPosition =
            gridManager.CanPlaceObject(
                gridPosition,
                gridObject
            );

        // =====================================================
        // VALID
        // =====================================================

        if (validPosition)
        {
            SetColor(
                validColor
            );
        }

        // =====================================================
        // INVALID
        // =====================================================

        else
        {
            SetColor(
                invalidColor
            );
        }
    }

    // =========================================================
    // SNAP TO GRID
    // =========================================================

    private void SnapToGrid(
        Vector2Int gridPosition
    )
    {
        Vector2 worldPosition =
            gridManager.ObjectGridToWorld(
                gridPosition,
                gridObject
            );

        transform.position =
            new Vector3(
                worldPosition.x,
                worldPosition.y,
                transform.position.z
            );
    }

    // =========================================================
    // CHANGE COLOR
    // =========================================================

    private void SetColor(
        Color color
    )
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color =
            color;
    }
}