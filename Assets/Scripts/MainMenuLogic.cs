using UnityEngine;

public class MainMenuLogic : MonoBehaviour
{
    public Canvas Canvas;
    public Canvas SettingsCanvas;
    public void SwapToSettings()
    {
        Canvas.gameObject.SetActive(false);
        SettingsCanvas.gameObject.SetActive(true);
    }

    public void SwapToMainMenu()
    {
        SettingsCanvas.gameObject.SetActive(false);
        Canvas.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
