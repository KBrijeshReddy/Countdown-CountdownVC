using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public LevelManager levelManager;

    public TMP_Text timerText;

    private void Update()
    {
        if (
            levelManager == null ||
            timerText == null
        )
        {
            return;
        }

        float time =
            levelManager.RemainingTime;

        // Convert to whole seconds.
        int seconds =
            Mathf.CeilToInt(time);

        timerText.text =
            seconds.ToString();
    }
}