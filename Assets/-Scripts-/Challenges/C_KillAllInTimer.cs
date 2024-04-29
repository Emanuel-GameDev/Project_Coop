using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class C_KillAllInTimer : Challenge
{

    [Header("Timer")]   
    [SerializeField] private float timerChallenge;

    
    [Header("Modifiers")]
    [SerializeField] private bool noDamage;
    [SerializeField] private bool noDash;

    private bool startTimer;
    private int enemyInt =0;
    public List<PlayerCharacter> players;



    public override void Initiate()
    {
        base.Initiate();     
            dialogueBox.gameObject.SetActive(true);
            dialogueBox.SetDialogue(dialogueOnStart);
            dialogueBox.AddDialogueEnd(onChallengeStartAction);
            dialogueBox.StartDialogue();

        players = PlayerCharacterPoolManager.Instance.ActivePlayerCharacters;
        foreach (PlayerCharacter p in players)
        {
            if(noDamage)
             p.OnHit.AddListener(OnFailChallenge);

            if(noDash)
             p.OnDash.AddListener(OnFailChallenge);
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

    }
    public override void OnWinChallenge()
    {
        base.OnWinChallenge();

        startTimer = false;
        dialogueBox.SetDialogue(dialogueOnSuccess);
        dialogueBox.RemoveAllDialogueEnd();
        dialogueBox.StartDialogue();
    }
    public override void AddToSpawned(EnemyCharacter enemyCharacter)
    {
        base.AddToSpawned(enemyCharacter);
        enemyInt++;
        enemyCharacter.OnDeath.AddListener(OnEnemyDeath);
        
    }
    private void Update()
    {     
        if (startTimer)
        {
            if (timerChallenge > 0)
            {
              
                if(enemySpawned && enemyInt ==0) 
                { 
                    OnWinChallenge();
                }

                timerChallenge -= Time.deltaTime;

            }
            else
            {
                OnFailChallenge();
            }

            DisplayTimer(timerChallenge);

        }
    }
   
    public override void OnEnemyDeath()
    {
        base.OnEnemyDeath();
        enemyInt--;
    }
}
