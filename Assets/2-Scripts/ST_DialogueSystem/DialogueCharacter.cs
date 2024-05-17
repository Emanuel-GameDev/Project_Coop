using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Dialogue/Character")]
public class DialogueCharacter : ScriptableObject
{
    [Header("Name info")]
    [SerializeField] private string characterName;
    [SerializeField] private Color characterNameDefaultColor = Color.white;
    [SerializeField] private TMP_FontAsset characterDefaultNameFont;

    [Header("Dialogue info")]
    [SerializeField] private Color characterDialogueDefaultColor = Color.white;
    [SerializeField] private TMP_FontAsset characterDefaultDialogueFont;
    [SerializeField,Min(0.1f)] private float dialogueCharacterPerSecond = 30;

    [Header("Other info")]
    [SerializeField] private Sprite characterImage;
    [SerializeField] private Color characterFrameColor = Color.white;
    [SerializeField] private AudioClip characterVoice;


    public string Name
    {
        get { return characterName; }
#if UNITY_EDITOR
        set { characterName = value; }
#endif
    }

    public Sprite CharacterImage
    {
        get { return characterImage; }
#if UNITY_EDITOR
        set { characterImage = value; }
#endif
    }

    public Color CharacterColor
    {
        get { return characterFrameColor; }
#if UNITY_EDITOR
        set { characterFrameColor = value; }
#endif
    }

    public Color CharacterNameColor
    {
        get { return characterNameDefaultColor; }
#if UNITY_EDITOR
        set { characterNameDefaultColor = value; }
#endif
    }

    public Color CharacterDialogueColor
    {
        get { return characterDialogueDefaultColor; }
#if UNITY_EDITOR
        set { characterDialogueDefaultColor = value; }
#endif
    }

    public TMP_FontAsset CharacterNameFont
    {
        get { return characterDefaultNameFont; }
#if UNITY_EDITOR
        set { characterDefaultNameFont = value; }
#endif
    }

    public TMP_FontAsset CharacterDialogueFont
    {
        get { return characterDefaultDialogueFont; }
#if UNITY_EDITOR
        set { characterDefaultDialogueFont = value; }
#endif
    }

    public AudioClip CharacterVoice
    {
        get { return characterVoice; }
#if UNITY_EDITOR
        set { characterVoice = value; }
#endif
    }

    public float DialogueCharacterPerSecond
    {
        get { return dialogueCharacterPerSecond; }
#if UNITY_EDITOR
        set { dialogueCharacterPerSecond = value; }
#endif
    }
}
