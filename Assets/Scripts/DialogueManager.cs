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

    [SerializeField] private float trembleAmount = 5f;
    [SerializeField] private float trembleSpeed = 20f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    private Queue<DialogueEntry> dialogueQueue;
    private Coroutine typingCoroutine;
    private CharacterFollower companionToSpawn;
    private bool isDialogueActive;

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

        if (isDialogueActive)
        {
            ApplyTrembleEffect();
        }
    }

    public void StartDialogue(List<DialogueEntry> dialogueList, CharacterFollower companionToSpawn)
    {
        PauseMenu.Instance.CanPause = false;
        CharacterMovement.Instance.CanMove = false;
        this.companionToSpawn = companionToSpawn;
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        dialogueQueue.Clear();

        foreach (DialogueEntry entry in dialogueList)
        {
            dialogueQueue.Enqueue(entry);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueEntry currentEntry = dialogueQueue.Dequeue();

        nameText.text = currentEntry.characterName;
        iconImage.sprite = currentEntry.characterIcon;

        typingCoroutine = StartCoroutine(TypeSentence(currentEntry.sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = sentence;
        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i <= sentence.Length; i++)
        {
            dialogueText.maxVisibleCharacters = i;

            if (i > 0 && i < sentence.Length && !char.IsWhiteSpace(sentence[i - 1]))
            {
                PlayTypingSound();
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }

    private void PlayTypingSound()
    {
        if (audioSource == null || typingSound == null || !isDialogueActive) return;

        audioSource.clip = typingSound;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    private void ApplyTrembleEffect()
    {
        dialogueText.ForceMeshUpdate();
        TMP_TextInfo textInfo = dialogueText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float offset = Mathf.Sin(Time.time * trembleSpeed + i) * trembleAmount;
            Vector3 translation = new Vector3(0, offset, 0);

            vertices[vertexIndex + 0] += translation;
            vertices[vertexIndex + 1] += translation;
            vertices[vertexIndex + 2] += translation;
            vertices[vertexIndex + 3] += translation;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            dialogueText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (companionToSpawn)
        {
            CharacterUnlockerManager.Instance.UnlockCharacter(companionToSpawn);
        }

        PauseMenu.Instance.CanPause = true;
        CharacterMovement.Instance.CanMove = true;
        dialoguePanel.SetActive(false);
    }
}