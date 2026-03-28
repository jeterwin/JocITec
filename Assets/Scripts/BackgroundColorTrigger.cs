using UnityEngine;

public class BackgroundColorTrigger : MonoBehaviour
{
    public BackgroundManager mgr;

    [Header("Room Theme (When Toggled On)")]
    public Color roomBgColor = Color.gray;
    public Color roomCloudColor = Color.red;

    [Header("Default Theme (When Toggled Off)")]
    public Color defaultBgColor = Color.white;
    public Color defaultCloudColor = Color.white;

    private bool isRoomThemeActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && mgr != null)
        {
            // Flip the state
            isRoomThemeActive = !isRoomThemeActive;

            if (isRoomThemeActive)
            {
                // Switch to the custom room colors
                mgr.SetTargetColors(roomBgColor, roomCloudColor);
            }
            else
            {
                // Switch back to the default white/white
                mgr.SetTargetColors(defaultBgColor, defaultCloudColor);
            }
        }
    }
}