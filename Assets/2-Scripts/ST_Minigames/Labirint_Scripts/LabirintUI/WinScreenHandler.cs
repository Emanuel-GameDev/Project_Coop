using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class WinScreenHandler : MonoBehaviour
{
    [SerializeField]
    private WinScreenCharacterReferences firstPlaceReferences;
    [SerializeField]
    private WinScreenCharacterReferences secondPlaceReferences;
    [SerializeField]
    private WinScreenCharacterReferences thirdPlaceReferences;
    [SerializeField]
    private WinScreenCharacterReferences fourthPlaceReferences;
    [SerializeField]
    private List<PlaceData> placeData = new();

    public void SetCharacterValues(ePlayerID playerID, ePlayerCharacter character, Rank rank, int earnedCoin, int totalCoin, int earnedkey, int totalKey)
    {
        WinScreenCharacterReferences characterRef = null;

        PlaceData dataRef = GetPlaceData(rank);
        switch (rank)
        {
            case Rank.First:
                characterRef = firstPlaceReferences;
                break;
            case Rank.Second:
                characterRef = secondPlaceReferences;
                break;
            case Rank.Third:
                characterRef = thirdPlaceReferences;
                break;
            case Rank.Fourth:
                characterRef = fourthPlaceReferences;
                break;
        }

        if (dataRef != null)
        {
            characterRef.SetValues(dataRef.placeString, dataRef.medalImage, character, playerID, earnedCoin, totalCoin, earnedkey, totalKey);
        }


    }

    private PlaceData GetPlaceData(Rank rank)
    {
        foreach (PlaceData data in placeData)
        {
            if (data.rank == rank)
            {
                return data;
            }
        }
        return null;
    }
}

[Serializable]
public class PlaceData
{
    public Rank rank;
    public LocalizedString placeString;
    public Sprite medalImage;

}


public enum Rank
{
    First,
    Second,
    Third,
    Fourth
}
