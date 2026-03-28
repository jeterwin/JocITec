using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private float typingSpeed = 0.05f;

    private Queue<DialogueEntry> dialogueQueue;
    private Coroutine typingCoroutine;
    private CharacterFollower companionToSpawn;

    void Awake()
    {
        dialogueQueue = new Queue<DialogueEntry>();
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && dialoguePanel.activeSelf)
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(List<DialogueEntry> dialogueList, 
        CharacterFollower companionToSpawn)
    {
        PauseMenu.Instance.CanPause = false;
        CharacterMovement.Instance.CanMove = false;
        this.companionToSpawn = companionToSpawn;
        dialoguePanel.SetActive(true);
        dialogueQueue.Clear();

        foreach (DialogueEntry entry in dialogueList)
        {
            dialogueQueue.Enqueue(entry);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueEntry currentEntry = dialogueQueue.Dequeue();

        nameText.text = currentEntry.characterName;
        iconImage.sprite = currentEntry.characterIcon;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeSentence(currentEntry.sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        if(companionToSpawn)
        {
            CharacterUnlockerManager.Instance.UnlockCharacter(companionToSpawn);
        }
        PauseMenu.Instance.CanPause = true;
        CharacterMovement.Instance.CanMove = true;
        dialoguePanel.SetActive(false);
    }
}