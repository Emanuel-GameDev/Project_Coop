using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSelection : MonoBehaviour
{
    [SerializeField]
    Image targetGraphics;

    [SerializeField]
    Sprite selectedSprite;

    Sprite defaultSprite;

    private void Awake()
    {
        defaultSprite = targetGraphics.sprite;
    }

    public void Select()
    {
        if(defaultSprite == null)
            defaultSprite = targetGraphics.sprite;

        targetGraphics.sprite = selectedSprite;
    }

    public void Deselect()
    {
        targetGraphics.sprite = defaultSprite;
    }
    
}
