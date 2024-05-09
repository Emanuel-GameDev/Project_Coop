using UnityEngine;
using UnityEngine.Events;

public class SvillupartyEndDumpy : MonoBehaviour
{
    [SerializeField]
    UnityEvent onFirstInteract;

    [SerializeField]
    UnityEvent onNormalInteract;

    [SerializeField]
    UnityEvent<UnityEvent> onLastInteract;

    [SerializeField]
    UnityEvent eventToAddAtTheEndOfLastDialogue;

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
        onLastInteract.Invoke(eventToAddAtTheEndOfLastDialogue);
    }

    public void Interact()
    {
        GetSaveData();

        if (gameComplete)
            LastInteract();
        else
        {
            if (!interacted)
                FirstInteract();
            else
                NormalInteract();
        }
    }

    private void GetSaveData()
    {
        bool passepartoutMinigameCompleted = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.Passepartout)?.GetBoolValue(SaveDataStrings.COMPLETED) ?? false;
        bool fullSlotMachineMinigameCompleted = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.SlotMachine)?.GetBoolValue(SaveDataStrings.COMPLETED) ?? false;
        bool allChallegesCompleted = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.ChallengesSaved)?.GetBoolValue(SaveDataStrings.COMPLETED) ?? false;
        gameComplete = passepartoutMinigameCompleted && fullSlotMachineMinigameCompleted && allChallegesCompleted;
    }
}
