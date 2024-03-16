using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHUDContainer : MonoBehaviour
{
    [SerializeField] public ePlayerID referredPlayerID;
    [SerializeField] public PlayerCharacter referredCharacter;
    ePlayerID PlayerID => referredPlayerID;


    [Header("HP & Background")]
    //[SerializeField] Image characterHUDImage;
    //[SerializeField] Image HPBar;
    [SerializeField] Image playerIdImage;
    [SerializeField] TMP_Text maxHP;
    [SerializeField] TMP_Text currentHP;



    [Header("EffectCooldown")]
    [SerializeField] RectTransform effectsStartingPoint;
    [SerializeField] RectTransform effectsPoolPoint;
    [SerializeField] Vector2 fillDirection;

    public void SetCharacterContainer(Sprite containerSprite)
    {
        GetComponent<Image>().sprite = containerSprite; 
    }

    public void SetUpHp()
    {
        maxHP.text = referredCharacter.MaxHp.ToString();
        currentHP.text = maxHP.text;
    }

    public void UpdateHp(float newHp)
    {
        currentHP.text = newHp.ToString();
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
