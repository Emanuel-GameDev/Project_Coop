using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class C_KillTillDead : Challenge
{

    [Header("Ranks")]
    [SerializeField] int enemiesFirstStar;   
    [SerializeField] int enemiesSecondStar;   
    [SerializeField] int enemiesThirdStar;
    
    private int enemyKilled = 0;
    public List<PlayerCharacter> activePlayers;



    public override void Initiate()
    {
        base.Initiate();

        ChallengeManager.Instance.dialogueBox.gameObject.SetActive(true);
        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnStart);
        ChallengeManager.Instance.dialogueBox.AddDialogueEnd(onChallengeStartAction);
        ChallengeManager.Instance.dialogueBox.StartDialogue();

        activePlayers = PlayerCharacterPoolManager.Instance.AllPlayerCharacters;
        foreach (PlayerCharacter p in activePlayers)
        {        
           p.OnDeath.AddListener(CheckChallengeResult);
        }

    }

    private void CheckChallengeResult()
    {
      if(enemyKilled >= 15)
        {
            OnWinChallenge();
        }
        else
        {
            OnFailChallenge();
        }
    }
    public override void StartChallenge()
    {
        base.StartChallenge();
        foreach (EnemySpawner s in enemySpawnPoints)
        {
            s.canSpawn = true;
        }
        enemyKilled = 0;
        ChallengeManager.Instance.timerText.text = enemyKilled.ToString();


    }
    public override void OnFailChallenge()
    {     
        base.OnFailChallenge();

        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnFailure);
        ChallengeManager.Instance.dialogueBox.RemoveAllDialogueEnd();
        ChallengeManager.Instance.dialogueBox.AddDialogueEnd(onChallengeFailReset);
        ChallengeManager.Instance.dialogueBox.StartDialogue();

    }
    public override void OnWinChallenge()
    {
        base.OnWinChallenge();

        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnSuccess);
        ChallengeManager.Instance.dialogueBox.RemoveAllDialogueEnd();
        ChallengeManager.Instance.dialogueBox.StartDialogue();
    }

    public override void AddToSpawned(EnemyCharacter enemyCharacter)
    {
        base.AddToSpawned(enemyCharacter);       
        enemyCharacter.OnDeath.AddListener(OnEnemyDeath);

    }  
    public override void OnEnemyDeath()
    {
        base.OnEnemyDeath();
        enemyKilled++;
        ChallengeManager.Instance.timerText.text = enemyKilled.ToString();
    }

    
}
