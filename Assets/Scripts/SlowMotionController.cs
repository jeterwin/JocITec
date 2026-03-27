using UnityEngine;
using System.Collections.Generic;

public class SlowMotionController : MonoBehaviour
{
    [SerializeField] private GameObject slowMoPanel;
    [SerializeField] private float slowTimeScale = 0.25f;

    private string currentSelection = "None";
    private List<string> unlockedAbilities = new List<string>();

    public string CurrentSelection
    {
        get => currentSelection;
        set => currentSelection = value;
    }

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

    public void UnlockAbility(string abilityName)
    {
        if (!unlockedAbilities.Contains(abilityName))
        {
            unlockedAbilities.Add(abilityName);
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
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        slowMoPanel.SetActive(false);
    }
}