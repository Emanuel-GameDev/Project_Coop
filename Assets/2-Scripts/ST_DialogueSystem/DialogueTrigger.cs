using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private Dialogue dialogueOnTrigger;
    [SerializeField] UnityEvent onDialogueEndEvent;
    private bool alreadyTriggered = true;
    [SerializeField] static string DIALOGUE_TRIGGER_SAVE_NAME = "TriggerDialogue";

    private void Start()
    {
        SaveManager.Instance.LoadData();
        bool alreadyTriggeredSave = SaveManager.Instance.LoadSetting<bool>(DIALOGUE_TRIGGER_SAVE_NAME);

        if (alreadyTriggeredSave)
        {
            alreadyTriggered = true;
            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            alreadyTriggered = false;
            GetComponent<Collider2D>().enabled = true;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerCharacter>(out PlayerCharacter player) && !alreadyTriggered)
        {
            alreadyTriggered = true;
            SetDialogue();
            SaveManager.Instance.SaveSetting(DIALOGUE_TRIGGER_SAVE_NAME, alreadyTriggered);
        }
    }

    private void SetDialogue()
    {
        dialogueBox.SetDialogue(dialogueOnTrigger);
        dialogueBox.RemoveAllDialogueEnd();
        dialogueBox.AddDialogueEnd(onDialogueEndEvent);
        dialogueBox.StartDialogue();
    }

}
