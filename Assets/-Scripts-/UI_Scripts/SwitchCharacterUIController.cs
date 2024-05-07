using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SwitchCharData
{
    public ePlayerCharacter character;
    public Image charIcon;
}

public class SwitchCharacterUIController : MonoBehaviour
{
    [SerializeField]
    private Color colorWhenSelected;

    [SerializeField]
    private List<SwitchCharData> switchingCharacters = new List<SwitchCharData>();

    private Dictionary<PlayerCharacter, SwitchCharData> keyValuePairs = new Dictionary<PlayerCharacter, SwitchCharData>();

    private void Start()
    {
        PubSub.Instance.RegisterFunction(EMessageType.switchingCharacters, SelectCharacter);

        StartCoroutine(Utility.WaitForPlayers(InitializeSwitchingStatus));
    }

    private void InitializeSwitchingStatus()
    {
        foreach (PlayerCharacter item in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            SwitchCharData data = switchingCharacters.Find(x => x.character == item.Character);

            keyValuePairs.Add(item, data);

            Color originalColor = Utility.HexToColor("FFFFFF");

            data.charIcon.color = originalColor;    
        }

        foreach (PlayerCharacter item in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            if (keyValuePairs.ContainsKey(item))
            {
                keyValuePairs[item].charIcon.color = colorWhenSelected;
            }
        }
    }

    private void SelectCharacter(object selectedChar)
    {
        if (selectedChar is not PlayerCharacter[]) return;
        
        PlayerCharacter[] switchingArray = selectedChar as PlayerCharacter[];

        PlayerCharacter oldChar = switchingArray[0];
        PlayerCharacter newChar = switchingArray[1];

        if (keyValuePairs.ContainsKey(oldChar))
        {
            Color originalColor = Utility.HexToColor("FFFFFF");

            keyValuePairs[oldChar].charIcon.color = originalColor;
        }
        
        if (keyValuePairs.ContainsKey(newChar))
        {
            keyValuePairs[newChar].charIcon.color = colorWhenSelected;
        }

        //if (selectedChar is PlayerCharacter)
        //{

        //    PlayerCharacter character = (PlayerCharacter)selectedChar;

        //    foreach (SwitchCharData data in switchingCharacters)
        //    {
        //        if (data.character == character.Character)
        //        {
        //            data.charIcon.color = colorWhenSelected;
        //        }
        //        else
        //        {
        //            Color originalColor = Utility.HexToColor("FFFFFF");

        //            data.charIcon.color = originalColor;
        //        }
        //    }

        //    // ciclo sugli occupati = grigio 

        //    // ciclo sui liberi = originali
        //}
        /*else */if (selectedChar is SwitchCharData)
        {
            SwitchCharData switchCharData = (SwitchCharData)selectedChar;

            switchCharData.charIcon.color = colorWhenSelected;
        }
    }
}
