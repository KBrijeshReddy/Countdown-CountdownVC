using UnityEngine;

public class ButtonPlayerDetection : MonoBehaviour
{
    [Header("Button")]
    public ButtonObject buttonObject;

    // =========================================================
    // PLAYER ENTERS DETECTION AREA
    // =========================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (
            buttonObject == null
        )
        {
            return;
        }

        buttonObject.PlayerEnteredDetection(
            other
        );
    }
}