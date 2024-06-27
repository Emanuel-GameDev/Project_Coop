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
    private Sprite hudSprite;
    [SerializeField]
    private Sprite dialogueSprite;
    [SerializeField]
    private Sprite notificationSprite;
    [SerializeField]
    private SpriteLibraryAsset pixelAnimations;
    [SerializeField]
    private SpriteLibraryAsset trashPressAnimations;
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
    [SerializeField] 
    private Sprite hpContainerSpriteLeft;
    [SerializeField]
    private Sprite hpContainerSpriteRight;
    [SerializeField]
    private RuntimeAnimatorController smashMinigameAnimator;
    [SerializeField]
    private Sprite p1Sprite;
    [SerializeField]
    private Sprite p2Sprite;
    [SerializeField]
    private Sprite p3Sprite;
    [SerializeField]
    private Sprite p4Sprite;



    public ePlayerCharacter Character => character;
    public Color CharacterColor => characterColor;
    public GameObject CharacterPrefab => characterPrefab;
    public Sprite FullBodyArt => fullBodyArt;  
    public Sprite HudSprite => hudSprite;
    public Sprite DialogueSprite => dialogueSprite;
    public SpriteLibraryAsset PixelAnimations => pixelAnimations;
    public SpriteLibraryAsset TrashPressAnimations => trashPressAnimations;
    public Sprite PixelFaceSprite => pixelFaceSprite;
    public Sprite PixelBackgroundSprite => pixelBackgroundSprite;
    public Sprite UniqueAbilitySprite => uniqueAbilitySprite;
    public Sprite NotificationBackground => notificationBackground;
    public Sprite NotificationSprite => notificationSprite;

    public Sprite HpContainerLeft => hpContainerSpriteLeft;
    public Sprite HpContainerRight => hpContainerSpriteRight;

    public RuntimeAnimatorController SmashMinigameAnimator => smashMinigameAnimator;

    public Sprite P1Sprite => p1Sprite;
    public Sprite P2Sprite => p2Sprite;
    public Sprite P3Sprite => p3Sprite;
    public Sprite P4Sprite => p4Sprite;

}
