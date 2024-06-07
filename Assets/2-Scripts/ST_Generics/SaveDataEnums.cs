using System;
using System.Collections.Generic;
using UnityEngine;

/* 
 * I PlayerPrefsSettings sono tutti i valori che possono essere salvati nel playerPrefs.
 * Servono per salvare le impostazioni generiche relative al gioco.
 * NON devono memorizzare impostazioni legate al singolo salvataggio
 */
[Serializable]
public enum PlayerPrefsSettings
{
    FirstStart,
    Languge,
    MasterVolume,
    MusicVolume,
    SFXVolume
}

/*
 * Gli SceneSaveSettings sono da eliminare li lascio temporaneamente come Reference.
 */

[Serializable]
public enum SceneSaveSettings
{
    //Dialogues
    FirstSceneDialogue,
    FirstMinigameDialogue,
    FirstTimeChallegeDialogueTriggered,
    //Minigames
    Passepartout,
    SlotMachine,
    //Shops
    ShopLilithFirstTime,
    ShopSouvenirFirstTime,
    //Challeges
    ChallengesSaved,
    AllFirstZoneChallengesCompleted,
    AllChallengesCompleted,
    //Others
    SviluppartyInteractions
}

/*
 * Le SaveData Strings possono essere utilizzate in caso di salvataggi che devono essere verificati da script diversi
 * così da avere un riferimento sicuro alla Key giusta
 */


public static class SaveDataStrings
{
    //Minigames
    public const string PASSEPARTOUT_MINIGAME_COMPLETED = "PassepartoutMinigameCompleted";
    public const string FOOLSLOT_MINIGAME_COMPLETED = "FoolSlotMinigameCompleted";
    public const string TRASHPRESS_MINIGAME_COMPLETED = "TrashPressMinigameCompleted";

}

