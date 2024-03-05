using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBoxUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text keyCount;

    public void SetIcon(Sprite newIcon)
    {
        icon.sprite = newIcon;
    }

    public void SetKeyCount(int count)
    {
        keyCount.text = count.ToString();
    }

}
