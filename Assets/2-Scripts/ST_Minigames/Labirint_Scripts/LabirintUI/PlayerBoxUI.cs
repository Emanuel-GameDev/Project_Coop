using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBoxUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image background;
    [SerializeField] TMP_Text keyCount;

    public void SetIconAndBackground(Sprite newIcon, Sprite newBackground)
    {
        icon.sprite = newIcon;
        background.sprite = newBackground;
    }

    public void SetKeyCount(int count)
    {
        keyCount.text = count.ToString();
    }

}
