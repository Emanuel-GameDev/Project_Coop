using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class C_KillTillDead : Challenge
{
    public override ChallengeName challengeNameEnum { get => ChallengeName.killTillDead;}
    [Header("Ranks Settings")]
    [SerializeField] int enemiesFirstStar;   
    [SerializeField] int enemiesSecondStar;   
    [SerializeField] int enemiesThirdStar;
    
    
    private int enemyKilled = 0;
    private int enemiesSpawned = 0;
    public List<PlayerCharacter> activePlayers;



    public override void Initiate()
    {
        base.Initiate();

        ChallengeManager.Instance.dialogueBox.gameObject.SetActive(true);
        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnStart);
        ChallengeManager.Instance.dialogueBox.RemoveAllDialogueEnd();
        ChallengeManager.Instance.dialogueBox.AddDialogueEnd(onChallengeStartAction);
        ChallengeManager.Instance.dialogueBox.StartDialogue();
    
        foreach (PlayerCharacter p in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {        
           p.OnDeath.AddListener(CheckChallengeResult);
        }

    }

    private void CheckChallengeResult()
    {
      if(enemyKilled >= enemiesFirstStar)
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
        PlayerCharacterPoolManager.Instance.showDeathScreen = false;
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
        foreach (PlayerCharacter p in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            p.Ress();
        }

        base.OnWinChallenge();

       
        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnSuccess);
        ChallengeManager.Instance.dialogueBox.RemoveAllDialogueEnd();
        ChallengeManager.Instance.dialogueBox.StartDialogue();
    }

    public override void AddToSpawned(EnemyCharacter enemyCharacter)
    {
        base.AddToSpawned(enemyCharacter);
        enemiesSpawned++;
        UpdateEnemyCounter();

        enemyCharacter.OnDeath.AddListener(OnEnemyDeath);

    }  
    public override void OnEnemyDeath()
    {
        base.OnEnemyDeath();
        enemyKilled++;
        CheckStarRating();
        UpdateEnemyCounter();
    }

    private void UpdateEnemyCounter()
    {
        ChallengeManager.Instance.timerText.text = (enemyKilled.ToString() + "/" + enemiesSpawned.ToString());
    }

    public override void CheckStarRating()
    {
        if(rank == 0 && enemyKilled >= enemiesFirstStar)
        {
            rank++;
            Debug.LogWarning("FIRST StarObtained");
            GiveRewardPopUp(coinsFirstRank, keysFirstRank);
            
        }
        if (rank == 1 && enemyKilled >= enemiesSecondStar)
        {
            rank++;
            Debug.LogWarning("SECOND StarObtained");
            GiveRewardPopUp(coinsSecondRank, keysSecondRank);
        }
        if (rank == 2 && enemyKilled >= enemiesThirdStar)
        {
            rank++;
            Debug.LogWarning("THIRD StarObtained");
            GiveRewardPopUp(coinsThirdRank, keysThirdRank);
        }
        if(enemiesSpawned == enemyKilled)
        {
            OnWinChallenge();
        }
    }
}
