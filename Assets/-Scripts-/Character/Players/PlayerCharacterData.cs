using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacterData", menuName = "Character/PlayerCharacterData")]
public class PlayerCharacterData : ScriptableObject
{
    [SerializeField]
    private ePlayerCharacter character;
    [SerializeField]
    private Color characterColor;
    [SerializeField] 
    private GameObject characterClassPrefab;
    [SerializeField]
    private Sprite fullBodyArt;
    [SerializeField]
    private Sprite hudSprite;
    [SerializeField]
    private Sprite dialogueSprite;
    [SerializeField]
    private Sprite pixelSprite;

    public ePlayerCharacter Character => character;
    public Color CharacterColor => characterColor;
    public GameObject CharacterClassPrefab => characterClassPrefab;
    public Sprite FullBodyArt => fullBodyArt;
    public Sprite HudSprite => hudSprite;
    public Sprite DialogueSprite => dialogueSprite;
    public Sprite PixelSprite => pixelSprite;

}