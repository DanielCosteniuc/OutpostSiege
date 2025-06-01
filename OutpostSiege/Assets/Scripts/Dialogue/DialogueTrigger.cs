using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;

    void Start()
    {
        // Automatically start dialogue when the scene loads
        FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
    }
}