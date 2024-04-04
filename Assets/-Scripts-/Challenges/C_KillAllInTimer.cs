using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class C_KillAllInTimer : Challenge
{
    [Header("DEBUG")]
    public bool debug;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private float timerChallenge;

    [Header("Enemies")]
    [SerializeField] private List<EnemySpawner> enemySpawnPoints;

    private bool startTimer;
    private bool challengeCompleted;
    //CONTROLLARE
    [HideInInspector] public UnityEvent onChallengeStartAction;



    public override void Initiate()
    {
        //CONTROLLARE
        onChallengeStartAction.AddListener(StartChallenge);

        base.Initiate();     
            dialogueBox.gameObject.SetActive(true);
            dialogueBox.SetDialogue(dialogueOnStart);
            dialogueBox.AddDialogueEnd(onChallengeStartAction);
            dialogueBox.StartDialogue();                            
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
      base.OnFailChallenge();

        dialogueBox.SetDialogue(dialogueOnFailure);
        dialogueBox.RemoveAllDialogueEnd();
        dialogueBox.StartDialogue();
    }
    public override void OnWinChallenge()
    {
        base.OnWinChallenge();
        dialogueBox.SetDialogue(dialogueOnSuccess);
        dialogueBox.RemoveAllDialogueEnd();
        dialogueBox.StartDialogue();
    }
    private void Update()
    {
        //DEBUG
        if (Input.GetKeyUp(KeyCode.I) && debug)
        {
           Initiate();
        }

        if (startTimer)
        {
            if (timerChallenge > 0)
            {

                if (enemySpawned)
                {
                    challengeCompleted = true;
                    foreach(EnemyCharacter e in spawnedEnemies)
                    {
                        if (!e.isDead)
                        {
                            challengeCompleted = false;
                        }

                    }

                    if (challengeCompleted)
                    {
                        startTimer = false;
                        OnWinChallenge();
                    }
                    
                }

                timerChallenge -= Time.deltaTime;

            }
            else
            {

                startTimer = false;
                Time.timeScale = 0;
                OnFailChallenge();

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
