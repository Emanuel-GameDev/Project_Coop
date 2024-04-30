using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class Challenge : MonoBehaviour
{
    [Header("Generics")]  
    public DialogueBox dialogueBox;  
    public LocalizedString challengeName;
    public LocalizedString challengeDescription;
    
    [SerializeField] protected TextMeshProUGUI TimerText;
   
    [Header("Enemies")]
    [SerializeField] public List<EnemySpawner> enemySpawnPoints;


    [Header("OnStart")]
    public  Dialogue dialogueOnStart;
    [SerializeField] UnityEvent onChallengeStart;


    [Header("OnSuccess")]
    public Dialogue dialogueOnSuccess;
    [SerializeField] UnityEvent onChallengeSuccessEvent;
    [SerializeField] int coinsOnSuccess;
    [SerializeField] int KeysOnSuccess;

    [Header("OnFail")]
    public Dialogue dialogueOnFailure;
    [SerializeField] UnityEvent onChallengeFailEvent;


    [HideInInspector] public List<EnemyCharacter> spawnedEnemiesList;
    [HideInInspector] public bool enemySpawned;
    [HideInInspector] public UnityEvent onChallengeStartAction;
    [HideInInspector] public UnityEvent onChallengeFailReset;
    [HideInInspector] public bool challengeCompleted;
    [HideInInspector] private bool challengeStarted;
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
        ChallengeManager.Instance.started = challengeStarted = true;
        onChallengeStart?.Invoke();

        
        TimerText.gameObject.transform.parent.gameObject.SetActive(true);
       
    }
    public virtual void OnFailChallenge()
    {
        Debug.Log("HAI PERSO");
        onChallengeFailEvent?.Invoke();

        challengeCompleted = false;   
     
        ResetChallenge();

        dialogueBox.SetDialogue(dialogueOnFailure);
        dialogueBox.RemoveAllDialogueEnd();
        dialogueBox.AddDialogueEnd(onChallengeFailReset);
        dialogueBox.StartDialogue();

        
    }
    public virtual void OnWinChallenge()
    {
        Debug.Log("HAI VINTO");
        onChallengeSuccessEvent?.Invoke();
        foreach(Transform rewardContainer in HPHandler.Instance.rewardsContainerTransform)
        {
           
            if(rewardContainer.parent.GetComponentInChildren<CharacterHUDContainer>() != null)
            {
                GameObject tempReward = Instantiate(ChallengeManager.Instance.rewardsUIprefeb, rewardContainer);
                tempReward.GetComponent<RewardUI>().SetUIValues(coinsOnSuccess, KeysOnSuccess);
                Destroy(tempReward,ChallengeManager.Instance.rewardsPopUpDuration);
            }
           
        }
        TimerText.gameObject.transform.parent.gameObject.SetActive(false);

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
        foreach (EnemyCharacter e in spawnedEnemiesList) 
        { 
            e.gameObject.SetActive(false);
        }
        foreach(EnemySpawner s in enemySpawnPoints)
        {
            s.canSpawn = false;
            s.gameObject.SetActive(false);
        }
        enemySpawned = false;
        challengeStarted = false;
    }

    protected void DisplayTimer(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }

}
