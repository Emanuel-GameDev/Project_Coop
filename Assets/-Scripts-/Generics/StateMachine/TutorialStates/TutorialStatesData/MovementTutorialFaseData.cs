using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/MovementTutorialFaseData")]
public class MovementTutorialFaseData : TutorialFaseData
{
    [SerializeField] public float faseLenght = 10;
    [SerializeField] public Dialogue specialFaseEndDialogue;
}

