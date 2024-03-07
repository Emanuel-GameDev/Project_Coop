using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Slotmachine : MonoBehaviour
{
    [Header("Variabili slot")]
    [SerializeField, Tooltip("Numero massimo di tentativi prima di vincere")]
    int lives = 3;
    [Header("Variabili colonna")]   
    
    [SerializeField,Tooltip("Numero delle figure totali nella colonna")]
    [Min(5)]
    int numberOfSlots=0;
    [SerializeField,Tooltip("Numero delle figure vincenti nella colonna")]
    int numberWinSlots=0;
    [SerializeField,Tooltip("Distanza delle figure nella colonna")] 
    float slotDistance = 0.25f;
    [SerializeField,Tooltip("Velocità di rotazione della colonna")] 
    float rotationSpeed=0;
    [SerializeField,Tooltip("Velocità/tempo impiegato alla colonna per fermarsi")] 
    float stabilizationSpeed = 0;
    [SerializeField, Tooltip("Delay restart della colonna")]
    float restartDelay=0;

    [Header("Variabili per vedere in editor")]

    [SerializeField] public ePlayerCharacter currentPlayer;
    [SerializeField] public List<SlotPlayer> listOfCurrentPlayer;
    [SerializeField] public List<SlotPlayer> randomListOfPlayer;

    [Header("Sprite figure")]
    [SerializeField] private Sprite dpsSprite;
    [SerializeField] private Sprite tankSprite;
    [SerializeField] private Sprite rangedSprite;
    [SerializeField] private Sprite healerSprite;
    [SerializeField] private Sprite enemySprite;

    List<Sprite> playerSprites;

    [Header("Colonne")]

    [SerializeField] private List<SlotRow> rows;

    private int currentNumberOfTheSlot = 0;

    bool canInteract;
    public bool inGame=false;
    

    //obsoleto
    //public GameObject GO;

    private void Awake()
    {
        playerSprites= new List<Sprite>() { dpsSprite, tankSprite, rangedSprite, healerSprite};

        

        // GameManager.Instance.coopManager.playerInputPrefab = GO;
    }

    private void Start()
    {
        

    }

    private void InitializeSlotMachineMinigame()
    {
        RandomReorder(listOfCurrentPlayer);

        //forse cancellare
        /*
        foreach (SlotRow row in rows)
        {
            row.SetRow(numberOfSlots, numberWinSlots, slotDistance, playerSprites, enemySprite, rotationSpeed, stabilizationSpeed);
        }
        */

        //da aggiungere dopo una possibile animazione/tutorial

        foreach (SlotRow row in rows)
        {
            row.Initialize();
            row.StartSlotMachine();
        }


        canInteract = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            InitializeSlotMachineMinigame();
        }

        if (Input.GetKeyDown(KeyCode.M)) 
        {
            StartCoroutine(RestartSlotMachine());
        }
        //TODO: inserire input manuali per debug

        //if (canInteract)
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        rows[0].StartSlowDown();
        //    }

        //    if (Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        rows[1].StartSlowDown();
        //    }

        //    if (Input.GetKeyDown(KeyCode.Alpha3))
        //    {
        //        rows[2].StartSlowDown();
        //    }

        //    if (Input.GetKeyDown(KeyCode.Alpha4))
        //    {
        //        rows[3].StartSlowDown();
        //    }
        //}
        //else
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        StartCoroutine(RestartSlotMachine());
        //    }
        //}
       

    }

    private void CheckForWin()
    {
        bool win = true;

        foreach (SlotRow row in rows)
        {
            if(row.GetSelectedSlot().Type != slotType.Player)
            {
                win = false;
                break;
            }
        }

        if (win)
        {
            Debug.Log("avete vinto");

            //fai animazione/dialogo di vincita
        }
        else
        {
            Debug.Log("avete perso");

            lives--;

            if(lives <= 0) 
            {
                
                //fai animazione/dialogo perdita
            }
        }
    }

    public GameObject GetLastRow()
    {
        return rows[rows.Count - 1].gameObject;
    }

    public void CheckAllRowStopped()
    {
        bool allStopped=true;

        foreach (SlotRow row in rows)
        {
            if(!row.stopped)
            {
                allStopped = false;
                break;
            }
        }

        if (allStopped)
        {
            canInteract = false;
            CheckForWin();
        }
    }

    public IEnumerator RestartSlotMachine()
    {
        foreach (SlotRow row in rows)
        {
            row.ResetRow();

            yield return new WaitForSeconds(restartDelay);
        }

        currentNumberOfTheSlot = 0;
        canInteract = true;
    }

    private void SetRowInIndex(int index,ePlayerCharacter characterEnum) 
    {
        Sprite playerSprites=null;

        switch (characterEnum)
        {
            case ePlayerCharacter.Brutus:
                playerSprites = dpsSprite;
                break;
            case ePlayerCharacter.Caina:
                playerSprites = tankSprite;
                break;
            case ePlayerCharacter.Cassius:
                playerSprites = healerSprite;
                break;
            case ePlayerCharacter.Jude:
                playerSprites = rangedSprite;
                break;
            default:
                Debug.LogError("Personaggio non riconosciuto");
                break;
        }

        rows[index].SetRow(numberOfSlots, numberWinSlots, slotDistance, playerSprites, enemySprite, rotationSpeed, stabilizationSpeed);
    }

    private void RandomReorder(List<SlotPlayer> currentPlayersList) 
    { 
        List<SlotPlayer> notRandomPlayers = new List<SlotPlayer>(currentPlayersList);

        //TODO guardare se si puoò fare in modo dinamico
        List<ePlayerCharacter> characterEnum = new List<ePlayerCharacter>() { ePlayerCharacter.Brutus, ePlayerCharacter.Caina, ePlayerCharacter.Cassius, ePlayerCharacter.Jude };

        if(notRandomPlayers.Count == 0)
        {
            Debug.LogError("Non ci sono player qua");
            return;
        }

        int indexRow = 0;
        do 
        {

            SlotPlayer player = notRandomPlayers[UnityEngine.Random.Range(0, notRandomPlayers.Count)];

            randomListOfPlayer.Add(player);


            notRandomPlayers.Remove(player);

            if(player.GetCharacter()== ePlayerCharacter.EmptyCharacter) 
            {
                Debug.LogError("il giocatore non ha un character, ne verrà associato uno randomico, ma non dovrebbe succedere");

                ePlayerCharacter characterRemainingType = characterEnum[UnityEngine.Random.Range(0, characterEnum.Count)];

                SetRowInIndex(indexRow, characterRemainingType);

                characterEnum.Remove(characterRemainingType);
            }
            else
            {
                SetRowInIndex(indexRow, player.GetCharacter());

                characterEnum.Remove(player.GetCharacter());
            }

            

            indexRow++;
            
        }
        while (notRandomPlayers.Count > 0);

        int index = 0;

        

        do 
        {
            ePlayerCharacter characterRemainingType= characterEnum[UnityEngine.Random.Range(0,characterEnum.Count)];

            characterEnum.Remove(characterRemainingType);

            if (index >= currentPlayersList.Count)
            {
                index=0;
            }

            randomListOfPlayer.Add(currentPlayersList[index]);

            //setto le immagini rimanenti

            SetRowInIndex(indexRow, characterRemainingType);


            index++;
            indexRow++;
        }
        while(randomListOfPlayer.Count < 4);

        for (int i = 0; i < rows.Count; i++) 
        {
            rows[i].selectedPlayer = randomListOfPlayer[i];
        }
    }

    private IEnumerator InputStopRowSlot(SlotPlayer player)
    {
        //ferma la riga per il giocatore giocante

        if(player == rows[currentNumberOfTheSlot].selectedPlayer && canInteract) 
        {
            canInteract= false;

            rows[currentNumberOfTheSlot].StartSlowDown();

            

            yield return new WaitForSeconds(stabilizationSpeed);

            currentNumberOfTheSlot++;
            canInteract = true;
        }

        
    }

    public void InputStop(SlotPlayer player) 
    {
        StartCoroutine(InputStopRowSlot(player));
    }


}
