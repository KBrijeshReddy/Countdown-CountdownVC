using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        if (levelManager == null || timerText == null)
            return;

        timerText.text = Mathf.CeilToInt(levelManager.RemainingTime).ToString();
    }
}