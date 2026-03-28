using System.Collections.Generic;
using UnityEngine;

public class CharacterUnlockerManager : MonoBehaviour
{
    public static CharacterUnlockerManager Instance { get; private set; }

    [SerializeField] private ParticleSystem swapSFX;
    [SerializeField] private PlayerAbilities playerAbilities;
    [SerializeField] private SpriteRenderer playerSR;

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
        CharacterFollower fromFollower = activeFollowers.Find(f => f.AbilityName == lastSelection);
        CharacterFollower toFollower = activeFollowers.Find(f => f.AbilityName == newAbility);

        if (toFollower == null) return;

        if (swapSFX != null) swapSFX.Play();

        if (fromFollower != null)
        {
            SpriteRenderer fromSR = fromFollower.GetComponent<SpriteRenderer>();
            Color returnColor = playerSR.color;
            playerSR.color = fromSR.color;
            fromSR.color = returnColor;
        }

        SpriteRenderer toSR = toFollower.GetComponent<SpriteRenderer>();
        Color takeColor = toSR.color;
        toSR.color = playerSR.color;
        playerSR.color = takeColor;
    }

    public void UnlockCharacter(CharacterFollower companionPrefab)
    {
        Transform playerTransform = CharacterMovement.Instance.transform;

        GameObject newCompanion = Instantiate(companionPrefab.gameObject,
            playerTransform.position, Quaternion.identity);

        CharacterFollower charFollower = newCompanion.GetComponent<CharacterFollower>();
        charFollower.SetTarget(playerTransform);

        if (!activeFollowers.Contains(charFollower))
        {
            activeFollowers.Add(charFollower);
        }

        if (playerAbilities != null)
        {
            playerAbilities.UnlockAbility(companionPrefab.AbilityName);
        }
    }
}