using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHUDContainer : MonoBehaviour
{
    [SerializeField] ePlayerID referredPlayerID;
    ePlayerID PlayerID => referredPlayerID;

    [Header("HP & Background")]
    [SerializeField] Image characterHUDImage;
    [SerializeField] Image HPBar;
    [SerializeField] TMP_Text maxHP;
    [SerializeField] TMP_Text currentHP;

    [Header("EffectCooldown")]
    [SerializeField] RectTransform effectsStartingPoint;
    [SerializeField] RectTransform effectsPoolPoint;
    [SerializeField] Vector2 fillDirection;

    public void RemoveEffect()
    {
        throw new NotImplementedException();
    }
    public void AddEffect()
    {
        throw new NotImplementedException();
    }

}
