using UnityEngine;

public class CameraController : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    [SerializeField] private Transform player;

    [SerializeField] private LevelManager levelManager;


    // =========================================================
    // CAMERA BOUNDARY
    // =========================================================

    [Header("Camera Boundary")]

    [Tooltip("The area that the camera is allowed to see.")]
    [SerializeField] private BoxCollider2D cameraBounds;


    // =========================================================
    // BUY PHASE CAMERA
    // =========================================================

    [Header("Buy Phase Camera")]

    [Tooltip("Camera position during the Buy Phase.")]
    [SerializeField] private Vector3 buyPhasePosition;

    [Tooltip("Camera zoom during the Buy Phase.")]
    [SerializeField] private float buyPhaseZoom = 6f;


    // =========================================================
    // PUZZLE PHASE CAMERA
    // =========================================================

    [Header("Puzzle Phase Camera")]

    [Tooltip("Camera zoom during the Puzzle Phase.")]
    [SerializeField] private float puzzlePhaseZoom = 5f;

    [Tooltip("How smoothly the camera follows the player.")]
    [SerializeField] private float followSmoothSpeed = 5f;

    [Tooltip("How smoothly the camera zooms.")]
    [SerializeField] private float zoomSmoothSpeed = 5f;


    // =========================================================
    // PLAYER OFFSET
    // =========================================================

    [Header("Player Camera Offset")]

    [Tooltip("Offset from the center of the player.")]
    [SerializeField] private Vector3 playerOffset =
        Vector3.zero;


    // =========================================================
    // INTERNAL
    // =========================================================

    private Camera cam;

    private Vector3 targetPosition;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        cam =
            GetComponent<Camera>();

        // Store the camera's starting position
        // if no Buy Phase position was assigned.

        if (
            buyPhasePosition == Vector3.zero
        )
        {
            buyPhasePosition =
                transform.position;
        }
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        SetBuyPhaseCamera();
    }


    // =========================================================
    // LATE UPDATE
    // =========================================================

    private void LateUpdate()
    {
        if (
            levelManager == null
        )
        {
            return;
        }


        // =====================================================
        // BUY PHASE
        // =====================================================

        if (
            levelManager.IsBuyingPhase()
        )
        {
            HandleBuyPhaseCamera();

            return;
        }


        // =====================================================
        // PUZZLE PHASE
        // =====================================================

        if (
            levelManager.IsPuzzlePhase()
        )
        {
            HandlePuzzlePhaseCamera();
        }
    }


    // =========================================================
    // BUY PHASE CAMERA
    // =========================================================

    private void HandleBuyPhaseCamera()
    {
        // Smoothly return to Buy Phase position.

        transform.position =
            Vector3.Lerp(
                transform.position,
                buyPhasePosition,
                followSmoothSpeed *
                Time.unscaledDeltaTime
            );


        // Smoothly return to Buy Phase zoom.

        cam.orthographicSize =
            Mathf.Lerp(
                cam.orthographicSize,
                buyPhaseZoom,
                zoomSmoothSpeed *
                Time.unscaledDeltaTime
            );
    }


    // =========================================================
    // PUZZLE PHASE CAMERA
    // =========================================================

    private void HandlePuzzlePhaseCamera()
    {
        if (
            player == null
        )
        {
            return;
        }


        // =====================================================
        // CALCULATE TARGET
        // =====================================================

        // The player is the center target.

        targetPosition =
            player.position +
            playerOffset;


        // Keep camera Z unchanged.

        targetPosition.z =
            transform.position.z;


        // =====================================================
        // CLAMP CAMERA TO LEVEL
        // =====================================================

        targetPosition =
            ClampCameraToBounds(
                targetPosition
            );


        // =====================================================
        // FOLLOW PLAYER
        // =====================================================

        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                followSmoothSpeed *
                Time.unscaledDeltaTime
            );


        // =====================================================
        // PUZZLE PHASE ZOOM
        // =====================================================

        cam.orthographicSize =
            Mathf.Lerp(
                cam.orthographicSize,
                puzzlePhaseZoom,
                zoomSmoothSpeed *
                Time.unscaledDeltaTime
            );
    }


    // =========================================================
    // CLAMP CAMERA
    // =========================================================

    private Vector3 ClampCameraToBounds(
        Vector3 desiredPosition
    )
    {
        // If no boundary was assigned,
        // simply return the desired position.

        if (
            cameraBounds == null
        )
        {
            return desiredPosition;
        }


        // =====================================================
        // GET CAMERA SIZE
        // =====================================================

        // Vertical size of the camera.

        float cameraHalfHeight =
            cam.orthographicSize;


        // Horizontal size depends
        // on the screen aspect ratio.

        float cameraHalfWidth =
            cam.orthographicSize *
            cam.aspect;


        // =====================================================
        // GET LEVEL BOUNDS
        // =====================================================

        Bounds bounds =
            cameraBounds.bounds;


        // =====================================================
        // CALCULATE MIN / MAX CAMERA POSITION
        // =====================================================

        float minX =
            bounds.min.x +
            cameraHalfWidth;

        float maxX =
            bounds.max.x -
            cameraHalfWidth;


        float minY =
            bounds.min.y +
            cameraHalfHeight;

        float maxY =
            bounds.max.y -
            cameraHalfHeight;


        // =====================================================
        // CLAMP X
        // =====================================================

        float clampedX =
            Mathf.Clamp(
                desiredPosition.x,
                minX,
                maxX
            );


        // =====================================================
        // CLAMP Y
        // =====================================================

        float clampedY =
            Mathf.Clamp(
                desiredPosition.y,
                minY,
                maxY
            );


        // =====================================================
        // RETURN CLAMPED POSITION
        // =====================================================

        return new Vector3(
            clampedX,
            clampedY,
            desiredPosition.z
        );
    }


    // =========================================================
    // SET BUY PHASE CAMERA
    // =========================================================

    private void SetBuyPhaseCamera()
    {
        transform.position =
            buyPhasePosition;

        cam.orthographicSize =
            buyPhaseZoom;
    }


    // =========================================================
    // DEBUG CAMERA BOUNDARY
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (
            cameraBounds == null
        )
        {
            return;
        }

        Gizmos.color =
            Color.yellow;

        Gizmos.DrawWireCube(
            cameraBounds.bounds.center,
            cameraBounds.bounds.size
        );
    }
}