using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum slotType
{
    Brutus,
    Kaina,
    Jude,
    Cassius,
    Dumpy,
    Lilith,
    Seven
}


public class Slotmachine : MonoBehaviour
{
    [Header("Variabili slot")]
    [SerializeField, Tooltip("Numero massimo di tentativi prima di vincere")]
    int lives = 3;
    int remainingLives;
    int numberOfJackpots = 0;
    public bool inGame = false;


    //Sprite List basato sul type
    [SerializeField] private List<Sprite> spriteDatabase;
    public List<slotType> allSlotType { get; private set; }
    [SerializeField] slotType[] WinCombination;

    [Header("Audio base")]
    [SerializeField] AudioClip winAudio;
    [SerializeField] AudioClip loseAudio;
    [SerializeField] AudioClip goodAudio;
    [SerializeField] AudioClip failAudio;
    [SerializeField] AudioClip stopRowAudio;
    [SerializeField] AudioSource loopSlotAudio;

    
    
    

    [Header("Win/lose Screen")]
    [SerializeField] GameObject jackpotScreen;
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
    [SerializeField, Tooltip("valore aggiunto alla velocità di rotazione per ogni fase successiva alla prima")]
    float incrementalVelocity = 1.5f;
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
    [SerializeField] private List<SpriteRenderer> buttonPlayerSpriteRenderers= new List<SpriteRenderer>();

    [Header("Manopola")]
    [SerializeField] private GameObject manopola;
    [SerializeField] private AudioClip giramentoManopolaAudio;


    //List<Sprite> playerSprites;



    [Header("Colonne")]

    [SerializeField] private List<SlotRow> rows;

    private int currentNumberOfTheSlot = 0;

    [Header("UI")]

    [SerializeField] private SlotMachineUI slotMachineUI;

    [SerializeField] private List<Sprite> PlayerUISprites;

    

    bool canInteract;
    

    [SerializeField] private SceneChanger sceneChanger;

    [Header("Dialogue Settings")]
    [SerializeField]
    private GameObject dialogueBox;
    [SerializeField]
    private Dialogue allJackpotDialogue;
    [SerializeField]
    private Dialogue slightyWinDialogue;
    [SerializeField]
    private Dialogue loseDialogue;
    [SerializeField]
    private UnityEvent onResultDialogueEnd;

    private DialogueBox _dialogueBox;


    [Header("Rewards Settings")]
    [SerializeField]
    private int coinForRightRow;
    [SerializeField]
    private int coinForJackpot;
    [SerializeField]
    private int keyForEachPlayer;



    private int totalCoinsBrutus;
    private int totalCoinsKaina;
    private int totalCoinsJude;
    private int totalCoinsCassius;
    private int totalJackpotCoins;
    [SerializeField]
    private WinScreenHandler winScreenHandler;



    //obsoleto
    //public GameObject GO;

    private void Awake()
    {
        //playerSprites = new List<Sprite>() { dpsSprite, tankSprite, rangedSprite, healerSprite };

        if (dialogueBox != null)
        {
            _dialogueBox = dialogueBox.GetComponent<DialogueBox>();
        }
        else
            Debug.LogError("DialogueBox is null");

        WinCombination = new slotType[rows.Count];
        allSlotType = Enum.GetValues(typeof(slotType)).Cast<slotType>().ToList();

        //sceneChanger = GetComponent<SceneChanger>();

        // GameManager.Instance.coopManager.playerInputPrefab = GO;
    }

    private void Start()
    {


        StartCoroutine(WaitForPlayers());
        remainingLives = lives;
    }

    public void InitializeNewTrySlotMachineMinigame()
    {
        randomListOfPlayer.Clear();
        
        //initialize win combination
        for (int i = 0; i < rows.Count; i++)
        {
            WinCombination[i] = allSlotType[UnityEngine.Random.Range(0, allSlotType.Count)];

            slotMachineUI.winCombinationUIGameObjects[i].sprite = ChoseSpriteBySlotType(WinCombination[i]);

            Material m = new Material(slotMachineUI.buttonUIGameObjects[i].material);
            slotMachineUI.buttonUIGameObjects[i].material = m;
            m.SetFloat("_IsGlowing", (int)0);

        }

        RandomReorder(listOfCurrentPlayer);

        if(remainingLives!=lives)
        {
            rotationSpeed += incrementalVelocity;
        }
                     

        foreach (SlotRow row in rows)
        {
            row.Initialize();
           
        }

        
        //slotMachineUI.UpdateRemainingTryText(remainingLives);
                
        //fail safe momentaneo
        currentNumberOfTheSlot = -1;

       

        canInteract = true;
    }

