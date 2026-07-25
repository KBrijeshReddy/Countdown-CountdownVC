using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject text;

    private Button button;
    private Color originalColor;
    private LevelManager subscribedLevelManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalColor = button.image.color;
    }

    private void OnEnable() => SubscribeToLevelManager();
    private void Start() => SubscribeToLevelManager();

    private void OnDisable()
    {
        if (subscribedLevelManager != null)
            subscribedLevelManager.LevelRestarted -= ShowButton;
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;

        subscribedLevelManager = levelManager;

        if (subscribedLevelManager == null)
            return;

        subscribedLevelManager.LevelRestarted -= ShowButton;
        subscribedLevelManager.LevelRestarted += ShowButton;
    }

    public void StartGame()
    {
        if (levelManager == null)
        {
            Debug.LogError($"{name}: LevelManager is missing.");
            return;
        }

        levelManager.StartPuzzle();
        HideButton();
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