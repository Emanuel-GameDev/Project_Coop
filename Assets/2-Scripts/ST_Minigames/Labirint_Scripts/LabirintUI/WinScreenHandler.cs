using UnityEngine;
using UnityEngine.Localization;

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
    private WinScreenCharacterReferences firstPlaceReferences;
    [SerializeField]
    private WinScreenCharacterReferences secondPlaceReferences;
    [SerializeField]
    private WinScreenCharacterReferences thirdPlaceReferences;
    [SerializeField]
    private WinScreenCharacterReferences fourthPlaceReferences;

    public void SetCharacterValues(ePlayerCharacter character, Rank rank, int earnedCoin, int totalCoin, int earnedkey, int totalKey)
    {
        WinScreenCharacterReferences characterRef = null;

        LocalizedString stringRef = null;
        switch (rank)
        {
            case Rank.First:
                stringRef = firstPlaceString;
                characterRef = firstPlaceReferences;
                break;
            case Rank.Second:
                stringRef = secondPlaceString;
                characterRef = secondPlaceReferences;
                break;
            case Rank.Third:
                stringRef = thirdPlaceString;
                characterRef = thirdPlaceReferences;
                break;
            case Rank.Fourth:
                stringRef = fourthPlaceString;
                characterRef = fourthPlaceReferences;
                break;
        }

        if (stringRef != null)
        {
            characterRef.SetValues(stringRef, character, earnedCoin, totalCoin, earnedkey, totalKey);
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
