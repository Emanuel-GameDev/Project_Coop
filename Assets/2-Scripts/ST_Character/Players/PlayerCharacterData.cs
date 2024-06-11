using UnityEngine;
using UnityEngine.Localization;
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
    private Sprite hudSprite;
    [SerializeField]
    private Sprite dialogueSprite;
    [SerializeField]
    private SpriteLibraryAsset pixelAnimations;
    [SerializeField]
    private Sprite pixelFaceSprite;
    [SerializeField]
    private Sprite pixelBackgroundSprite;
    [SerializeField]
    private Sprite uniqueAbilitySprite;
    [SerializeField]
    private Sprite notificationBackground;
    [SerializeField] 
    private Sprite hpContainerSpriteLeft;
    [SerializeField]
    private Sprite hpContainerSpriteRight;

    [Header("Character Texts")]
    [SerializeField]
    private LocalizedString abilityName1;
    [SerializeField]
    private LocalizedString abilityName2;
    [SerializeField]
    private LocalizedString abilityName3;
    [SerializeField]
    private LocalizedString abilityName4;
    [SerializeField]
    private LocalizedString abilityName5;
    [SerializeField]
    private LocalizedString abilityDescription1;
    [SerializeField]
    private LocalizedString abilityDescription2;
    [SerializeField]
    private LocalizedString abilityDescription3;
    [SerializeField]
    private LocalizedString abilityDescription4;
    [SerializeField]
    private LocalizedString abilityDescription5;
    [SerializeField]
    private LocalizedString characterBiography;
    


    public ePlayerCharacter Character => character;
    public Color CharacterColor => characterColor;
    public GameObject CharacterPrefab => characterPrefab;
    public Sprite FullBodyArt => fullBodyArt;  
    public Sprite HudSprite => hudSprite;
    public Sprite DialogueSprite => dialogueSprite;
    public SpriteLibraryAsset PixelAnimations => pixelAnimations;
    public Sprite PixelFaceSprite => pixelFaceSprite;
    public Sprite PixelBackgroundSprite => pixelBackgroundSprite;
    public Sprite UniqueAbilitySprite => uniqueAbilitySprite;
    public Sprite NotificationBackground => notificationBackground;
    public Sprite HpContainerLeft => hpContainerSpriteLeft;
    public Sprite HpContainerRight => hpContainerSpriteRight;

    #region Texts
    public LocalizedString AbilityName1 => abilityName1;
    public LocalizedString AbilityName2 => abilityName2;
    public LocalizedString AbilityName3 => abilityName3;
    public LocalizedString AbilityName4 => abilityName4;
    public LocalizedString AbilityName5 => abilityName5;
    public LocalizedString AbilityDescription1 => abilityDescription1;
    public LocalizedString AbilityDescription2 => abilityDescription2;
    public LocalizedString AbilityDescription3 => abilityDescription3;
    public LocalizedString AbilityDescription4 => abilityDescription4;
    public LocalizedString AbilityDescription5 => abilityDescription5;
    public LocalizedString CharacterBiography => characterBiography;
    #endregion


}
