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
 * Gli SceneSaveSettings sono tutti i valori che possono essere salvati legati al singolo salvataggio
 */

[Serializable]
public enum SceneSaveSettings
{
    FirstSceneDialogue,
    FirstMinigameDialogue,
    ChallengesSaved,
    SviluppartyInteractions,
    DialogueTrigger,
    Passepartout,
    SlotMachine

}

public static class SaveDataStrings
{
    public const string COMPLETED = "Completed";
    public const string INTERACTED = "Interacted";
    public const string UNLOCKED = "Unlocked";
    public const string SELECTED = "Selected";
    public const string SVILUPPARTY_END_DUMPY_INTERACTED = "SviluppartyEndDumpyInteracted";
    public const string CHALLENGE = "Challenge";
}

