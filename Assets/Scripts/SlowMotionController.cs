using UnityEngine;

public class SlowMotionController : MonoBehaviour
{
    [SerializeField] private GameObject slowMoPanel;
    [SerializeField] private float slowTimeScale = 0.25f;

    void Update()
    {
        if(PlayerHealth.Instance == null) return;

        if (PlayerHealth.Instance.IsDead) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartSlowMotion();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            StopSlowMotion();
        }
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
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        slowMoPanel.SetActive(false);
    }
}