    public void SetIngameValueAfterCountDown()
    {
        inGame = true;
    }

    private IEnumerator CheckForJackpot()
    {
        bool win = true;       

        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].GetSelectedSlot().Type != WinCombination[i])
            {
                win = false;
                yield return null;
            }
        }

        if (win)
        {
            Debug.Log("Jackpot");

            AudioManager.Instance.PlayAudioClip(goodAudio);

            //aggiungo monete al jackpot

            totalJackpotCoins += coinForJackpot;
            numberOfJackpots++;

            canInteract = false;
            inGame = false;

            AudioManager.Instance.PlayAudioClip(winAudio);
            jackpotScreen.SetActive(true);


            //fai animazione/dialogo di vincita
            
            yield return new WaitForSeconds(screenDelay);

            jackpotScreen.SetActive(false);
        }
        else
        {
            Debug.Log("niente jackpot");

            canInteract = false;
            inGame = false;


            AudioManager.Instance.PlayAudioClip(failAudio);
            loseScreen.SetActive(true);


            yield return new WaitForSeconds(screenDelay);

            loseScreen.SetActive(false);





        }

        if (remainingLives <= 0)
        {
            ShowResult();
        }
        else
        {
            InitializeNewTrySlotMachineMinigame();
            inGame = true;
        }

        
    }   

    private void ShowResult()
    {
        _dialogueBox.RemoveAllDialogueEnd();

        if(numberOfJackpots == lives)
        {
            _dialogueBox.SetDialogue(allJackpotDialogue);
        }
        else if(numberOfJackpots > 0)
        {
            _dialogueBox.SetDialogue(slightyWinDialogue);
        }
        else
        {
            _dialogueBox.SetDialogue(loseDialogue);
        }
        
        _dialogueBox.AddDialogueEnd(onResultDialogueEnd);

        dialogueBox.SetActive(true);
        _dialogueBox.StartDialogue();

        MakeRankList();
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
            StartCoroutine(CheckForJackpot());
        }
    }

    public IEnumerator RestartSlotMachine()
    {
        loopSlotAudio.Play();

        remainingLives--;

        slotMachineUI.lightEasyModeGameobject.SetActive(false);
        slotMachineUI.LightMediumModeGameobject.SetActive(false);
        slotMachineUI.LighthardModeGameobject.SetActive(false);

        if(remainingLives == lives - 1)
        {
            slotMachineUI.lightEasyModeGameobject.SetActive(true);
            slotMachineUI.SetTextDifficulty("Easy");
        }
        else if(remainingLives == 0)
        {
            slotMachineUI.LighthardModeGameobject.SetActive(true);
            slotMachineUI.SetTextDifficulty("Hard");
        }
        else
        {
            slotMachineUI.LightMediumModeGameobject.SetActive(true);
            slotMachineUI.SetTextDifficulty("Medium");
        }

        foreach (SlotRow row in rows)
        {
            row.StartSlotMachine();

            yield return new WaitForSeconds(restartDelay);
        }

        currentNumberOfTheSlot = 0;
        canInteract = true;

        buttonSlots[currentNumberOfTheSlot].Arrow.SetActive(true);
    }

    private void SetRowInIndex(int index, ePlayerCharacter characterEnum)
    {
        Sprite buttonSprite = null;
        SpriteLibraryAsset libraryButton = null;

        switch (characterEnum)
        {
            case ePlayerCharacter.Brutus:
                buttonSprite = dpsButtonSprite;
                libraryButton = dpsSpriteLibrary;
                slotMachineUI.buttonUIGameObjects[index].sprite= slotMachineUI.charactersButtonsUISprites[0];
                break;
            case ePlayerCharacter.Kaina:

                buttonSprite = tankButtonSprite;
                libraryButton = tankSpriteLibrary;
                slotMachineUI.buttonUIGameObjects[index].sprite = slotMachineUI.charactersButtonsUISprites[1];
                break;
            case ePlayerCharacter.Cassius:

                buttonSprite = healerButtonSprite;
                libraryButton = healerSpriteLibrary;
                slotMachineUI.buttonUIGameObjects[index].sprite = slotMachineUI.charactersButtonsUISprites[3];
                break;
            case ePlayerCharacter.Jude:

                buttonSprite = rangedButtonSprite;
                libraryButton = rangedSpriteLibrary;
                slotMachineUI.buttonUIGameObjects[index].sprite = slotMachineUI.charactersButtonsUISprites[2];
                break;
            default:
                Debug.LogError("Personaggio non riconosciuto");
                break;
        }

        rows[index].SetRow(numberOfSlots, numberWinSlots, slotDistance, WinCombination[index], rotationSpeed, stabilizationSpeed);
        buttonSlots[index].InitializeButton(buttonSprite, libraryButton);
    }

    public Sprite ChoseSpriteBySlotType(slotType slotType)
    {
        slotType currentType;

        for (int i = 0;i< allSlotType.Count;i++)
        {
            currentType = allSlotType[i];

            if(currentType == slotType)
            {
                return spriteDatabase[i];
            }
        }

        return null;
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
        }
        while (randomListOfPlayer.Count < 4);

        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].selectedPlayer = randomListOfPlayer[i];

            for(int x=0;x< randomListOfPlayer.Count;x++)
            {
                if (randomListOfPlayer[i] == listOfCurrentPlayer[x])
                {
                    slotMachineUI.playerUISprite[i].sprite = PlayerUISprites[x];
                    buttonPlayerSpriteRenderers[i].sprite = PlayerUISprites[x];
                    break;
                }
            }
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



            yield return new WaitUntil(()=>rows[currentNumberOfTheSlot].stopped==true);

            //check se la colonna è stata bloccata sull'immagine giusta
            
            if (rows[currentNumberOfTheSlot].GetSelectedSlot().Type == WinCombination[currentNumberOfTheSlot])
            {
                AudioManager.Instance.PlayAudioClip(goodAudio);
                //TODO:aggiungere ui sulla destra con bottoni e immagini, fai illuminare i bottoni se hai azzeccato l'immagine

                
                slotMachineUI.buttonUIGameObjects[currentNumberOfTheSlot].material.SetFloat("_IsGlowing", (int)1);
               


                //aggiunta monete al personaggio
                switch (randomListOfPlayer[currentNumberOfTheSlot].GetCharacter())
                {                    
                    case ePlayerCharacter.Brutus:
                        totalCoinsBrutus += coinForRightRow;
                        break;
                    case ePlayerCharacter.Kaina:
                        totalCoinsKaina += coinForRightRow;
                        break;
                    case ePlayerCharacter.Cassius:
                        totalCoinsCassius += coinForRightRow;
                        break;
                    case ePlayerCharacter.Jude:
                        totalCoinsJude += coinForRightRow;
                        break;
                    default:
                        Debug.LogError("ePlayerCharacter non trovato");
                        break;
                }
            }
            else
            {
                AudioManager.Instance.PlayAudioClip(failAudio);
            }


            //passo alla prossima colonna
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
            canInteract = false;           
            
            StartCoroutine(RestartSlotMachine());

            //animazione manopola

            manopola.GetComponent<Animator>().SetTrigger("SpinTrigger");

            AudioManager.Instance.PlayAudioClip(giramentoManopolaAudio);


        }
    }

    public void InputFromPlayer(SlotPlayer player)
    {
        if (currentNumberOfTheSlot <= -1)
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
                
                switch (ranking[i])
                {
                    case ePlayerCharacter.Brutus:
                        totalCoin += (totalCoinsBrutus+totalJackpotCoins);
                        totalKey += keyForEachPlayer;
                        break;
                    case ePlayerCharacter.Kaina:
                        totalCoin += (totalCoinsKaina+totalJackpotCoins);
                        totalKey += keyForEachPlayer;
                        break;
                    case ePlayerCharacter.Jude:
                        totalCoin += (totalCoinsJude + totalJackpotCoins);
                        totalKey += keyForEachPlayer;
                        break;
                    case ePlayerCharacter.Cassius:
                        totalCoin += (totalCoinsCassius + totalJackpotCoins);
                        totalKey += keyForEachPlayer;
                        break;
                    default:
                        Debug.LogError("ePlayerCharacter non trovato");
                        break;
                }

                if (saveData != null)
                {
                    saveData.extraData.coin = totalCoin;
                    saveData.extraData.key = totalKey;
                }
            }

            Enum.TryParse<Rank>(i.ToString(), out Rank rank);
            winScreenHandler.SetCharacterValues(ranking[i], rank, gainedCoin, totalCoin, gainedKey, totalKey);

        }

        SaveManager.Instance.SaveData();


    }

    private bool CheckAndSaveYetCompleted()
    {
        SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.SlotMachine);
        if (sceneSetting == null)
            sceneSetting = new SceneSetting(SceneSaveSettings.SlotMachine);
        if (!sceneSetting.GetBoolValue(SaveDataStrings.COMPLETED))
        {
            sceneSetting.AddBoolValue(SaveDataStrings.COMPLETED, true);
            SaveManager.Instance.SaveSceneData(sceneSetting);
            return false;
        }
        else
            return true;
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
