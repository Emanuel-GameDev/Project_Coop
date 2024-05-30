using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class C_Survive : Challenge
{
    public override ChallengeName challengeNameEnum { get => ChallengeName.Survive; }

    [Header("Timer")]  
    [SerializeField] private float timerChallenge;

    
    private bool startTimer;
    public List<PlayerCharacter> players;
   



    public override void Initiate()
    {

        base.Initiate();

        ChallengeManager.Instance.dialogueBox.gameObject.SetActive(true);
        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnStart);
        ChallengeManager.Instance.dialogueBox.AddDialogueEnd(onChallengeStartAction);
        ChallengeManager.Instance.dialogueBox.StartDialogue();     
        
        players = PlayerCharacterPoolManager.Instance.ActivePlayerCharacters;
        foreach (PlayerCharacter p in players)
        {
            p.OnHit.AddListener(OnFailChallenge);
        }

    }
    public override void StartChallenge()
    {
        base.StartChallenge();
        foreach (EnemySpawner s in enemySpawnPoints)
        {
            s.canSpawn = true;           
        }

        startTimer = true;

    }
    public override void OnFailChallenge()
    {
        startTimer = false;
        base.OnFailChallenge();

        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnFailure);
        ChallengeManager.Instance.dialogueBox.RemoveAllDialogueEnd();
        ChallengeManager.Instance.dialogueBox.AddDialogueEnd(onChallengeFailReset);
        ChallengeManager.Instance.dialogueBox.StartDialogue();

    }
    public override void OnWinChallenge()
    {
        base.OnWinChallenge();

        challengeCompleted = true;
        startTimer = false;
        foreach (EnemyCharacter e in spawnedEnemiesList)
        {
            //CONTROLLARE
            Destroy(e.gameObject);
        }

        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnSuccess);
        ChallengeManager.Instance.dialogueBox.RemoveAllDialogueEnd();
        ChallengeManager.Instance.dialogueBox.StartDialogue();
    }

    public override void AddToSpawned(EnemyCharacter enemyCharacter)
    {
       base.AddToSpawned(enemyCharacter);
         enemyCharacter.OnDeath.AddListener(OnFailChallenge);
    }

   
    private void Update()
    {
        if (startTimer)
        {
            if (timerChallenge > 0)
            {
                timerChallenge -= Time.deltaTime;

            }
            else
            {                               
                    OnWinChallenge();
                             
            }

            DisplayTimer(timerChallenge);


        }
    }
    
}
