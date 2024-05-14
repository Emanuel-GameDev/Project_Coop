using System.Collections.Generic;
using UnityEngine;

public class C_KillAllInTimer : Challenge
{

    [Header("Timer")]
    [SerializeField] private float timerChallenge;


    [Header("Modifiers")]
    [SerializeField] private bool noDamage;
    [SerializeField] private bool noDash;

    private bool startTimer;
    private int enemyInt = 0;
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
            if (noDamage)
                p.OnHit.AddListener(OnFailChallenge);

            if (noDash)
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

        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnFailure);
        ChallengeManager.Instance.dialogueBox.RemoveAllDialogueEnd();
        ChallengeManager.Instance.dialogueBox.AddDialogueEnd(onChallengeFailReset);
        ChallengeManager.Instance.dialogueBox.StartDialogue();



    }
    public override void OnWinChallenge()
    {
        base.OnWinChallenge();

        startTimer = false;

        ChallengeManager.Instance.dialogueBox.SetDialogue(dialogueOnSuccess);
        ChallengeManager.Instance.dialogueBox.RemoveAllDialogueEnd();
        ChallengeManager.Instance.dialogueBox.StartDialogue();
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

                if (enemySpawned && enemyInt == 0)
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
