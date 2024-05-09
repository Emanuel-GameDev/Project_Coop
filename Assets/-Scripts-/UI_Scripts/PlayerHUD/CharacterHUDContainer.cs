using System;
using System.Collections;
using System.IO.MemoryMappedFiles;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CharacterHUDContainer : MonoBehaviour
{
    [SerializeField] public ePlayerID referredPlayerID;
    [SerializeField] public PlayerCharacter referredCharacter;
    ePlayerID PlayerID => referredPlayerID;


    [Header("HP & Background")]
    [SerializeField] Image characterHUDImage;
    //[SerializeField] Image HPBar;
    [SerializeField] Image playerIdImage;
    [SerializeField] TMP_Text maxHP;
    [SerializeField] TMP_Text currentHP;
    [SerializeField] GenericBarScript hpBar;
    [SerializeField] public bool right;

    [Header("Ability")]
    [SerializeField] Image abilityImage;
    [SerializeField] TMP_Text abilityCooldownText;
    [SerializeField] Image abilityCooldownBackground;
    
    private float abilityCooldownValue;
    private float cooldownTimer;
    private bool abilityUsed;


    [Header("EffectCooldown")]
    [SerializeField] RectTransform effectsStartingPoint;
    [SerializeField] RectTransform effectsPoolPoint;
    [SerializeField] Vector2 fillDirection;

    [Header("SwitchingCooldown")]
    [SerializeField] private Slider slider;

    private void Update()
    {
        if (abilityUsed)
        {
            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                SetAbilityUsed(false);
               
            }

            else
            {
                int cooldownInt = (int)cooldownTimer;
                abilityCooldownText.text = cooldownInt.ToString();
                cooldownTimer -= Time.deltaTime; 
                

            }
        }
    }
    public void SetCharacterContainer(Sprite containerSprite)
    {
        characterHUDImage.sprite = containerSprite;
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
    #region ability

    public void SetUpAbility(float abilityTimerValue)
    {
        if (GameManager.Instance.GetCharacterData(referredCharacter.Character).UniqueAbilitySprite != null)
        {
            abilityImage.sprite = GameManager.Instance.GetCharacterData(referredCharacter.Character).UniqueAbilitySprite;
        }

        SetAbilityUsed(false);
        abilityCooldownValue = abilityTimerValue;
        cooldownTimer = abilityCooldownValue;
        abilityCooldownText.text = abilityCooldownValue.ToString();


    }
    public void SetAbilityTimer(float newCooldownValue)
    {
        abilityCooldownValue = newCooldownValue;
        cooldownTimer = abilityCooldownValue;
        SetAbilityUsed(true);


    }
    public void SetAbilityUsed(bool value)
    {


        abilityUsed = value;
        abilityCooldownBackground.gameObject.SetActive(value);
        abilityCooldownText.gameObject.SetActive(value);

    }

    #endregion

    // Durante

    internal void StartSwitchCooldown(float cooldown)
    {
        slider.gameObject.SetActive(true);
        slider.value = 1f;
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

            slider.value = Mathf.Clamp01(progress);
            
            yield return null;
        }

        slider.gameObject.SetActive(false);

        Debug.Log("Finecooldown");
    }

    public void RemoveEffect()
    {
        throw new NotImplementedException();
    }
    public void AddEffect()
    {
        throw new NotImplementedException();
    }

}
