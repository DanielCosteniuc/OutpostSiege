using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    private Queue<string> sentences;
    private Coroutine typingRoutine;
    private bool isTyping = false;
    private string currentSentence;

    void Awake()
    {
        sentences = new Queue<string>();
        continueButton.onClick.AddListener(DisplayNextSentence);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueBox.SetActive(true);
        animator.SetBool("isOpen", true);
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // Skip typing animation if currently typing
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            isTyping = false;
            return;
        }

        // End dialogue if queue is empty
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Get next sentence and start typing
        currentSentence = sentences.Dequeue();
        typingRoutine = StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        StartCoroutine(DisableAfterAnimation());
    }

    IEnumerator DisableAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        dialogueBox.SetActive(false);
    }

    // Public method to check if dialogue is active
    public bool IsDialogueActive()
    {
        return dialogueBox.activeSelf;
    }
}