using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class Challenge : MonoBehaviour
{
    [Header("Generics")]
    public LocalizedString challengeName;
    public LocalizedString challengeDescription;
    public virtual ChallengeName challengeNameEnum { get; }

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

    [Header("Ranks")]
    public bool hasRanks;
    [SerializeField] public LocalizedString firstStarDescription;
    [SerializeField] protected int coinsFirstRank;
    [SerializeField] protected int keysFirstRank;
    [SerializeField] public LocalizedString secondStarDescription;
    [SerializeField] protected int coinsSecondRank;
    [SerializeField] protected int keysSecondRank;
    [SerializeField] public LocalizedString thirdStarDescription;
    [SerializeField] protected int coinsThirdRank;
    [SerializeField] protected int keysThirdRank;


    [HideInInspector] public List<EnemyCharacter> spawnedEnemiesList;
    [HideInInspector] public bool enemySpawned;
    [HideInInspector] public UnityEvent onChallengeStartAction;
    [HideInInspector] public UnityEvent onChallengeFailReset;
    [HideInInspector] public bool challengeCompleted;
    [HideInInspector] private bool challengeStarted;
    [HideInInspector] public ChallengeUI challengeUI;
    private string destinationSceneName = "ChallengeScene";

    [HideInInspector] public int rank = 0;

    #region gameFuncions 
    public virtual void Initiate()
    {
        PlayerCharacterPoolManager.Instance.showDeathScreen = true;
        foreach (PlayerCharacter p in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            p.OnDeath.RemoveAllListeners();
        }

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
        ChallengeManager.Instance.DeactivateInteractable();
        DisplayChallengeDescription();

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


        //RewardPopUP
        GiveRewardPopUp(coinsOnSuccess, keysOnSuccess);

        PlayerCharacterPoolManager.Instance.HealAllPlayerFull();
        SaveManager.Instance.SavePlayersData();


        ChallengeManager.Instance.timerText.gameObject.transform.parent.gameObject.SetActive(false);
        ChallengeManager.Instance.SaveChallengeCompleted(this.challengeNameEnum, challengeCompleted);
        ChallengeManager.Instance.SaveChallengeRanks(this.challengeNameEnum, rank);

        onChallengeSuccessEvent?.Invoke();

        challengeUI.SetUpUI();
        challengeUI.ShowRanks(false);
        ResetChallenge();
        ChallengeManager.Instance.ActivateInteractable();
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
        //ChallengeManager.Instance.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
    internal void AutoComplete()
    {
        OnWinChallenge();
    }
    #endregion

    public void ActivateGameobject()
    {
        gameObject.SetActive(true);
    }
    #region enemies
    public virtual void AddToSpawned(EnemyCharacter tempEnemy)
    {
        TargetManager.Instance.AddEnemy(tempEnemy);
        spawnedEnemiesList.Add(tempEnemy);
        enemySpawned = true;
    }
    public virtual void OnEnemyDeath()
    {

    }
    #endregion

    protected void GiveRewardPopUp(int coinsToGive, int keysToGive)
    {
        foreach (Transform HPContainer in HPHandler.Instance.HpContainerTransform)
        {

            if (HPContainer.GetComponentInChildren<CharacterHUDContainer>() != null)
            {

                RewardContainer rewardContainer = HPContainer.GetComponentInChildren<RewardContainer>();

                if (rewardContainer.right)
                {
                    GameObject tempReward = Instantiate(RewardManager.Instance.rightPrefabRewards, rewardContainer.transform);
                    tempReward.transform.position = rewardContainer.targetPosition.position;
                    tempReward.GetComponent<RewardUI>().SetUIValues(coinsToGive, keysToGive);
                    rewardContainer.rewardPopUp = tempReward;
                    StartCoroutine(rewardContainer.MoveCooroutine());

                }
                else
                {
                    GameObject tempReward = Instantiate(RewardManager.Instance.leftPrefabRewards, rewardContainer.transform);
                    tempReward.transform.position = rewardContainer.targetPosition.position;
                    tempReward.GetComponent<RewardUI>().SetUIValues(coinsToGive, keysToGive);
                    rewardContainer.rewardPopUp = tempReward;
                    StartCoroutine(rewardContainer.MoveCooroutine());

                }



                SaveManager.Instance.SavePlayersData();

            }


        }
        foreach (PlayerCharacter p in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            p.ExtraData.coin += coinsOnSuccess;
            p.ExtraData.key += keysOnSuccess;
            if (p.isDead)
            {
                p.Ress();
            }

        }
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
    protected void DisplayChallengeDescription()
    {
        ChallengeManager.Instance.challengeText.GetComponent<LocalizeStringEvent>().StringReference = challengeDescription;
    }
   
    public virtual void CheckStarRating()
    {

    }
}

public enum ChallengeName
{
    noName,
    Survive,
    KillAllInTimer,
    KerberosBoss,
    KillAllNoDefenceAbility,
    KillAllNoDamage,
    killTillDead,
    defendBox
}

