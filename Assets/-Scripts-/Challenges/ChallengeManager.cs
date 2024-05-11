using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChallengeManager : MonoBehaviour
{

    private static ChallengeManager _instance;
    public static ChallengeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ChallengeManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("ChallengeManager");
                    _instance = singletonObject.AddComponent<ChallengeManager>();
                }
            }

            return _instance;
        }
    }

    [Header("Generics")]
    [SerializeField] private List<Challenge> challengesList;
    [SerializeField] private MenuInfo menuInfo;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject challengeUIPrefab;
    [SerializeField] public List<Challenge> currentSaveChallenges;
    [SerializeField] private GameObject bottoneInutile;

    [Header("Dialogue")]
    [SerializeField] Dialogue dialogueOnInteraction;
    public DialogueBox dialogueBox;

    [Header("Timer")]
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public TextMeshProUGUI challengeText;


    [HideInInspector] public Challenge selectedChallenge;
    [HideInInspector] public bool interacted;
    [HideInInspector] public UnityEvent onInteractionAction;
    [SerializeField] private PressInteractable pressInteractable;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
           
        }
    }
    private void Start()
    {
        SaveManager.Instance.LoadData();

        SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.ChallengesSaved);
        bool selected = false;

        if (sceneSetting == null)
        {
            sceneSetting = new(SceneSaveSettings.ChallengesSaved);
            sceneSetting.AddBoolValue(SaveDataStrings.SELECTED, selected);
        }
        else
        {
            selected = sceneSetting.GetBoolValue(SaveDataStrings.SELECTED);
            sceneSetting.AddBoolValue(SaveDataStrings.SELECTED, selected);
        }

        if (!selected)
        {
            onInteractionAction.AddListener(OnInteraction);
            Shuffle(challengesList);

            for (int i = 0; i < 3; ++i)
            {
                GameObject tempObj = Instantiate(challengeUIPrefab, panel.gameObject.transform);
                ChallengeUI tempUI = tempObj.GetComponent<ChallengeUI>();
                tempUI.challengeSelected = challengesList[i];
                tempUI.challengeSelected.challengeUI = tempUI;
                tempUI.SetUpUI();
                currentSaveChallenges.Add(tempUI.challengeSelected);
            }

            sceneSetting.AddBoolValue(SaveDataStrings.SELECTED, true);

            SaveSceneData(sceneSetting);
        }
        else
        {
            onInteractionAction.AddListener(OnInteraction);

            foreach (SavingStringValue c in sceneSetting.strings.FindAll(x => x.valueName == SaveDataStrings.CHALLENGE))
            {
                GameObject tempObj = Instantiate(challengeUIPrefab, panel.gameObject.transform);
                ChallengeUI tempUI = tempObj.GetComponent<ChallengeUI>();
                tempUI.challengeSelected = challengesList.Find(x => x.name == c.value);
                tempUI.challengeSelected.challengeUI = tempUI;
                tempUI.challengeSelected.challengeCompleted = sceneSetting.GetBoolValue(tempUI.challengeSelected.name);
                tempUI.SetUpUI();
                currentSaveChallenges.Add(tempUI.challengeSelected);
            }

        }

    }

    #region saving
    private void SaveSceneData(SceneSetting sceneSetting)
    {
        foreach (Challenge c in currentSaveChallenges)
        {
            
            sceneSetting.strings.Add(new(SaveDataStrings.CHALLENGE, c.name.ToString()));
        }

        SaveManager.Instance.SaveSceneData(sceneSetting);
    }
    public void SaveChallengeCompleted(string challengeName, bool completed)
    {
        SceneSetting settings = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.ChallengesSaved);
        settings.AddBoolValue(challengeName, completed);

        bool allCompleted = true;
        foreach (Challenge c in challengesList)
        {
            if(!c.challengeCompleted)
                allCompleted = false;
        }

        settings.AddBoolValue(SaveDataStrings.COMPLETED, allCompleted);

        SaveManager.Instance.SaveSceneData(settings);
    }
    #endregion

    public static void Shuffle(List<Challenge> list)
    {
        int count = list.Count;
        int last = count - 1;
        for (int i = 0; i < last; ++i)
        {
            int r = UnityEngine.Random.Range(i, count);
            Challenge tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    public void Interact()
    {
        if (!interacted)
        {
            bottoneInutile.SetActive(true);
            dialogueBox.SetDialogue(dialogueOnInteraction);
            dialogueBox.RemoveAllDialogueEnd();
            dialogueBox.AddDialogueEnd(onInteractionAction);
            dialogueBox.StartDialogue();

        }

    }


    private void OnInteraction()
    {
        MenuManager.Instance.OpenMenu(menuInfo, CoopManager.Instance.GetFirstPlayer());
        dialogueBox.RemoveAllDialogueEnd();
        
    }

    public void DeactivateInteractable()
    {
        if (pressInteractable == null)
            pressInteractable = GetComponentInChildren<PressInteractable>();

        pressInteractable.gameObject.SetActive(false);
    }
    internal void ActivateInteractable()
    {
        if (pressInteractable == null)
            pressInteractable = GetComponentInChildren<PressInteractable>(true);

        pressInteractable.gameObject.SetActive(true);
    }
}
