using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/MovementTutorialFaseData")]
public class MovementTutorialFaseData : TutorialFaseData
{
    [Header("Special dialogues")]
    [Tooltip("Dialoue to play if no one moves")]
    [SerializeField] internal Dialogue specialFaseEndDialogue;

    [Header("Fase info")]
    [Tooltip("Lenght of the fase in seconds")]
    [SerializeField] internal float faseLenght = 10;
}

