using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class InteractionNotification : MonoBehaviour
{
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Image chracterImage;
    [SerializeField]
    private LocalizeStringEvent description;
    [SerializeField]
    private TextMeshProUGUI countText;

    private int Count = 0;
    public void SetBackgroundSprite(Sprite sprite)
    {
        backgroundImage.sprite = sprite;
    }

    public void SetCharacterSprite(Sprite sprite)
    {
        chracterImage.sprite = sprite;
    }

    public void SetDescription(LocalizedString descriptionString)
    {
        this.description.StringReference = descriptionString;
    }

    private void SetCount()
    {
        string players = CoopManager.Instance.GetActiveHandlers().Count.ToString();

        string countText = Count.ToString() + "/" +players;
        this.countText.text = countText;
    }

    public void AddToCount()
    {
        Count++;
        SetCount();
    }

    public void RemoveFromCount()
    {
        Count--;
        SetCount();
    }
}
