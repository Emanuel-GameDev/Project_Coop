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
    [SerializeField] private GameObject panelRanks;
    [SerializeField] private GameObject challengeUIPrefab;
    [SerializeField] private GameObject challengeUIRankPrefab;
    [SerializeField] public List<Challenge> currentSaveChallenges;
    [SerializeField] private GameObject bottoneInutile;
    [SerializeField] private string zoneSettingSaveName = "AllFirstZoneChallengesCompleted";

    [Header("Dialogue")]
    [SerializeField] Dialogue dialogueOnInteraction;
    public DialogueBox dialogueBox;

    [Header("Timer")]
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public TextMeshProUGUI challengeText;


    [HideInInspector] public Challenge selectedChallenge;
    public bool interacted;
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

        List<ChallengeData> savedChallenges = SaveManager.Instance.LoadChallenges();

        //Creo Salvataggio
        if (savedChallenges == null)
        {
            onInteractionAction.AddListener(OnInteraction);
            Shuffle(challengesList);
            savedChallenges = new();

            //creo Ui challenge
            for (int i = 0; i <3; i++)
            {
                ChallengeUI tempUI = CreateChallengeUI(challengesList[i]);
               
                //creo Ui rank
                if (challengesList[i].hasRanks)
                {
                    
                    for (int j = 0; j < 3; j++)
                    {
                        CreateChallengeRankUI(challengesList[i], tempUI, j);
                    }
                }

                currentSaveChallenges.Add(tempUI.challengeSelected);
                savedChallenges.Add(new(tempUI.challengeSelected.challengeNameEnum, false, 0));
            }

            SaveManager.Instance.SaveChallenges(savedChallenges);
        }

        //carico Salvataggio
        else
        {
            onInteractionAction.AddListener(OnInteraction);

            //creo Ui challenge
            foreach (ChallengeData c in savedChallenges)
            {
                
                ChallengeUI tempUI = CreateChallengeUI(challengesList.Find(x => x.challengeNameEnum == c.challengeName));
                tempUI.challengeSelected.challengeCompleted = c.completed;
               


                //creo Ui rank
                if (tempUI.challengeSelected.hasRanks)
                {                
                    for (int j = 0; j < 3; j++)
                    {
                        UiChallengeRank tempRankUI = CreateChallengeRankUI(tempUI.challengeSelected, tempUI, j);
                        tempRankUI.challengeSelected.rank = c.rank;
                        tempRankUI.SetUpUI(j);

                    }
                }

                tempUI.SetUpUI();
                currentSaveChallenges.Add(tempUI.challengeSelected);
            }

        }

    }

    private ChallengeUI CreateChallengeUI(Challenge challengeToSelect)
    {
        GameObject tempObj = Instantiate(challengeUIPrefab, panel.gameObject.transform);
        ChallengeUI tempUI = tempObj.GetComponent<ChallengeUI>();
        tempUI.challengeSelected = challengeToSelect;
        tempUI.challengeSelected.challengeUI = tempUI;
        tempUI.SetUpUI();
        return tempUI;
    }

    private UiChallengeRank CreateChallengeRankUI(Challenge challengeToSelect, ChallengeUI tempUI, int j)
    {
        GameObject tempRankObj = Instantiate(challengeUIRankPrefab, panelRanks.gameObject.transform);
        UiChallengeRank tempRankUI = tempRankObj.GetComponent<UiChallengeRank>();
        tempRankUI.challengeSelected = challengeToSelect; 
        tempUI.uiChallengeRanks.Add(tempRankUI);
        tempRankUI.SetUpUI(j);
        tempRankObj.SetActive(false);
        return tempRankUI;
    }

    #region saving
    public void SaveChallengeCompleted(ChallengeName challenge, bool completed)
    {
        List<ChallengeData> savedChallenges = SaveManager.Instance.LoadChallenges();

        if (savedChallenges == null)
        {
            SaveManager.Instance.SaveChallenge(new(challenge, completed, 0));
        }
        else
        {
            ChallengeData challengeData = savedChallenges.Find(x => x.challengeName == challenge);
            challengeData.completed = completed;
            SaveManager.Instance.SaveChallenge(challengeData);
        }


        //Controllo tutte sfide completate
        bool allChallegesCompleted = true;
        foreach (ChallengeData c in savedChallenges)
        {
            if (!c.completed)
            {
                allChallegesCompleted = false;
                break;
            }
        }
        SaveManager.Instance.SaveSetting(zoneSettingSaveName, allChallegesCompleted);
    }
    public void SaveChallengeRanks(ChallengeName challenge, int rank)
    {
        List<ChallengeData> savedChallenges = SaveManager.Instance.LoadChallenges();
    
            ChallengeData challengeData = savedChallenges.Find(x => x.challengeName == challenge);
            challengeData.rank = rank;
            SaveManager.Instance.SaveChallenge(challengeData);
              
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

    #region Interaction
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
        pressInteractable.CancelAllInteraction();
    }
    internal void ActivateInteractable()
    {
        if (pressInteractable == null)
            pressInteractable = GetComponentInChildren<PressInteractable>(true);

        pressInteractable.gameObject.SetActive(true);
    }
    #endregion
}
