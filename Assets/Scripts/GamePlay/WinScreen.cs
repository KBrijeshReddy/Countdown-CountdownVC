using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text totalTimeText;
    [SerializeField] private TMP_Text thankYouText;

    [Header("Message")]
    [SerializeField, TextArea] private string thankYouMessage = "Thanks for playing!";

    private void Start()
    {
        DisplayTotalTime();
        DisplayThankYouMessage();
    }

    private void DisplayTotalTime()
    {
        if (totalTimeText == null)
            return;

        float totalSeconds = GameSession.Instance != null ? GameSession.Instance.TotalTimeSpent : 0f;

        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        totalTimeText.text = $"Total Time Spent: {minutes:00}:{seconds:00}";
    }

    private void DisplayThankYouMessage()
    {
        if (thankYouText != null)
            thankYouText.text = thankYouMessage;
    }

    public void MainMenu(){
        SceneManager.LoadScene(0);
    }

    public void QuitButton(){
        Application.Quit();
    }
}