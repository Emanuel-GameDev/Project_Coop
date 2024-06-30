using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHUDContainer : MonoBehaviour
{
    [SerializeField] public ePlayerID referredPlayerID;
    [SerializeField] public PlayerCharacter referredCharacter;
    ePlayerID PlayerID => referredPlayerID;


    [Header("HP & Background")]
    [SerializeField] Image characterHUDBackground;
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
    [SerializeField] Image abilityFillImage;
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

    float ExpressionDuration => HPHandler.Instance.ExpressionDuration;
    bool deathStatus = false;

    private void Update()
    {
        UniqueAbilityCooldownUpdate();
    }

    private void OnEnable()
    {
        CheckCharacterSwitchCooldown();
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

        if (data == null) return;

        abilityImage.sprite = data.UniqueAbilitySprite;
        abilityFillImage.color = data.CharacterColor;

        if (right)
        {
            characterHUDBackground.sprite = data.HpContainerRight;
            characterHUDFace.sprite = data.NormalFaceRight;
            abilityReadyImage.sprite = data.AbilityReadyRight;
        }
        else
        {
            characterHUDBackground.sprite = data.HpContainerLeft;
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
        if (newHp <= 0)
        {
            SetPlayerDead();
        }
        else if (int.TryParse(currentHP.text, out int oldHp))
        {
            if (deathStatus)
            {
                SetPlayerAlive();
            }
            
            if (newHp < oldHp)
                MakeFacialExpression(Expression.Hurt, ExpressionDuration);
            else
                MakeFacialExpression(Expression.Happy, ExpressionDuration);
        }

        currentHP.text = newHp.ToString();
        hpBar.SetValue(newHp);
    }

    #endregion

    private void CheckCharacterSwitchCooldown()
    {
        if (referredCharacter == null)
            return;

        if (Time.time - referredCharacter.LastestCharacterSwitch < referredCharacter.SwitchCharacterCooldown)
            StartCoroutine(DisplaySwitchCooldown(referredCharacter.SwitchCharacterCooldown, true));
    }

    private void StartSwitchCooldown()
    {
        float cooldown = referredCharacter.SwitchCharacterCooldown;
        switchSlider.gameObject.SetActive(true);
        switchSlider.value = 1f;
        StartCoroutine(DisplaySwitchCooldown(cooldown));
    }

    private IEnumerator DisplaySwitchCooldown(float duration, bool isRunning = false)
    {
        float remainingTime = duration;
        float progress = 1f;

        if (isRunning)
        {
            remainingTime = referredCharacter.SwitchCharacterCooldown - (Time.time - referredCharacter.LastestCharacterSwitch);
            progress = remainingTime / duration;
        }

        while (progress >= 0f)
        {
            remainingTime -= Time.deltaTime;
            progress = remainingTime / duration;

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

    public void MakeFacialExpression(Expression expression, float expressionDuration)
    {
        MakeFacialExpression(expression);
        StartCoroutine(MakeFacialExpressionAfterTime(Expression.Neutral, expressionDuration));
    }

    IEnumerator MakeFacialExpressionAfterTime(Expression expression, float expressionDuration)
    {
        yield return new WaitForSeconds(expressionDuration);
        MakeFacialExpression(expression);
    }


    public void MakeFacialExpression(Expression expression)
    {
        PlayerCharacterData data = GameManager.Instance.GetCharacterData(referredCharacter.Character);

        Sprite sprite = null;
        switch (expression)
        {
            case Expression.Neutral:
                if (right)
                    sprite = data.NormalFaceRight;
                else
                    sprite = data.NormalFaceLeft;
                break;
            case Expression.Happy:
                if (right)
                    sprite = data.HappyFaceRight;
                else
                    sprite = data.HappyFaceLeft;
                break;
            case Expression.Hurt:
                if (right)
                    sprite = data.HitFaceRight;
                else
                    sprite = data.HitFaceLeft;
                break;
            case Expression.Death:
                if (right)
                    sprite = data.DeathFaceRight;
                else
                    sprite = data.DeathFaceLeft;
                break;
        }

        if (sprite != null)
            characterHUDFace.sprite = sprite;

    }

    public void MakeDefaultFacialExpresison()
    {
        MakeFacialExpression(Expression.Neutral);
    }

    private void SetPlayerDead()
    {
        MakeFacialExpression(Expression.Death);
        deathStatus = true;
        //OtherThings
    }

    private void SetPlayerAlive()
    {
        deathStatus = false;
        MakeDefaultFacialExpresison();
        //OtherThings
    }


}

public enum Expression
{
    Neutral,
    Happy,
    Hurt,
    Death
}