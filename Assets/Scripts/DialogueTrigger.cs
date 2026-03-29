using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager manager;
    [SerializeField] private CharacterFollower companionToSpawn;
    [SerializeField] private List<DialogueEntry> conversation;

    [SerializeField] private bool shouldDestroy = true;

    public void TriggerDialogue()
    {
        manager.StartDialogue(conversation, companionToSpawn);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerDialogue();

        if(shouldDestroy)
            Destroy(gameObject);
    }
}