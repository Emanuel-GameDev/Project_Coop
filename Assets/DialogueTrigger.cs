using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private Dialogue dialogueOnTrigger;
    private bool canTrigger = true;
    private string settingSaveName = "FirstTriggerChallenge";

    private void Start()
    {
        SaveManager.Instance.LoadData();
        SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.DialogueTrigger);

        if (sceneSetting == null)
        {
            sceneSetting = new(SceneSaveSettings.DialogueTrigger);
            sceneSetting.bools.Add(new("FirstTriggerChallenge", canTrigger));
        }
        else
        {
            SavingBoolValue canTriggerBool = sceneSetting.bools.Find(x => x.valueName == settingSaveName);

            if (canTriggerBool != null)
            {
                canTrigger = canTriggerBool.value;
            }
            else
            {
                sceneSetting.bools.Add(new(settingSaveName, canTrigger));
            }
        }

        if(canTrigger)
        {
            GetComponent<BoxCollider2D>().enabled = true;
            canTrigger = true;
            SaveManager.Instance.SaveSceneData(sceneSetting);
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = false;
            canTrigger = false;
            SaveManager.Instance.SaveSceneData(sceneSetting);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerCharacter>(out PlayerCharacter player) && canTrigger)
        {
            canTrigger = false;
            SetDialogue();
            
        }
    }

    private void SetDialogue()
    {
        dialogueBox.SetDialogue(dialogueOnTrigger);
        dialogueBox.RemoveAllDialogueEnd();
        dialogueBox.StartDialogue();
    }

}
