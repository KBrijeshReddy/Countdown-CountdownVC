using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartGameButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject text;

    [Header("Missing Placement Warning")]
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private string warningMessage = "Place all buttons and doors before starting!";
    [SerializeField] private float warningDisplayDuration = 2f;

    [Header("Jitter Feedback")]
    [SerializeField] private float jitterDistance = 8f;
    [SerializeField] private float jitterDuration = 0.3f;
    [SerializeField] private int jitterCycles = 4;

    private Button button;
    private RectTransform rectTransform;
    private Color originalColor;
    private Vector2 originalAnchoredPosition;

    private LevelManager levelManager;
    private Coroutine jitterRoutine;
    private Coroutine warningRoutine;

    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        originalColor = button.image.color;
        originalAnchoredPosition = rectTransform.anchoredPosition;

        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    private void OnEnable() => SubscribeToLevelManager();
    private void Start() => SubscribeToLevelManager();

    private void OnDisable()
    {
        if (levelManager != null)
            levelManager.LevelRestarted -= ShowButton;
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == LevelManager.Instance)
            return;

        if (levelManager != null)
            levelManager.LevelRestarted -= ShowButton;

        levelManager = LevelManager.Instance;

        if (levelManager == null)
            return;

        levelManager.LevelRestarted += ShowButton;
    }

    public void StartGame()
    {
        if (levelManager == null)
        {
            Debug.LogError($"{name}: LevelManager.Instance is missing.");
            return;
        }

        if (!AreAllRequiredObjectsPlaced())
        {
            PlayBlockedFeedback();
            return;
        }

        levelManager.StartPuzzle();
        HideButton();
    }

    private bool AreAllRequiredObjectsPlaced()
    {
        Purchasable[] allPurchasables = FindObjectsByType<Purchasable>(FindObjectsSortMode.None);

        foreach (Purchasable purchasable in allPurchasables)
        {
            if (purchasable.IsRequiredForLevel && !purchasable.IsPurchased)
                return false;
        }

        return true;
    }

    private void PlayBlockedFeedback()
    {
        if (jitterRoutine != null) StopCoroutine(jitterRoutine);
        jitterRoutine = StartCoroutine(JitterRoutine());

        if (warningRoutine != null) StopCoroutine(warningRoutine);
        warningRoutine = StartCoroutine(ShowWarningRoutine());
    }

    private System.Collections.IEnumerator JitterRoutine()
    {
        float elapsed = 0f;

        while (elapsed < jitterDuration)
        {
            float t = elapsed / jitterDuration;
            float damping = 1f - t;

            float offsetX = Mathf.Sin(t * jitterCycles * Mathf.PI * 2f) * jitterDistance * damping;

            rectTransform.anchoredPosition = originalAnchoredPosition + new Vector2(offsetX, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalAnchoredPosition;
        jitterRoutine = null;
    }

    private System.Collections.IEnumerator ShowWarningRoutine()
    {
        if (warningText == null)
            yield break;

        warningText.text = warningMessage;
        warningText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(warningDisplayDuration);

        warningText.gameObject.SetActive(false);
        warningRoutine = null;
    }

    private void HideButton()
    {
        button.image.color = new Color(0f, 0f, 0f, 0f);

        if (text != null)
            text.SetActive(false);
    }

    private void ShowButton()
    {
        button.image.color = originalColor;

        if (text != null)
            text.SetActive(true);
    }
}