using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject audioPanel;
    public GameObject controlsPanel;

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        OpenAudioTab(); // This is for audio only beta version
    }

    public void CloseOptions()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenAudioTab()
    {
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void OpenControlsTab()
    {
        audioPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }
}