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
    private void Start(){
        button.gameObject.SetActive(false);
        SubscribeToLevelManager();
    }
    // brijesh anna sexy

    private void OnDisable()
    {
        if (levelManager != null)
            levelManager.PuzzlePhaseStarted -= EnableButton;
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;

        if (levelManager == null)
            return;

        levelManager.PuzzlePhaseStarted -= EnableButton;
        levelManager.PuzzlePhaseStarted += EnableButton;
    }

    private void EnableButton(){
        button.gameObject.SetActive(true);
        button.interactable = true;
    }        

    private void HandleClick() => levelManager?.RestartLevel();
}