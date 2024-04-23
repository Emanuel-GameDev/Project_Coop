using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class C_Survive : Challenge
{
    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private float timerChallenge;

    private bool startTimer;
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

        TimerText.gameObject.SetActive(true);
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

        challengeCompleted = true;
        startTimer = false;
        foreach (EnemyCharacter e in spawnedEnemiesList)
        {
            //CONTROLLARE
            Destroy(e.gameObject);
        }

        dialogueBox.SetDialogue(dialogueOnSuccess);
        dialogueBox.RemoveAllDialogueEnd();
        dialogueBox.StartDialogue();
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
    private void DisplayTimer(float timeToDisplay)
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
