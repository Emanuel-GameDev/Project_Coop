using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private Dialogue dialogueOnTrigger;
    [SerializeField] UnityEvent onDialogueEndEvent;
    private bool alreadyTriggered = true;
    [SerializeField] string settingSaveName = "FirstTriggerChallenge";

    private void Start()
    {
        SaveManager.Instance.LoadData();
        SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.DialogueTrigger);

        if (sceneSetting == null)
        {
            sceneSetting = new(SceneSaveSettings.DialogueTrigger);
            alreadyTriggered = false;
            GetComponent<BoxCollider2D>().enabled = !alreadyTriggered;
            sceneSetting.bools.Add(new(settingSaveName, alreadyTriggered));
        }
        else
        {
            if (sceneSetting.GetBoolValue(settingSaveName))
            {
                alreadyTriggered = true;
                GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                alreadyTriggered = false;
                GetComponent<BoxCollider2D>().enabled = true;
            }

        }

        SaveManager.Instance.SaveSceneData(sceneSetting);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerCharacter>(out PlayerCharacter player) && !alreadyTriggered)
        {
            SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.DialogueTrigger);

            alreadyTriggered = true;
            SetDialogue();

            sceneSetting.AddBoolValue(settingSaveName, alreadyTriggered);
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
