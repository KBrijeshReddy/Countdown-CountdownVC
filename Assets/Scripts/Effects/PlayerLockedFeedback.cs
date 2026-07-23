using UnityEngine;

public class PlayerLockedFeedback : MonoBehaviour
{
    // [Header("References")]
    // public LevelManager levelManager;

    // [Header("Player Model")]
    // public Transform playerModel;

    // [Header("Camera")]
    // public Transform cameraTransform;

    // [Header("Shake Settings")]
    // public float shakeDuration = 0.12f;

    // public float playerShakeAmount = 0.04f;

    // public float cameraShakeAmount = 0.025f;

    // [Header("Movement Input")]
    // public float movementThreshold = 0.1f;

    // private Vector3 playerModelOriginalPosition;

    // private Vector3 cameraOriginalPosition;

    // private float shakeTimer;

    // private bool wasTryingToMove;


    // private void Start()
    // {
    //     // Store original positions.

    //     if (playerModel != null)
    //     {
    //         playerModelOriginalPosition =
    //             playerModel.localPosition;
    //     }

    //     if (cameraTransform != null)
    //     {
    //         cameraOriginalPosition =
    //             cameraTransform.localPosition;
    //     }
    // }


    // private void Update()
    // {
    //     // =====================================================
    //     // ONLY WORK DURING BUYING PHASE
    //     // =====================================================

    //     if (
    //         levelManager == null ||
    //         !levelManager.IsBuyingPhase()
    //     )
    //     {
    //         return;
    //     }


    //     // =====================================================
    //     // CHECK MOVEMENT INPUT
    //     // =====================================================

    //     float horizontal =
    //         Input.GetAxisRaw(
    //             "Horizontal"
    //         );


    //     // =====================================================
    //     // PLAYER IS TRYING TO MOVE
    //     // =====================================================

    //     bool tryingToMove =
    //         Mathf.Abs(horizontal) >
    //         movementThreshold;


    //     // =====================================================
    //     // START SHAKE
    //     // =====================================================

    //     if (
    //         tryingToMove &&
    //         !wasTryingToMove
    //     )
    //     {

    //     }


    //     wasTryingToMove =
    //         tryingToMove;


}