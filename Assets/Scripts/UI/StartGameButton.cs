using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject text;

    public void StartGame()
    {
        if (levelManager == null)
        {
            Debug.LogError($"{name}: LevelManager is missing.");
            return;
        }

        levelManager.StartPuzzle();

        gameObject.GetComponent<Button>().image.color = new Color(0f, 0f, 0f, 0f);
        text.SetActive(false);
        
        // gameObject.SetActive(false);
    }
}