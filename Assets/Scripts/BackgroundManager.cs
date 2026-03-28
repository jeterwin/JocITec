using UnityEngine;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer backgroundSprite;
    public List<Transform> clouds;

    [Header("Movement Settings")]
    public float baseWindSpeed = 1.5f;
    public float leftBound = -25f;
    public float rightBound = 25f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Positioning & Layers")]
    public float zOffset = 20f;
    public string sortingLayerName = "Background";

    [Header("Color Fading")]
    public float colorFadeSpeed = 2f;
    private Color targetBgColor = Color.white;
    private Color targetCloudColor = Color.white;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        if (backgroundSprite != null)
        {
            backgroundSprite.sortingLayerName = sortingLayerName;
            backgroundSprite.sortingOrder = -10;
            targetBgColor = backgroundSprite.color;
        }

        foreach (Transform cloud in clouds)
        {
            SetupCloud(cloud, true);
        }

        ScaleToFillScreen();
    }

    void LateUpdate()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        // 1. Lock to Camera
        transform.position = new Vector3(mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z + zOffset);

        // 2. Handle Scaling
        ScaleToFillScreen();

        // 3. Smooth Color Fading
        if (backgroundSprite != null)
        {
            backgroundSprite.color = Color.Lerp(backgroundSprite.color, targetBgColor, Time.deltaTime * colorFadeSpeed);
        }

        // 4. Move Clouds & Fade Colors
        foreach (Transform cloud in clouds)
        {
            if (cloud == null) continue;

            // Parallax Movement
            float sizeFactor = cloud.localScale.x;
            cloud.localPosition += Vector3.left * (baseWindSpeed * sizeFactor * Time.deltaTime);

            // Cloud Color Fade
            SpriteRenderer sr = cloud.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.Lerp(sr.color, targetCloudColor, Time.deltaTime * colorFadeSpeed);
            }

            // Loop Logic
            if (cloud.localPosition.x < leftBound)
            {
                SetupCloud(cloud, false);
                cloud.localPosition = new Vector3(rightBound, cloud.localPosition.y, cloud.localPosition.z);
            }
        }
    }

    public void SetTargetColors(Color bg, Color cloud)
    {
        targetBgColor = bg;
        targetCloudColor = cloud;
    }

    void SetupCloud(Transform cloud, bool randomizeX)
    {
        SpriteRenderer sr = cloud.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = -5;
            sr.color = targetCloudColor; // Ensure new clouds match current theme
        }

        float x = randomizeX ? Random.Range(leftBound, rightBound) : rightBound;
        float y = Random.Range(minY, maxY);
        float s = Random.Range(0.6f, 1.4f);

        cloud.localScale = new Vector3(s, s, 1);
        cloud.localPosition = new Vector3(x, y, -0.1f);
    }

    void ScaleToFillScreen()
    {
        if (backgroundSprite == null || mainCam == null) return;
        float height = mainCam.orthographicSize * 2f;
        float width = height * mainCam.aspect;
        float sW = backgroundSprite.sprite.bounds.size.x;
        float sH = backgroundSprite.sprite.bounds.size.y;
        backgroundSprite.transform.localScale = new Vector3((width / sW) * 1.2f, (height / sH) * 1.2f, 1);
    }
}