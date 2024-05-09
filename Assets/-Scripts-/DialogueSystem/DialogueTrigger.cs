using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private Dialogue dialogueOnTrigger;
    [SerializeField] UnityEvent onDialogueEndEvent;
    private bool canTrigger = true;
    [SerializeField] string settingSaveName = "FirstTriggerChallenge";

    private void Start()
    {
        SaveManager.Instance.LoadData();
        SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.DialogueTrigger);

        if (sceneSetting == null)
        {
            sceneSetting = new(SceneSaveSettings.DialogueTrigger);
            canTrigger = true;
            sceneSetting.bools.Add(new(settingSaveName, !canTrigger));
        }
        else
        {
            if (sceneSetting.GetBoolValue(settingSaveName))
            {
                canTrigger = false;
                GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                canTrigger = true;
                GetComponent<BoxCollider2D>().enabled = false;
            }

        }

        SaveManager.Instance.SaveSceneData(sceneSetting);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerCharacter>(out PlayerCharacter player) && canTrigger)
        {
            SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.DialogueTrigger);

            canTrigger = false;
            SetDialogue();

            sceneSetting.AddBoolValue(settingSaveName, !canTrigger);
            SaveManager.Instance.SaveSceneData(sceneSetting);
            

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
