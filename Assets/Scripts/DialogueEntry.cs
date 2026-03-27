using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public string characterName;
    public Sprite characterIcon;
    [TextArea(3, 10)]
    public string sentence;
}