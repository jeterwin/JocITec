using UnityEngine;

public class CharacterUnlockerManager : MonoBehaviour
{
    public static CharacterUnlockerManager Instance { get; private set; }

    [SerializeField] private string abilityName;
    [SerializeField] private SlowMotionController slowMo;

    private void Awake()
    {
        Instance = this;
    }

    public void UnlockCharacter(CharacterFollower companionPrefab)
    {
        Vector3 spawnDirection = -transform.right;
        Vector3 spawnPos = transform.position + (spawnDirection * 0.5f);

        GameObject newCompanion = Instantiate(companionPrefab.gameObject, spawnPos, Quaternion.identity);

        companionPrefab.SetTarget(transform);

        if (slowMo != null)
        {
            slowMo.UnlockAbility(companionPrefab.AbilityName);
            Debug.Log(abilityName + " has been unlocked!");
        }
    }
}