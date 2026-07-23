using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    public LevelManager levelManager;

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
        gameObject.SetActive(
            false
        );
    }
}