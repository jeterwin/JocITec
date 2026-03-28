using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public float speed = 3f;
    public float leftBound = -15f;
    public float rightBound = 15f;

    void Update()
    {
        // Moves the cloud left relative to the BackgroundManager
        transform.localPosition += Vector3.left * speed * Time.deltaTime;

        // If it goes past the left edge, warp it to the right edge
        if (transform.localPosition.x < leftBound)
        {
            transform.localPosition = new Vector3(rightBound, transform.localPosition.y, transform.localPosition.z);
        }
    }
}