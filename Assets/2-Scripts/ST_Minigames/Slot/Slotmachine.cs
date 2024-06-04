using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D.Animation;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Slotmachine : MonoBehaviour
{
    [Header("Variabili slot")]
    [SerializeField, Tooltip("Numero massimo di tentativi prima di vincere")]
    int lives = 3;
    int remainingLives;
    [SerializeField] AudioClip winAudio;
    [SerializeField] AudioClip loseAudio;
    [SerializeField] AudioClip goodAudio;
    [SerializeField] AudioClip failAudio;
    [SerializeField] AudioClip stopRowAudio;
    [SerializeField] AudioSource loopSlotAudio;

    [Header("Win/lose Screen")]
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] float screenTime=5f;

    [Header("Variabili colonna")]

    [SerializeField, Tooltip("Numero delle figure totali nella colonna")]
    [Min(5)]
    int numberOfSlots = 0;
    [SerializeField, Tooltip("Numero delle figure vincenti nella colonna")]
    int numberWinSlots = 0;
    [SerializeField, Tooltip("Distanza delle figure nella colonna")]
    float slotDistance = 0.25f;
    [SerializeField, Tooltip("Velocità di rotazione della colonna")]
    float rotationSpeed = 0;
    [SerializeField, Tooltip("Velocità/tempo impiegato alla colonna per fermarsi")]
    float stabilizationSpeed = 0;
    [SerializeField, Tooltip("Delay restart della colonna")]
    float restartDelay = 0;
    [SerializeField, Tooltip("Delay della schermata vincita/perdita")]
    float screenDelay = 1.5f;

    [Header("Variabili per vedere in editor")]

    [SerializeField] public ePlayerCharacter currentPlayer;
    [SerializeField] public List<SlotPlayer> listOfCurrentPlayer;
    [SerializeField] public List<SlotPlayer> randomListOfPlayer;

    [Header("Sprite figure")]
    [SerializeField] private Sprite dpsSprite;
    [SerializeField] private Sprite tankSprite;
    [SerializeField] private Sprite rangedSprite;
    [SerializeField] private Sprite healerSprite;
    [SerializeField] private List<Sprite> losingSpriteList = new List<Sprite>();

    [Header("Sprite Button")]
    [SerializeField] private Sprite dpsButtonSprite;
    [SerializeField] private Sprite tankButtonSprite;
    [SerializeField] private Sprite rangedButtonSprite;
    [SerializeField] private Sprite healerButtonSprite;

    [Header("Button SpriteLibrary")]
    [SerializeField] private SpriteLibraryAsset dpsSpriteLibrary;
    [SerializeField] private SpriteLibraryAsset tankSpriteLibrary;
    [SerializeField] private SpriteLibraryAsset rangedSpriteLibrary;
    [SerializeField] private SpriteLibraryAsset healerSpriteLibrary;

    [Header("Buttons")]
    [SerializeField] private List<ButtonSlot> buttonSlots = new List<ButtonSlot>();

    [Header("Manopola")]
    [SerializeField] private GameObject manopola;
    [SerializeField] private AudioClip giramentoManopolaAudio;
    [SerializeField] private GameObject ActionButton;


    List<Sprite> playerSprites;



    [Header("Colonne")]

    [SerializeField] private List<SlotRow> rows;

    private int currentNumberOfTheSlot = 0;

    [Header("UI")]

    [SerializeField] private SlotMachineUI slotMachineUI;

    bool canInteract;
    public bool inGame = false;

    [SerializeField] private SceneChanger sceneChanger;

    [Header("Dialogue Settings")]
    [SerializeField]
    private GameObject dialogueBox;
    [SerializeField]
    private Dialogue winDialogue;
    [SerializeField]
    private Dialogue loseDialogue;
    [SerializeField]
    private UnityEvent onWinDialogueEnd;
    [SerializeField]
    private UnityEvent onLoseDialogueEnd;

    private DialogueBox _dialogueBox;


    [Header("Rewards Settings")]
    [SerializeField]
    private int coinForEachPlayer;
    [SerializeField]
    private int coinForFirstPlayer;
    [SerializeField]
    private int keyForEachPlayer;
    [SerializeField]
    private int keyForFirstPlayer;
    [SerializeField]
    private WinScreenHandler winScreenHandler;



    //obsoleto
    //public GameObject GO;

    private void Awake()
    {
        playerSprites = new List<Sprite>() { dpsSprite, tankSprite, rangedSprite, healerSprite };

        if (dialogueBox != null)
        {
            _dialogueBox = dialogueBox.GetComponent<DialogueBox>();
        }
        else
            Debug.LogError("DialogueBox is null");

        //sceneChanger = GetComponent<SceneChanger>();

        // GameManager.Instance.coopManager.playerInputPrefab = GO;
    }

    public void SetPauseGame(bool value)
    {
        if (value)
            GameManager.Instance.PauseGame();
        else
            GameManager.Instance.ResumeGame();
    }
    private void Start()
    {


        StartCoroutine(WaitForPlayers());
    }

    public void InitializeSlotMachineMinigame()
    {
        randomListOfPlayer.Clear();
        RandomReorder(listOfCurrentPlayer);
        remainingLives = lives;

        loopSlotAudio.Play();

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

        remainingLives--;
        slotMachineUI.UpdateRemainingTryText(remainingLives);


        canInteract = true;
        

        currentNumberOfTheSlot = 0;

        buttonSlots[currentNumberOfTheSlot].Arrow.SetActive(true);
    }

    public void SetIngameValueAfterCountDown()
    {
        inGame = true;
    }

    private void CheckForWin()
    {
        bool win = true;

        foreach (SlotRow row in rows)
        {
            if (row.GetSelectedSlot().Type != slotType.Player)
            {
                win = false;
                break;
            }
        }

        if (win)
        {
            Debug.Log("avete vinto");

            AudioManager.Instance.PlayAudioClip(goodAudio);

            canInteract = false;
            inGame = false;


            //fai animazione/dialogo di vincita
            StartCoroutine(ShowWin());
        }
        else
        {
            Debug.Log("avete perso");

            AudioManager.Instance.PlayAudioClip(failAudio);

            if (remainingLives <= 0)
            {
                canInteract = false;
                inGame = false;


                //fai animazione/dialogo perdita
                StartCoroutine(ShowLose());




                return;
            }
            else
            {
                ActionButton.SetActive(true);
            }


        }
    }

    private IEnumerator ShowWin()
    {
        _dialogueBox.RemoveAllDialogueEnd();

        yield return new WaitForSeconds(screenDelay);

        AudioManager.Instance.PlayAudioClip(winAudio);
        winScreen.SetActive(true);
        yield return new WaitForSeconds(screenTime);
        winScreen.SetActive(false);


        _dialogueBox.SetDialogue(winDialogue);
        _dialogueBox.AddDialogueEnd(onWinDialogueEnd);

        dialogueBox.SetActive(true);
        _dialogueBox.StartDialogue();

        MakeRankList();
    }

    private IEnumerator ShowLose()
    {
        _dialogueBox.RemoveAllDialogueEnd();

        yield return new WaitForSeconds(screenDelay);

        AudioManager.Instance.PlayAudioClip(loseAudio);

        loseScreen.SetActive(true);
        yield return new WaitForSeconds(screenTime);
        loseScreen.SetActive(false);

        _dialogueBox.SetDialogue(loseDialogue);
        _dialogueBox.AddDialogueEnd(onLoseDialogueEnd);

        dialogueBox.SetActive(true);
        _dialogueBox.StartDialogue();
    }



    public GameObject GetLastRow()
    {
        return rows[rows.Count - 1].gameObject;
    }

    public void CheckAllRowStopped()
    {
        bool allStopped = true;

        foreach (SlotRow row in rows)
        {
            if (!row.stopped)
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
        loopSlotAudio.Play();
        
        foreach (SlotRow row in rows)
        {
            row.ResetRow();

            yield return new WaitForSeconds(restartDelay);
        }

        currentNumberOfTheSlot = 0;
        canInteract = true;

        buttonSlots[currentNumberOfTheSlot].Arrow.SetActive(true);
    }

    private void SetRowInIndex(int index, ePlayerCharacter characterEnum)
    {
        Sprite playerSprites = null;
        Sprite buttonSprite = null;
        SpriteLibraryAsset libraryButton = null;

        switch (characterEnum)
        {
            case ePlayerCharacter.Brutus:
                playerSprites = dpsSprite;
                buttonSprite = dpsButtonSprite;
                libraryButton = dpsSpriteLibrary;
                break;
            case ePlayerCharacter.Kaina:
                playerSprites = tankSprite;
                buttonSprite = tankButtonSprite;
                libraryButton = tankSpriteLibrary;
                break;
            case ePlayerCharacter.Cassius:
                playerSprites = healerSprite;
                buttonSprite = healerButtonSprite;
                libraryButton = healerSpriteLibrary;
                break;
            case ePlayerCharacter.Jude:
                playerSprites = rangedSprite;
                buttonSprite = rangedButtonSprite;
                libraryButton = rangedSpriteLibrary;
                break;
            default:
                Debug.LogError("Personaggio non riconosciuto");
                break;
        }

        rows[index].SetRow(numberOfSlots, numberWinSlots, slotDistance, playerSprites, losingSpriteList, rotationSpeed, stabilizationSpeed);
        buttonSlots[index].InitializeButton(buttonSprite, libraryButton);
    }

    private void RandomReorder(List<SlotPlayer> currentPlayersList)
    {
        List<SlotPlayer> notRandomPlayers = new List<SlotPlayer>(currentPlayersList);

        //TODO guardare se si puoò fare in modo dinamico
        List<ePlayerCharacter> characterEnum = new List<ePlayerCharacter>() { ePlayerCharacter.Brutus, ePlayerCharacter.Kaina, ePlayerCharacter.Cassius, ePlayerCharacter.Jude };

        if (notRandomPlayers.Count == 0)
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

            if (player.GetCharacter() == ePlayerCharacter.EmptyCharacter)
            {
                Debug.LogError("il giocatore non ha un character, ne verrà associato uno randomico, ma non dovrebbe succedere");

                ePlayerCharacter characterRemainingType = characterEnum[UnityEngine.Random.Range(0, characterEnum.Count)];

                SetRowInIndex(indexRow, characterRemainingType);

                characterEnum.Remove(characterRemainingType);

                player.SetCharacter(characterRemainingType);
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


        if(characterEnum.Count != 0)
        do
        {
            ePlayerCharacter characterRemainingType = characterEnum[UnityEngine.Random.Range(0, characterEnum.Count)];

            characterEnum.Remove(characterRemainingType);

            if (index >= currentPlayersList.Count)
            {
                index = 0;
            }

            randomListOfPlayer.Add(currentPlayersList[index]);

            //setto le immagini rimanenti

            SetRowInIndex(indexRow, characterRemainingType);


            index++;
            indexRow++;
            Debug.Log(index);
        }
        while (randomListOfPlayer.Count < 4);

        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].selectedPlayer = randomListOfPlayer[i];
        }
    }

    private IEnumerator InputRowSlot(SlotPlayer player)
    {

        if (player == rows[currentNumberOfTheSlot].selectedPlayer && canInteract) //ferma la riga per il giocatore giocante
        {
            canInteract = false;

            buttonSlots[currentNumberOfTheSlot].Arrow.SetActive(false);

            //animazione premuta tasto

            buttonSlots[currentNumberOfTheSlot].GetComponent<Animator>().SetTrigger("Press");

            rows[currentNumberOfTheSlot].StartSlowDown();

            //sounds
            //play suono di stop
            AudioManager.Instance.PlayAudioClip(stopRowAudio);


            //se è l'ultima riga fai smettere la canzone del loop

            if(currentNumberOfTheSlot >= rows.Count - 1)
            {
                loopSlotAudio.Stop();
            }



            yield return new WaitForSeconds(stabilizationSpeed);

            currentNumberOfTheSlot++;
            

            if (currentNumberOfTheSlot <= rows.Count - 1)
            {
                buttonSlots[currentNumberOfTheSlot].Arrow.SetActive(true);
            }

            canInteract = true;



        }


    }

    private void InputRestartSlot(SlotPlayer player)
    {

        if (canInteract && player == listOfCurrentPlayer[0]) //restarta la slot se tutta ferma
        {
            ActionButton.gameObject.SetActive(false);
            canInteract = false;
            remainingLives--;
            slotMachineUI.UpdateRemainingTryText(remainingLives);
            StartCoroutine(RestartSlotMachine());

            //animazione manopola

            manopola.GetComponent<Animator>().SetTrigger("SpinTrigger");

            AudioManager.Instance.PlayAudioClip(giramentoManopolaAudio);


        }
    }

    public void InputFromPlayer(SlotPlayer player)
    {
        if (currentNumberOfTheSlot > rows.Count - 1)
        {
            InputRestartSlot(player);
        }
        else
        {
            StartCoroutine(InputRowSlot(player));
        }

    }

    private void MakeRankList()
    {
        List<SlotPlayer> winOrder = new();

        foreach (SlotPlayer player in listOfCurrentPlayer)
        {
            winOrder.Add(player);
        }



        List<ePlayerCharacter> ranking = new();

        foreach (SlotPlayer player in winOrder)
        {
            ranking.Add(player.GetCharacter());
        }

        foreach (ePlayerCharacter c in Enum.GetValues(typeof(ePlayerCharacter)))
        {
            if (c != ePlayerCharacter.EmptyCharacter)
            {
                if (!ranking.Contains(c))
                {
                    ranking.Add(c);
                }
            }
        }

        bool yetCompleted = CheckAndSaveYetCompleted();

        for (int i = 0; i < ranking.Count; i++)
        {
            int totalCoin = 0;
            int totalKey = 0;
            int gainedCoin = 0;
            int gainedKey = 0;

            CharacterSaveData saveData = SaveManager.Instance.GetPlayerSaveData(ranking[i]);

            if (saveData != null)
            {
                totalCoin = saveData.extraData.coin;
                totalKey = saveData.extraData.key;
            }

            Debug.Log(yetCompleted);

            if (!yetCompleted)
            {
                if (i == 0)
                {
                    totalCoin += coinForFirstPlayer;
                    totalKey += keyForFirstPlayer;

                    gainedCoin += coinForFirstPlayer;
                    gainedKey += keyForFirstPlayer;
                }
                else
                {
                    totalCoin += coinForEachPlayer;
                    totalKey += keyForEachPlayer;

                    gainedCoin += coinForEachPlayer;
                    gainedKey += keyForEachPlayer;

                }

                if (saveData != null)
                {
                    saveData.extraData.coin = totalCoin;
                    saveData.extraData.key = totalKey;
                }
            }

            Enum.TryParse<Rank>(i.ToString(), out Rank rank);
            ePlayerID playerID = listOfCurrentPlayer.Find(x => x.GetCharacter() == ranking[i]).GetInputHandler().playerID;
            winScreenHandler.SetCharacterValues(playerID, ranking[i], rank, gainedCoin, totalCoin, gainedKey, totalKey);

        }

        SaveManager.Instance.SaveData();


    }

    private bool CheckAndSaveYetCompleted()
    {
        if (SaveManager.Instance.TryLoadSetting<bool>(SaveDataStrings.FOOLSLOT_MINIGAME_COMPLETED, out bool value))
        {
            return value;
        }
        else
        {
            SaveManager.Instance.SaveSetting(SaveDataStrings.FOOLSLOT_MINIGAME_COMPLETED, true);
            return false;
        }
    }



    IEnumerator WaitForPlayers()
    {
        yield return new WaitUntil(() => CoopManager.Instance.GetActiveHandlers() != null && CoopManager.Instance.GetActiveHandlers().Count > 0);
        //dialogueObject.SetActive(true);
        //DA RIVEDERE #MODIFICATO

        dialogueBox.SetActive(true);
        _dialogueBox.StartDialogue();
    }

    public void ExitMinigame()
    {

        if (sceneChanger != null)
        {
            sceneChanger.ChangeScene();
            Debug.Log(sceneChanger.ToString());
        }
    }


}
