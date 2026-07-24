using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LevelManager levelManager;

    [Header("Camera Boundary")]
    [SerializeField] private BoxCollider2D cameraBounds;

    [Header("Buy Phase Camera")]
    [SerializeField] private Vector3 buyPhasePosition;
    [SerializeField] private float buyPhaseZoom = 6f;

    [Header("Puzzle Phase Camera")]
    [SerializeField] private float puzzlePhaseZoom = 5f;
    [SerializeField] private float followSmoothSpeed = 5f;
    [SerializeField] private float zoomSmoothSpeed = 5f;

    [Header("Player Offset")]
    [SerializeField] private Vector3 playerOffset = Vector3.zero;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (buyPhasePosition == Vector3.zero)
            buyPhasePosition = transform.position;
    }

    private void Start()
    {
        transform.position = buyPhasePosition;
        cam.orthographicSize = buyPhaseZoom;
    }

    private void LateUpdate()
    {
        if (levelManager == null)
            return;

        if (levelManager.IsBuyingPhase())
            FollowBuyPhase();
        else if (levelManager.IsPuzzlePhase())
            FollowPuzzlePhase();
    }

    private void FollowBuyPhase()
    {
        transform.position = Vector3.Lerp(transform.position, buyPhasePosition, followSmoothSpeed * Time.unscaledDeltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, buyPhaseZoom, zoomSmoothSpeed * Time.unscaledDeltaTime);
    }

    private void FollowPuzzlePhase()
    {
        if (player == null)
            return;

        Vector3 target = player.position + playerOffset;
        target.z = transform.position.z;
        target = ClampToBounds(target);

        transform.position = Vector3.Lerp(transform.position, target, followSmoothSpeed * Time.unscaledDeltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, puzzlePhaseZoom, zoomSmoothSpeed * Time.unscaledDeltaTime);
    }

    private Vector3 ClampToBounds(Vector3 desired)
    {
        if (cameraBounds == null)
            return desired;

        float halfHeight = cam.orthographicSize;
        float halfWidth = cam.orthographicSize * cam.aspect;

        Bounds bounds = cameraBounds.bounds;

        float minX = bounds.min.x + halfWidth;
        float maxX = bounds.max.x - halfWidth;
        float minY = bounds.min.y + halfHeight;
        float maxY = bounds.max.y - halfHeight;

        return new Vector3(
            Mathf.Clamp(desired.x, minX, maxX),
            Mathf.Clamp(desired.y, minY, maxY),
            desired.z);
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraBounds == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(cameraBounds.bounds.center, cameraBounds.bounds.size);
    }
}