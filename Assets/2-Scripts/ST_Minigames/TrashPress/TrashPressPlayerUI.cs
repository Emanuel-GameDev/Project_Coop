using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TrashPressPlayerUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image background;
    [SerializeField] TMP_Text currentHp;

    public void SetIconAndBackground(Sprite newIcon, Sprite newBackground)
    {
        icon.sprite = newIcon;
        background.sprite = newBackground;
    }

    public void SetHp(int count)
    {
        currentHp.text = count.ToString();
    }
}
