using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class Challenge : MonoBehaviour
{
    [Header("Generics")]

    public LocalizedString challengeName;
    public LocalizedString challengeDescription;


    [Header("Enemies")]
    [SerializeField] public List<EnemySpawner> enemySpawnPoints;


    [Header("OnStart")]
    public Dialogue dialogueOnStart;
    [SerializeField] UnityEvent onChallengeStart;


    [Header("OnSuccess")]
    public Dialogue dialogueOnSuccess;
    [SerializeField] UnityEvent onChallengeSuccessEvent;
    [SerializeField] int coinsOnSuccess;
    [SerializeField] int keysOnSuccess;

    [Header("OnFail")]
    public Dialogue dialogueOnFailure;
    [SerializeField] UnityEvent onChallengeFailEvent;


    [HideInInspector] public List<EnemyCharacter> spawnedEnemiesList;
    [HideInInspector] public bool enemySpawned;
    [HideInInspector] public UnityEvent onChallengeStartAction;
    [HideInInspector] public UnityEvent onChallengeFailReset;
    [HideInInspector] public bool challengeCompleted;
    [HideInInspector] private bool challengeStarted;
    [HideInInspector] public ChallengeUI challengeUI;
    private string destinationSceneName = "ChallengeSceneTest";

    public void ActivateGameobject()
    {
        gameObject.SetActive(true);
    }
    public virtual void Initiate()
    {
        onChallengeStartAction.AddListener(StartChallenge);
        onChallengeFailReset.AddListener(ResetScene);


    }
    public virtual void StartChallenge()
    {
        Debug.Log("SFIDA INIZIATA");
        ChallengeManager.Instance.interacted = challengeStarted = true;
        ChallengeManager.Instance.selectedChallenge = this;
        onChallengeStart?.Invoke();


        ChallengeManager.Instance.timerText.gameObject.transform.parent.gameObject.SetActive(true);

    }
    public virtual void OnFailChallenge()
    {
        Debug.Log("HAI PERSO");
        onChallengeFailEvent?.Invoke();

        challengeCompleted = false;

        ResetChallenge();

    }
    public virtual void OnWinChallenge()
    {
        Debug.Log("HAI VINTO");
        challengeCompleted = true;

        onChallengeSuccessEvent?.Invoke();

        //RewardPopUP
        foreach (Transform HPContainer in HPHandler.Instance.HpContainerTransform)
        {
                   
            if (HPContainer.GetComponentInChildren<CharacterHUDContainer>() != null)
            {
                CharacterSaveData charSaveData = SaveManager.Instance.GetPlayerSaveData(HPContainer.GetComponentInChildren<CharacterHUDContainer>().referredCharacter.Character);

                RewardContainer rewardContainer = HPContainer.GetComponentInChildren<RewardContainer>();

                if (rewardContainer.right)
                {
                    GameObject tempReward = Instantiate(RewardManager.Instance.rightPrefabRewards, rewardContainer.transform);
                    tempReward.transform.position = rewardContainer.targetPosition.position;
                    tempReward.GetComponent<RewardUI>().SetUIValues(coinsOnSuccess, keysOnSuccess);
                    rewardContainer.rewardPopUp = tempReward;
                    StartCoroutine(rewardContainer.MoveCooroutine());

                }
                else
                {
                    GameObject tempReward = Instantiate(RewardManager.Instance.leftPrefabRewards, rewardContainer.transform);
                    tempReward.transform.position = rewardContainer.targetPosition.position;
                    tempReward.GetComponent<RewardUI>().SetUIValues(coinsOnSuccess, keysOnSuccess);
                    rewardContainer.rewardPopUp = tempReward;
                    StartCoroutine(rewardContainer.MoveCooroutine());

                }


                charSaveData.extraData.coin += coinsOnSuccess;
                charSaveData.extraData.key += keysOnSuccess;
                
            }

        }

        SaveManager.Instance.SaveSceneData(SceneSaveSettings.ChallengesSaved, ChallengeManager.Instance.currentSaveChallenges);
       

        ChallengeManager.Instance.timerText.gameObject.transform.parent.gameObject.SetActive(false);

        challengeUI.SetUpUI();
        ResetChallenge();



    }
    public virtual void AddToSpawned(EnemyCharacter tempEnemy)
    {
        TargetManager.Instance.AddEnemy(tempEnemy);
        spawnedEnemiesList.Add(tempEnemy);
        enemySpawned = true;
    }
    public virtual void OnEnemyDeath()
    {

    }
    public void ResetScene()
    {
        GameManager.Instance.ChangeScene(destinationSceneName);
    }

    public void ResetChallenge()
    {
        if (spawnedEnemiesList.Count > 0)
            foreach (EnemyCharacter e in spawnedEnemiesList)
            {
                if (e != null)
                    Destroy(e.gameObject);
            }

        foreach (EnemySpawner s in enemySpawnPoints)
        {
            s.canSpawn = false;
            s.gameObject.SetActive(false);
        }

        enemySpawned = false;
        challengeStarted = false;
        ChallengeManager.Instance.interacted = false;
    }

    protected void DisplayTimer(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        ChallengeManager.Instance.timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }

    internal void AutoComplete()
    {
        OnWinChallenge();
    }
}
