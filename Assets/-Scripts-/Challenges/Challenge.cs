using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class Challenge : MonoBehaviour
{
    [Header("Generics")]
    public DialogueBox dialogueBox;  
    public LocalizedString challengeName;
    public LocalizedString challengeDescription;
    
    [Header("Enemies")]
    [SerializeField] public List<EnemySpawner> enemySpawnPoints;


    [Header("OnStart")]
    public Dialogue dialogueOnStart;
    public UnityEvent onChallengeStart;


    [Header("OnSuccess")]
    public Dialogue dialogueOnSuccess;
    public UnityEvent onChallengeSuccessEvent;
    public int coinsOnSuccess;

    [Header("OnFail")]
    public Dialogue dialogueOnFailure;
    public UnityEvent onChallengeFailEvent;


    [HideInInspector] public List<EnemyCharacter> spawnedEnemiesList;
    [HideInInspector] public bool enemySpawned;
    [HideInInspector] public UnityEvent onChallengeStartAction;
    [HideInInspector] public UnityEvent onChallengeFailReset;
    [HideInInspector] public bool challengeCompleted;
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
        onChallengeStart?.Invoke();
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
    }

}
