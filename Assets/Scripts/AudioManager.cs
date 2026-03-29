using UnityEngine;
using System.Collections;

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

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float fadeDuration = 1.2f;

    private Coroutine _musicCoroutine;
    private float _originalMusicVolume;

    private void Awake()
    {
        Instance = this;
        _originalMusicVolume = musicSource.volume;
    }

    public void ChangeBackgroundMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip) return;

        if (_musicCoroutine != null) StopCoroutine(_musicCoroutine);
        _musicCoroutine = StartCoroutine(CrossfadeMusic(newClip));
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(_originalMusicVolume, 0, timer / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, _originalMusicVolume, timer / fadeDuration);
            yield return null;
        }

        musicSource.volume = _originalMusicVolume;
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