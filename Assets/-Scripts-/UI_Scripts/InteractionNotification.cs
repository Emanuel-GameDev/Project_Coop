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

    public void SetCount(string countText)
    {
        this.countText.text = countText;
    }
}
