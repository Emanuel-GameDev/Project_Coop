using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum slotType
{
    Player,
    OtherCharacter       //con gli altri si perde
}

public class Slot : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private slotType slotType;

   

    //didindi
    public Sprite Sprite 
    { 
        get => sprite;
        set => sprite = value;
    }

    public slotType Type
    {
        get => slotType;
        set => slotType = value;
    }


    public Slot(Sprite sprite, slotType slotType)
    {
        this.sprite = sprite;
        this.slotType = slotType;
    }

    private void Start()
    {
        gameObject.AddComponent<SpriteRenderer>().sprite=sprite;

        GetComponent<SpriteRenderer>().maskInteraction= SpriteMaskInteraction.VisibleInsideMask;
    }
}
