using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    public void StartGame()
    {
        if (levelManager == null)
        {
            Debug.LogError($"{name}: LevelManager is missing.");
            return;
        }

        levelManager.StartPuzzle();
        gameObject.SetActive(false);
    }
}