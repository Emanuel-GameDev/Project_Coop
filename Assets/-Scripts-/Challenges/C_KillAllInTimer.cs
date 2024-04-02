using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class C_KillAllInTimer : Challenge
{
    [Header("DEBUG")]
    public bool debug;
    [Header("=============")]
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private float timerChallenge;
    [SerializeField] private List<EnemySpawner> enemySpawnPoints;
    private bool startTimer;
   
    public override void Initiate()
    {
        if (!debug)
        {
            dialogueBox.gameObject.SetActive(true);
            dialogueBox.StartDialogue();
           
        }

    }

    public override void StartChallenge()
    {
        foreach (EnemySpawner s in enemySpawnPoints)
        {
            s.canSpawn = true;
        }
        TimerText.gameObject.SetActive(true);
        startTimer = true;

    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.I) && debug)
        {
            dialogueBox.gameObject.SetActive(true);
            dialogueBox.StartDialogue();
            
        }

        if (startTimer)
        {
            if(timerChallenge > 0)
            {             
                timerChallenge -= Time.deltaTime;
            }          
           else
            {
                Debug.Log("Hai perso");
                startTimer = false;
            }

            DisplayTimer(timerChallenge);
           

        }
    }

    private void DisplayTimer(float timeToDisplay)
    {
      
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        TimerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);
       
    }
}
