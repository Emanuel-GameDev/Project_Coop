using UnityEngine;
using UnityEngine.Events;

public class DemoEndDumpy : MonoBehaviour
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
    private static string DEMO_END_DUMPY_INTERACTED = "DemoEndDumpyInteracted";

    private void Awake()
    {
        if(SaveManager.Instance.TryLoadSetting<bool>(DEMO_END_DUMPY_INTERACTED, out bool value))
        {
            interacted = value;
        }
    }

    private void FirstInteract()
    {
        onFirstInteract.Invoke();
        interacted = true;
        SaveManager.Instance.SaveSetting(DEMO_END_DUMPY_INTERACTED, interacted);
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
        bool passepartoutMinigameCompleted = SaveManager.Instance.TryLoadSetting<bool>(SaveDataStrings.PASSEPARTOUT_MINIGAME_COMPLETED, out bool value) && value;
        bool fullSlotMachineMinigameCompleted = SaveManager.Instance.TryLoadSetting<bool>(SaveDataStrings.FOOLSLOT_MINIGAME_COMPLETED, out bool value2) && value2;
        bool allChallegesCompleted = SaveManager.Instance.TryLoadSetting<bool>("AllFirstZoneChallengesCompleted", out bool value3) && value3;
        gameComplete = passepartoutMinigameCompleted && fullSlotMachineMinigameCompleted && allChallegesCompleted;
    }
}
