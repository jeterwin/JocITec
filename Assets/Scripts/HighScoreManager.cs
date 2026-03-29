using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HighScoreManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Trophy UI Elements")]
    [SerializeField] private GameObject bronzeTrophy;
    [SerializeField] private GameObject silverTrophy;
    [SerializeField] private GameObject goldTrophy;

    void Start()
    {
        UpdateHighScoreDisplay();
    }

    public void UpdateHighScoreDisplay()
    {
        if (highScoreText == null) return;

        // Retrieve the best time. Default to a high number if no score exists.
        float bestTime = PlayerPrefs.GetFloat("BestTime", 999999f);

        if (bestTime < 999999f)
        {
            highScoreText.text = "Best Time: " + GameTimer.FormatTime(bestTime);

            // Handle Trophy Visibility based on seconds:
            // Gold: 4:00 (240s) | Silver: 4:30 (270s) | Bronze: 5:00 (300s)
            if (goldTrophy != null) goldTrophy.SetActive(bestTime <= 240f);
            if (silverTrophy != null) silverTrophy.SetActive(bestTime <= 270f);
            if (bronzeTrophy != null) bronzeTrophy.SetActive(bestTime <= 300f);
        }
        else
        {
            highScoreText.text = "Best Time: --:--";
            HideAllTrophies();
        }
    }

    private void HideAllTrophies()
    {
        if (bronzeTrophy != null) bronzeTrophy.SetActive(false);
        if (silverTrophy != null) silverTrophy.SetActive(false);
        if (goldTrophy != null) goldTrophy.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("BestTime");
        HideAllTrophies();
        UpdateHighScoreDisplay();
    }
}