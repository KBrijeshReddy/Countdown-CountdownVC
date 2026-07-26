using UnityEngine;

public class Purchasable : MonoBehaviour
{
    [SerializeField] private float cost = 10f;

    [Tooltip("If true, the puzzle can't start until this object has been placed on the grid at least once.")]
    [SerializeField] private bool isRequiredForLevel = false;

    public float Cost => cost;
    public bool IsRequiredForLevel => isRequiredForLevel;
    public bool IsPurchased { get; private set; }

    public bool TryBuy()
    {
        if (IsPurchased) return true;
        if (LevelManager.Instance == null || !LevelManager.Instance.IsBuyingPhase()) return false;
        if (!LevelManager.Instance.SpendTime(cost)) return false;

        IsPurchased = true;
        return true;
    }

    public bool Sell()
    {
        if (!IsPurchased) return false;
        if (LevelManager.Instance == null || !LevelManager.Instance.IsBuyingPhase()) return false;

        LevelManager.Instance.AddTime(cost);
        IsPurchased = false;
        return true;
    }
}