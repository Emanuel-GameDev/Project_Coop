using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterHUDContainer : MonoBehaviour
{
    [SerializeField] public ePlayerID referredPlayerID;
    [SerializeField] public PlayerCharacter referredCharacter;
    ePlayerID PlayerID => referredPlayerID;


    [Header("HP & Background")]
    [SerializeField] Image characterHUDbackground;
    [SerializeField] Image characterHUDFace;
    [SerializeField] Image playerIdImage;
    [SerializeField] TMP_Text maxHP;
    [SerializeField] TMP_Text currentHP;
    [SerializeField] GenericBarScript hpBar;
    [SerializeField] public bool right;

    [Header("Ability")]
    [SerializeField] Image abilityImage;
    [SerializeField] Image abilityReadyImage;
    [SerializeField] GameObject abilityReady;
    [SerializeField] private Slider abilitySlider;
    [SerializeField] Animation readyAnimation;

    //private float uniqueAbilityCooldown;
    //private float uniqueAbilityCooldownRemainingTime;
    //private bool uniqueAbilityUsed;
    private bool uniqueAbilityReadyAnimationExecuted = false;


    [Header("EffectCooldown")]
    [SerializeField] RectTransform effectsStartingPoint;
    [SerializeField] RectTransform effectsPoolPoint;
    [SerializeField] Vector2 fillDirection;

    [Header("SwitchingCooldown")]
    [SerializeField] private Slider switchSlider;

    private void Update()
    {
        UniqueAbilityCooldownUpdate();
    }

    private void UniqueAbilityCooldownUpdate()
    {
        if (referredCharacter == null) return;

        if (!referredCharacter.UniqueAbilityAvaiable)
        {
            abilitySlider.value = 1 - (referredCharacter.UniqueAbilityRemainingCooldown / referredCharacter.UniqueAbilityCooldown);
            if (uniqueAbilityReadyAnimationExecuted)
            {
                uniqueAbilityReadyAnimationExecuted = false;
                abilityReady.SetActive(false);
            }
               
        }
        else if (!uniqueAbilityReadyAnimationExecuted)
        {
            uniqueAbilityReadyAnimationExecuted = true;
            abilityReady.SetActive(true);
            readyAnimation?.Play();
        }

    }

    public void SetCharacterContainer()
    {
        PlayerCharacterData data = GameManager.Instance.GetCharacterData(referredCharacter.Character);

        if(data == null) return;

        abilityImage.sprite = data.UniqueAbilitySprite;

        if (right)
        {
            characterHUDbackground.sprite = data.HpContainerRight;
            characterHUDFace.sprite = data.NormalFaceRight;
            abilityReadyImage.sprite = data.AbilityReadyRight;
        }
        else
        {
            characterHUDbackground.sprite = data.HpContainerLeft;
            characterHUDFace.sprite = data.NormalFaceLeft;
            abilityReadyImage.sprite = data.AbilityReadyLeft;
        }

    }

    #region Hp
    public void SetUpHp()
    {
        maxHP.text = referredCharacter.MaxHp.ToString();
        currentHP.text = maxHP.text;
        hpBar.SetMaxValue(referredCharacter.MaxHp);
    }

    public void UpdateHp(float newHp)
    {
        currentHP.text = newHp.ToString();
        hpBar.SetValue(newHp);
    }

    #endregion

    internal void StartSwitchCooldown()
    {
        float cooldown = referredCharacter.SwitchCharacterCooldown;
        switchSlider.gameObject.SetActive(true);
        switchSlider.value = 1f;
        StartCoroutine(DisplaySwitchCooldown(cooldown));
    }

    private IEnumerator DisplaySwitchCooldown(float duration)
    {
        float elapsedTime = duration;
        float progress = 1f;

        while (progress >= 0f)
        {
            elapsedTime -= Time.deltaTime;
            progress = elapsedTime / duration;

            switchSlider.value = Mathf.Clamp01(progress);

            yield return null;
        }

        switchSlider.gameObject.SetActive(false);
    }

    public void RemoveEffect()
    {
        throw new NotImplementedException();
    }
    public void AddEffect()
    {
        throw new NotImplementedException();
    }

    public void SetUpContainer(PlayerCharacter player)
    {
        referredCharacter = player;
        SetCharacterContainer();
        SetUpHp();
        UpdateHp(referredCharacter.CurrentHp);
        StartSwitchCooldown();
        abilitySlider.value = 1f;
    }
}
