using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Challenge : MonoBehaviour
{
    [Header("Generics")]
    public DialogueBox dialogueBox;

    [Header("OnStart")]
    public Dialogue dialogueOnStart;


    [Header("OnSuccess")]
    public Dialogue dialogueOnSuccess;
    public UnityEvent onChallengeSuccessEvent;

    [Header("OnFail")]
    public Dialogue dialogueOnFailure;
    public UnityEvent onChallengeFailEvent;


    [HideInInspector] public List<EnemyCharacter> spawnedEnemiesList;
    [HideInInspector] public bool enemySpawned;
    [HideInInspector] public UnityEvent onChallengeStartAction;
    [HideInInspector] public bool challengeCompleted;


    public virtual void Initiate()
    {
        onChallengeStartAction.AddListener(StartChallenge);
    }
    public virtual void StartChallenge()
    {
        Debug.Log("SFIDA INIZIATA");
    }
    public virtual void OnFailChallenge()
    {
        Debug.Log("HAI PERSO");
        onChallengeFailEvent?.Invoke();
    }
    public virtual void OnWinChallenge()
    {
        Debug.Log("HAI VINTO");
        onChallengeSuccessEvent?.Invoke();
    }

    public virtual void AddToSpawned(EnemyCharacter tempEnemy)
    {
        spawnedEnemiesList.Add(tempEnemy);  
    }

}
