using UnityEngine;
using Unity.Cinemachine;

public class RoomCameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera roomCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            roomCamera.Priority = 20;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            roomCamera.Priority = 10;
        }
    }
}