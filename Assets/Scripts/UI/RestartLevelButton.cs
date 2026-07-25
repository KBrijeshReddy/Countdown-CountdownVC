using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RestartLevelButton : MonoBehaviour
{
    private Button button;
    private LevelManager levelManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = false;
        button.onClick.AddListener(HandleClick);
    }

    private void OnEnable() => SubscribeToLevelManager();
    private void Start() => SubscribeToLevelManager();

    private void OnDisable()
    {
        if (levelManager != null)
        {
            levelManager.PuzzlePhaseStarted -= EnableButton;
            levelManager.LevelRestarted -= DisableButton;
        }
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;

        if (levelManager == null)
            return;

        levelManager.PuzzlePhaseStarted -= EnableButton;
        levelManager.PuzzlePhaseStarted += EnableButton;

        levelManager.LevelRestarted -= DisableButton;
        levelManager.LevelRestarted += DisableButton;
    }

    private void EnableButton() => button.interactable = true;
    private void DisableButton() => button.interactable = false;

    private void HandleClick() => levelManager?.RestartLevel();
}