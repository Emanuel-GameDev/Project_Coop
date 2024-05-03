using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "PlayerCharacterData", menuName = "Character/PlayerCharacterData")]
public class PlayerCharacterData : ScriptableObject
{
    [SerializeField]
    private ePlayerCharacter character;
    [SerializeField]
    private Color characterColor;
    [SerializeField] 
    private GameObject characterPrefab;
    [SerializeField]
    private Sprite fullBodyArt;
    [SerializeField]
    private Sprite hudHealthSprite;
    [SerializeField]
    private Sprite hudSprite;
    [SerializeField]
    private Sprite dialogueSprite;
    [SerializeField]
    private SpriteLibraryAsset pixelAnimations;
    [SerializeField]
    private Sprite pixelFaceSprite;
    [SerializeField]
    private Sprite pixelBackgroundSprite;
    [SerializeField, TextArea]
    private string characterDescription;
    [SerializeField]
    private Sprite uniqueAbilitySprite;
    [SerializeField]
    private Sprite notificationBackground;



    public ePlayerCharacter Character => character;
    public Color CharacterColor => characterColor;
    public GameObject CharacterPrefab => characterPrefab;
    public Sprite FullBodyArt => fullBodyArt;
    public Sprite HudHealthSprite => hudHealthSprite;
    public Sprite HudSprite => hudSprite;
    public Sprite DialogueSprite => dialogueSprite;
    public SpriteLibraryAsset PixelAnimations => pixelAnimations;
    public Sprite PixelFaceSprite => pixelFaceSprite;
    public Sprite PixelBackgroundSprite => pixelBackgroundSprite;
    public Sprite UniqueAbilitySprite => uniqueAbilitySprite;
    public Sprite NotificationBackground => notificationBackground;

}
