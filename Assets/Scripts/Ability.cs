using UnityEngine;

public class Ability : MonoBehaviour
{
    public string AbilityName
    {
        get => abilityName;
        set => abilityName = value;
    }

    [SerializeField] private string abilityName;
}
