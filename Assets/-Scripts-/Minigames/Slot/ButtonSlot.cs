using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ButtonSlot : MonoBehaviour
{
    public GameObject Arrow;

    public void InitializeButton(Sprite buttonSprite,SpriteLibraryAsset spriteLibrary)
    {
        GetComponent<SpriteRenderer>().sprite = buttonSprite;
        GetComponent<SpriteLibrary>().spriteLibraryAsset = spriteLibrary;
    }
}
