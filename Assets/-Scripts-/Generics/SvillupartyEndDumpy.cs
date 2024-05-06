using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SvillupartyEndDumpy : MonoBehaviour
{
    [SerializeField]
    UnityEvent onFirstInteract;

    [SerializeField]
    UnityEvent onNormalInteract;

    [SerializeField]
    UnityEvent onLastInteract;

    bool interacted = false;
    bool gameComplete = false;

    static string InteractedKey = "SvillupartyEndDumpyInteracted";

    private void Awake()
    {
        SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.SviluppartyInteractions);
        if (sceneSetting != null)
        {
            SavingBoolValue savingBoolValue = sceneSetting.bools.Find(x => x.valueName == InteractedKey);
            if(savingBoolValue != null)
                interacted = savingBoolValue.value;
        }
    }

    private void FirstInteract()
    {
        onFirstInteract.Invoke();
        interacted = true;

        SceneSetting sceneSetting = new(SceneSaveSettings.SviluppartyInteractions);
        sceneSetting.bools.Add(new(InteractedKey, interacted));
        SaveManager.Instance.SaveSceneData(sceneSetting);
    }

    private void NormalInteract() 
    {  
        onNormalInteract.Invoke(); 
    }

    private void LastInteract() 
    { 
        onLastInteract.Invoke(); 
    }

    public void Interact()
    {
        if(gameComplete) 
            LastInteract();
        else
        {
            if (!interacted) 
                FirstInteract();
            else 
                NormalInteract();
        }
    }
}
