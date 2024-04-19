using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class WinScreenHandler : MonoBehaviour
{
    [SerializeField]
    private LocalizedString firstPlaceString;
    [SerializeField]
    private LocalizedString secondPlaceString;
    [SerializeField]
    private LocalizedString thirdPlaceString;
    [SerializeField]
    private LocalizedString fourthPlaceString;
    [SerializeField]
    private WinScreenCharacterReferences brutusReferences;
    [SerializeField]
    private WinScreenCharacterReferences kainaReferences;
    [SerializeField]
    private WinScreenCharacterReferences judeReferences;
    [SerializeField]
    private WinScreenCharacterReferences cassiusReferences;

    public void SetCharacterValues(ePlayerCharacter character, Rank rank, int earnedCoin, int totalCoin, int earnedkey, int totalKey)
    {
        WinScreenCharacterReferences characterRef = null;

        switch (character)
        {
            case ePlayerCharacter.Brutus:
                characterRef = brutusReferences;
                break;
            case ePlayerCharacter.Kaina:
                characterRef = kainaReferences;
                break;
            case ePlayerCharacter.Cassius:
                characterRef = cassiusReferences;
                break;
            case ePlayerCharacter.Jude:
                characterRef = judeReferences;
                break;
        }

        if (characterRef != null)
        {
            LocalizedString stringRef = null;
            switch (rank)
            {
                case Rank.First:
                    stringRef = firstPlaceString;
                    break;
                case Rank.Second:
                    stringRef = secondPlaceString;
                    break;
                case Rank.Third:
                    stringRef = thirdPlaceString;
                    break;
                case Rank.Fourth:
                    stringRef = fourthPlaceString;
                    break;
            }

            if(stringRef != null)
            {
                characterRef.SetValues(stringRef, earnedCoin, totalCoin, earnedkey, totalKey);
            }

        }
    }
}

public enum Rank
{
    First,
    Second,
    Third,
    Fourth
}
