using UnityEngine;

[RequireComponent(typeof(GridFootprint))]
[RequireComponent(typeof(PlaceableDragHandler))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(1)]
    private int maximumMovementWidth = 9;

    [SerializeField, Min(0.01f)]
    private float speed = 2f;

    private GridFootprint footprint;
    private PlaceableDragHandler dragHandler;
    private Rigidbody2D rb;

    private LevelManager levelManager;
    private GridManager gridManager;

    private Vector2Int initialGridPosition;
    private Vector2Int leftGridPosition;
    private Vector2Int rightGridPosition;

    private float leftWorldX;
    private float rightWorldX;

    private bool isMoving;
    private bool movingRight;

    private void Awake()
    {
        footprint = GetComponent<GridFootprint>();
        dragHandler = GetComponent<PlaceableDragHandler>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        SubscribeToLevelManager();
    }

    private void Start()
    {
        SubscribeToLevelManager();
        gridManager = GridManager.Instance;
    }

    private void OnDisable()
    {
        if (levelManager != null)
            levelManager.PuzzlePhaseStarted -= StartMovement;
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;

        if (levelManager == null)
            return;

        levelManager.PuzzlePhaseStarted -= StartMovement;
        levelManager.PuzzlePhaseStarted += StartMovement;
    }

    private void Update()
    {
        if (!isMoving || rb != null)
            return;

        Move(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!isMoving || rb == null)
            return;

        Move(Time.fixedDeltaTime);
    }

    private void StartMovement()
    {
        if (isMoving)
            return;

        if (levelManager == null ||
            !levelManager.IsPuzzlePhase())
        {
            return;
        }

        if (gridManager == null)
            gridManager = GridManager.Instance;

        if (gridManager == null)
        {
            Debug.LogError(
                $"{name}: GridManager is missing."
            );

            return;
        }

        if (!dragHandler.IsPlacedOnGrid)
            return;

        if (!ValidateConfiguration())
            return;

        initialGridPosition =
            gridManager.WorldToObjectGrid(
                transform.position,
                footprint
            );

        if (!TryBuildMovementTrack())
            return;

        isMoving = true;
        movingRight = true;

        SnapToInitialPosition();
    }

    private bool ValidateConfiguration()
    {
        Vector2Int size = footprint.Size;

        if (size.x != 3 || size.y != 1)
        {
            Debug.LogError(
                $"{name}: MovingPlatform requires a 3x1 GridFootprint."
            );

            return false;
        }

        if (maximumMovementWidth < size.x)
        {
            Debug.LogError(
                $"{name}: Maximum movement width cannot be smaller than the platform width."
            );

            return false;
        }

        return true;
    }

    private bool TryBuildMovementTrack()
    {
        int platformWidth = footprint.Size.x;

        int maximumTravelDistance =
            maximumMovementWidth - platformWidth;

        int maxLeftDistance =
            Mathf.FloorToInt(
                maximumTravelDistance / 2f
            );

        int maxRightDistance =
            Mathf.CeilToInt(
                maximumTravelDistance / 2f
            );

        int leftDistance =
            FindAvailableDistance(
                -1,
                maxLeftDistance
            );

        int rightDistance =
            FindAvailableDistance(
                1,
                maxRightDistance
            );

        leftGridPosition =
            new Vector2Int(
                initialGridPosition.x - leftDistance,
                initialGridPosition.y
            );

        rightGridPosition =
            new Vector2Int(
                initialGridPosition.x + rightDistance,
                initialGridPosition.y
            );

        leftWorldX =
            gridManager.ObjectGridToWorld(
                leftGridPosition,
                footprint
            ).x;

        rightWorldX =
            gridManager.ObjectGridToWorld(
                rightGridPosition,
                footprint
            ).x;

        return true;
    }

    private int FindAvailableDistance(
        int direction,
        int maximumDistance)
    {
        int availableDistance = 0;

        for (int distance = 1;
             distance <= maximumDistance;
             distance++)
        {
            Vector2Int testPosition =
                new Vector2Int(
                    initialGridPosition.x +
                    direction * distance,

                    initialGridPosition.y
                );

            if (!CanOccupyPosition(testPosition))
                break;

            availableDistance = distance;
        }

        return availableDistance;
    }

    private bool CanOccupyPosition(
        Vector2Int bottomLeftCell)
    {
        Vector2Int size =
            footprint.Size;

        for (int x = 0;
             x < size.x;
             x++)
        {
            for (int y = 0;
                 y < size.y;
                 y++)
            {
                int cellX =
                    bottomLeftCell.x + x;

                int cellY =
                    bottomLeftCell.y + y;

                if (cellX < 0 ||
                    cellX >= gridManager.GridWidth ||
                    cellY < 0 ||
                    cellY >= gridManager.GridHeight)
                {
                    return false;
                }

                if (gridManager.HasAnythingAt(
                    new Vector2Int(
                        cellX,
                        cellY
                    )))
                {
                    if (!IsOwnCell(
                        cellX,
                        cellY))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool IsOwnCell(
        int cellX,
        int cellY)
    {
        Vector2Int ownPosition =
            initialGridPosition;

        Vector2Int size =
            footprint.Size;

        return cellX >= ownPosition.x &&
               cellX < ownPosition.x + size.x &&
               cellY >= ownPosition.y &&
               cellY < ownPosition.y + size.y;
    }

    private void Move(float deltaTime)
    {
        float currentX =
            rb != null
                ? rb.position.x
                : transform.position.x;

        float direction =
            movingRight
                ? 1f
                : -1f;

        float nextX =
            currentX +
            direction *
            speed *
            deltaTime;

        if (movingRight &&
            nextX >= rightWorldX)
        {
            nextX = rightWorldX;
            movingRight = false;
        }
        else if (!movingRight &&
                 nextX <= leftWorldX)
        {
            nextX = leftWorldX;
            movingRight = true;
        }

        Vector2 nextPosition =
            new Vector2(
                nextX,
                transform.position.y
            );

        if (rb != null)
        {
            rb.MovePosition(
                nextPosition
            );
        }
        else
        {
            transform.position =
                new Vector3(
                    nextPosition.x,
                    nextPosition.y,
                    transform.position.z
                );
        }
    }

    private void SnapToInitialPosition()
    {
        Vector2 initialWorldPosition =
            gridManager.ObjectGridToWorld(
                initialGridPosition,
                footprint
            );

        Vector2 position =
            new Vector2(
                initialWorldPosition.x,
                initialWorldPosition.y
            );

        if (rb != null)
        {
            rb.position = position;
        }
        else
        {
            transform.position =
                new Vector3(
                    position.x,
                    position.y,
                    transform.position.z
                );
        }
    }
}