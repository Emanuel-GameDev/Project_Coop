using System;
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

    public Color cacca = Color.green;

    [SerializeField]
    private List<SwitchCharData> switchingCharacters = new List<SwitchCharData>();

    private Dictionary<PlayerCharacter, SwitchCharData> keyValuePairs = new Dictionary<PlayerCharacter, SwitchCharData>();

    private void Start()
    {
        StartCoroutine(Utility.WaitForPlayers(InitializeSwitchingStatus));
    }

    private void ActivateCharacter(object obj)
    {
        //foreach (PlayerCharacter item in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        //{
        //    if (keyValuePairs.ContainsKey(item))
        //    {
        //        keyValuePairs[item].charIcon.color = colorWhenSelected;
        //    }
        //}

        if (obj is PlayerCharacter)
            keyValuePairs[(PlayerCharacter)obj].charIcon.color = colorWhenSelected;

    }

    private void InitializeSwitchingStatus()
    {
        keyValuePairs.Clear();

        foreach (PlayerCharacter item in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            SwitchCharData data = switchingCharacters.Find(x => x.character == item.Character);

            keyValuePairs.Add(item, data);

            Color originalColor = Color.white;

            data.charIcon.color = originalColor;
        }

        foreach (PlayerCharacter item in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            if (keyValuePairs.ContainsKey(item))
            {
                keyValuePairs[item].charIcon.color = colorWhenSelected;
            }
        }

        PubSub.Instance.RegisterFunction(EMessageType.characterActivated, ActivateCharacter);
        PubSub.Instance.RegisterFunction(EMessageType.characterDeactivated, DeactivateCharacter);
    }

    private void DeactivateCharacter(object obj)
    {
        //if (selectedChar is not PlayerCharacter[]) return;

        //PlayerCharacter[] switchingArray = selectedChar as PlayerCharacter[];

        //PlayerCharacter oldChar = switchingArray[0];
        //PlayerCharacter newChar = switchingArray[1];

        //Debug.Log(oldChar.Character.ToString() + " into " + newChar.Character.ToString());

        //if (keyValuePairs.ContainsKey(oldChar))
        //{
        //    Color originalColor = Color.white;

        //    keyValuePairs[oldChar].charIcon.color = originalColor;
        //    Debug.Log("Resetting " + oldChar.Character.ToString() + "Color: " + keyValuePairs[oldChar].charIcon.color);
        //}

        //if (keyValuePairs.ContainsKey(newChar))
        //{
        //    keyValuePairs[newChar].charIcon.color = colorWhenSelected;
        //    Debug.Log("Setting " + newChar.Character.ToString());
        //}

        if (obj is PlayerCharacter)
            keyValuePairs[(PlayerCharacter)obj].charIcon.color = Color.white;
    }
}
