using UnityEngine;

public class SlowMotionController : MonoBehaviour
{
    [SerializeField] private GameObject slowMoPanel;
    [SerializeField] private float slowTimeScale = 0.25f;

    private string currentSelection = "None";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartSlowMotion();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            StopSlowMotion();
        }
    }

    public void SetCurrentSelection(string name)
    {
        currentSelection = name;
    }

    void StartSlowMotion()
    {
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        slowMoPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void StopSlowMotion()
    {
        Debug.Log("Selected: " + currentSelection);

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        slowMoPanel.SetActive(false);

        currentSelection = "None";
    }
}