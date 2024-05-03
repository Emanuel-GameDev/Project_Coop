using System;

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
 * Gli SceneSaveSettings sono tutti i valori che possono essere salvati leagati al singolo salvataggio
 */
[Serializable]
public enum SceneSaveSettings
{
    FirstSceneDialogue,
    FirstMinigameDialogue,
}