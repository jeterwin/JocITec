using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class AsyncLoader : MonoBehaviour
{
    public GameObject LoadingScreen;
    public TextMeshProUGUI LoadingText;
    public float BounceSpeed = 5f;
    public float BounceAmplitude = 10f;

    private Vector3 _textInitialPosition;
    private bool _isLoading = false;

    private void Start()
    {
        _textInitialPosition = LoadingText.transform.localPosition;
        LoadingScreen.SetActive(false);
    }

    private void Update()
    {
        if (_isLoading)
        {
            float newY = _textInitialPosition.y + Mathf.Sin(Time.time * BounceSpeed) * BounceAmplitude;
            LoadingText.transform.localPosition = new Vector3(_textInitialPosition.x, newY, _textInitialPosition.z);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }    

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        _isLoading = true;
        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                LoadingText.text = "Press SPACE to continue";

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}