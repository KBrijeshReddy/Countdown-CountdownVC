using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayMainMenuButton(){
        SceneManager.LoadScene(1);
    }
    
    public void QuitMainMenuButton(){
        Application.Quit();
    }
}
