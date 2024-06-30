using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private Dialogue dialogueOnTrigger;
    [SerializeField] UnityEvent onDialogueEndEvent;
    [SerializeField] bool MustBeSaved = true;
    private bool alreadyTriggered = true;
    [SerializeField] string dialogueTriggerSaveName = "TriggerDialogue";

    private void Start()
    {
        if (!MustBeSaved)
        {
            alreadyTriggered = false;
            GetComponent<Collider2D>().enabled = true;
            return;
        }
           
        SaveManager.Instance.LoadData();
        bool alreadyTriggeredSave = SaveManager.Instance.LoadSetting<bool>(dialogueTriggerSaveName);

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
            if(MustBeSaved)
                SaveManager.Instance.SaveSetting(dialogueTriggerSaveName, alreadyTriggered);
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
