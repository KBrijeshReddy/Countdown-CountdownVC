using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    public LevelManager levelManager;

    [SerializeField] private Button startButton;

    public void StartGame()
    {
        if (
            levelManager == null
        )
        {
            Debug.LogError(
                "StartGameButton: LevelManager is missing!"
            );

            return;
        }

        levelManager.StartPuzzle();
        

        // Disable the Start button.
        startButton.interactable = false;
        
    }
}