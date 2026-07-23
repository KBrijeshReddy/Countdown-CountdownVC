using UnityEngine;

public class BuyableObject : MonoBehaviour
{
    [Header("Cost")]
    [SerializeField] private float cost = 10f;

    [Header("References")]
    [SerializeField] private LevelManager levelManager;

    private bool isPurchased = false;

    public float Cost
    {
        get
        {
            return cost;
        }
    }

    public bool IsPurchased
    {
        get
        {
            return isPurchased;
        }
    }

    private void Awake()
    {
        // Make sure every object starts
        // in the Buy Area as unpurchased.
        isPurchased = false;
    }

    // =========================================================
    // BUY
    // =========================================================

    public bool TryBuy()
    {
        // Already purchased.
        if (isPurchased)
        {
            return true;
        }

        // Make sure we have LevelManager.
        if (levelManager == null)
        {
            Debug.LogError(
                gameObject.name +
                ": LevelManager is missing!"
            );

            return false;
        }

        // Buying is only possible
        // during the planning phase.
        if (
            !levelManager.IsBuyingPhase()
        )
        {
            return false;
        }

        // Check if we have enough time.
        if (
            levelManager.RemainingTime <
            cost
        )
        {
            Debug.Log(
                "Not enough time to buy " +
                gameObject.name
            );

            return false;
        }

        // Deduct cost.
        bool spent =
            levelManager.SpendTime(
                cost
            );

        if (!spent)
        {
            return false;
        }

        // Mark as purchased.
        isPurchased = true;

        Debug.Log(
            "Bought " +
            gameObject.name +
            " for " +
            cost +
            " seconds."
        );

        return true;
    }

    // =========================================================
    // SELL
    // =========================================================

    public bool Sell()
    {
        // Can't sell something
        // that wasn't purchased.
        if (!isPurchased)
        {
            return false;
        }

        if (levelManager == null)
        {
            Debug.LogError(
                gameObject.name +
                ": LevelManager is missing!"
            );

            return false;
        }

        // Only sell during buying phase.
        if (
            !levelManager.IsBuyingPhase()
        )
        {
            return false;
        }

        // Refund the cost.
        levelManager.AddTime(
            cost
        );

        // Mark as not purchased.
        isPurchased = false;

        Debug.Log(
            "Sold " +
            gameObject.name +
            " for " +
            cost +
            " seconds."
        );

        return true;
    }
}