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

    private void Awake()
    {
        SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.SviluppartyInteractions);
        if (sceneSetting != null)
        {
            interacted = sceneSetting.GetBoolValue(SaveDataStrings.SVILUPPARTY_END_DUMPY_INTERACTED);
        }
    }

    private void FirstInteract()
    {
        onFirstInteract.Invoke();
        interacted = true;

        SceneSetting sceneSetting = new(SceneSaveSettings.SviluppartyInteractions);
        sceneSetting.AddBoolValue(SaveDataStrings.SVILUPPARTY_END_DUMPY_INTERACTED, interacted);
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
