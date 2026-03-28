using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource dashSource;
    [SerializeField] private AudioSource grappleSource;
    [SerializeField] private AudioSource coinGrapSource;
    [SerializeField] private AudioSource jumpSource;
    [SerializeField] private AudioSource wallJumpSource;
    [SerializeField] private AudioSource doubleJumpSource;
    [SerializeField] private AudioSource deathSource;

    private void Awake()
    {
        Instance = this;
    }

    public void PickedCoin()
    {
        coinGrapSource.Play();
    }

    public void PlayJump()
    {
        jumpSource.Play();
    }

    public void PlayWallJump()
    {
        wallJumpSource.Play();
    }

    public void DoubleJumpPlay()
    {
        doubleJumpSource.Play();
    }

    public void DashPlay()
    {
        dashSource.Play();
    }

    public void GrapplePlay()
    {
        grappleSource.Play();
    }

    public void DeathPlay()
    {
        deathSource.Play();
    }
}
