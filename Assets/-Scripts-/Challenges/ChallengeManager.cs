using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChallengeManager : MonoBehaviour, IInteractable
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


    [HideInInspector] public Challenge selectedChallenge;
    [HideInInspector] public bool interacted;
    [HideInInspector] public UnityEvent onInteractionAction;

    private void Start()
    {
        SaveManager.Instance.LoadData();
        object challengesSelected = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.ChallengesSelected);
        
        Debug.LogWarning(challengesSelected);
        if(challengesSelected == null)
        {          
            challengesSelected = false;
        }
        Debug.LogWarning(challengesSelected);
        if ((bool)challengesSelected == false)
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

            SaveManager.Instance.SaveSceneData(SceneSaveSettings.ChallengesSaved, currentSaveChallenges);
            challengesSelected = true;
            SaveManager.Instance.SaveSceneData(SceneSaveSettings.ChallengesSelected, challengesSelected);

        }
        else
        {
            Debug.Log("Sfide trovate");
            onInteractionAction.AddListener(OnInteraction);

            foreach (Challenge c in (List<Challenge>)SaveManager.Instance.GetSceneSetting(SceneSaveSettings.ChallengesSaved))
            {
                GameObject tempObj = Instantiate(challengeUIPrefab, panel.gameObject.transform);
                ChallengeUI tempUI = tempObj.GetComponent<ChallengeUI>();
                tempUI.challengeSelected = c;
                tempUI.challengeSelected.challengeUI = tempUI;
                tempUI.SetUpUI();
                currentSaveChallenges.Add(tempUI.challengeSelected);
            }
           
        }
        
    }



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

    public void Interact(IInteracter interacter)
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.EnableInteraction(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.DisableInteraction(this);
        }
    }

    public void CancelInteraction(IInteracter interacter)
    {

    }

    public IInteracter GetFirstInteracter()
    {
        return null;
    }

    public void AbortInteraction(IInteracter interacter)
    {

    }
    private void OnInteraction()
    {
        MenuManager.Instance.OpenMenu(menuInfo, CoopManager.Instance.GetFirstPlayer());
        dialogueBox.RemoveAllDialogueEnd();
    }
}
