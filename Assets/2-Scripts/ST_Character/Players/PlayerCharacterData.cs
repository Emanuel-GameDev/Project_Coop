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
    private RuntimeAnimatorController smashMinigameAnimator;

    [Header("Combat HUD")]
    [SerializeField]
    private Sprite hpContainerBackgroundLeft;
    [SerializeField]
    private Sprite hpContainerBackgroundRight;
    [SerializeField]
    private Sprite hudNormalFaceLeft;
    [SerializeField]
    private Sprite hudNormalFaceRight;
    [SerializeField]
    private Sprite hudHappyFaceLeft;
    [SerializeField]
    private Sprite hudHappyFaceRight;
    [SerializeField]
    private Sprite hudHitFaceLeft;
    [SerializeField]
    private Sprite hudHitFaceRight;
    [SerializeField]
    private Sprite hudDeathFaceLeft;
    [SerializeField]
    private Sprite hudDeathFaceRight;
    [SerializeField]
    private Sprite hudAbilityReadyLeft;
    [SerializeField]
    private Sprite hudAbilityReadyRight;

    public ePlayerCharacter Character => character;
    public Color CharacterColor => characterColor;
    public GameObject CharacterPrefab => characterPrefab;
    public Sprite FullBodyArt => fullBodyArt;  
    
    public Sprite DialogueSprite => dialogueSprite;
    public SpriteLibraryAsset PixelAnimations => pixelAnimations;
    public SpriteLibraryAsset TrashPressAnimations => trashPressAnimations;
    public Sprite PixelFaceSprite => pixelFaceSprite;
    public Sprite PixelBackgroundSprite => pixelBackgroundSprite;
    public Sprite UniqueAbilitySprite => uniqueAbilitySprite;
    public Sprite NotificationBackground => notificationBackground;
    public Sprite NotificationSprite => notificationSprite;

    #region CombatHUD

    public Sprite HpContainerLeft => hpContainerBackgroundLeft;
    public Sprite HpContainerRight => hpContainerBackgroundRight;
    public Sprite NormalFaceLeft => hudNormalFaceLeft;
    public Sprite NormalFaceRight => hudNormalFaceRight;
    public Sprite HappyFaceLeft => hudHappyFaceLeft;
    public Sprite HappyFaceRight => hudHappyFaceRight;
    public Sprite HitFaceLeft => hudHitFaceLeft;
    public Sprite HitFaceRight => hudHitFaceRight;
    public Sprite DeathFaceLeft => hudDeathFaceLeft;
    public Sprite DeathFaceRight => hudDeathFaceRight;
    public Sprite AbilityReadyLeft => hudAbilityReadyLeft;
    public Sprite AbilityReadyRight => hudAbilityReadyRight;

    #endregion

    public RuntimeAnimatorController SmashMinigameAnimator => smashMinigameAnimator;

}
