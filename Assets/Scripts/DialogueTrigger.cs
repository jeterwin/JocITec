using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager manager;
    [SerializeField] private CharacterFollower companionToSpawn;
    [SerializeField] private List<DialogueEntry> conversation;

    public void TriggerDialogue()
    {
        manager.StartDialogue(conversation, companionToSpawn);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerDialogue();
        Destroy(gameObject);
    }
}