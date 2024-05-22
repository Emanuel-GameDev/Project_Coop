using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum slotType
{
    Brutus,
    Kaina,
    Jude,
    Cassius,
    Dumpy,
    Lilith,
    Seven
}

public class Slot : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private slotType slotType;

    //Sprite List basato sul type
    [SerializeField] private List<Sprite> spriteDatabase;

   

    //didindi
    public Sprite Sprite 
    { 
        get => sprite;
        set => sprite = value;
    }

    public slotType Type
    {
        get => slotType;
        set { slotType = value;
            sprite = spriteDatabase[(int)value];
        }
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
