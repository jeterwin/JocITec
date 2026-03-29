using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUnlockerManager : MonoBehaviour
{
    public static CharacterUnlockerManager Instance { get; private set; }

    [SerializeField] private ParticleSystem swapSFX;
    [SerializeField] private PlayerAbilities playerAbilities;
    [SerializeField] private Animator UIanim;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private Image heroImage;
    [SerializeField] private SpriteRenderer playerSR;
    [SerializeField] private AudioSource audioSource;

    private List<CharacterFollower> activeFollowers = new List<CharacterFollower>();
    public List<CharacterFollower> ActiveFollowers => activeFollowers;

    private string lastSelection;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (playerAbilities != null)
        {
            lastSelection = playerAbilities.CurrentSelection;

            CharacterFollower[] existing = FindObjectsByType<CharacterFollower>(FindObjectsSortMode.None);
            foreach (var f in existing)
            {
                if (!activeFollowers.Contains(f)) activeFollowers.Add(f);
            }

            if (lastSelection == "Speed")
            {
                playerSR.color = Color.white;
            }
        }
    }

    private void Update()
    {
        if (playerAbilities == null) return;

        if (playerAbilities.CurrentSelection != lastSelection)
        {
            HandleVisualSwap(playerAbilities.CurrentSelection);
            lastSelection = playerAbilities.CurrentSelection;
        }
    }

    private void HandleVisualSwap(string newAbility)
    {
        if (swapSFX != null) swapSFX.Play();

        CharacterFollower fromFollower = activeFollowers.Find(f => f.AbilityName == lastSelection);

        if (newAbility == "Speed")
        {
            if (fromFollower != null)
            {
                fromFollower.GetComponent<SpriteRenderer>().color = playerSR.color;
            }
            playerSR.color = Color.white;
            return;
        }

        CharacterFollower toFollower = activeFollowers.Find(f => f.AbilityName == newAbility);
        if (toFollower == null) return;

        SpriteRenderer toSR = toFollower.GetComponent<SpriteRenderer>();
        Color targetColor = toSR.color;

        if (fromFollower != null)
        {
            fromFollower.GetComponent<SpriteRenderer>().color = playerSR.color;
        }

        playerSR.color = targetColor;
        toSR.color = Color.white;
    }

    public void UnlockCharacter(CharacterFollower companionPrefab)
    {
        if (activeFollowers.Exists(f => f.AbilityName == companionPrefab.AbilityName)) return;

        UIanim.Play("AcquiredHero");
        heroName.text = companionPrefab.gameObject.name;
        heroImage.sprite = companionPrefab.CharSprite;
        Transform playerTransform = CharacterMovement.Instance.transform;

        GameObject newCompanion = Instantiate(companionPrefab.gameObject,
            playerTransform.position, Quaternion.identity);

        CharacterFollower charFollower = newCompanion.GetComponent<CharacterFollower>();
        charFollower.SetTarget(playerTransform);

        if (audioSource != null) audioSource.Play();

        activeFollowers.Add(charFollower);

        if (playerAbilities != null)
        {
            playerAbilities.UnlockAbility(companionPrefab.AbilityName);
        }
    }
}