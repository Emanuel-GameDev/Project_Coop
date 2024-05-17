using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/TutorialFaseData")]
public class TutorialFaseData : ScriptableObject
{
    [SerializeField] internal TutorialFaseType faseType;

    [Header("Objective")]
    [Tooltip("LocalizedString of the fase objective")]
    [SerializeField] internal LocalizedString faseObjective;

    [Header("Dialogues")]
    [Tooltip("Dialogue at the start of the fase")]
    [SerializeField] internal Dialogue faseStartDialogue;
    [Tooltip("Dialogue at the end of the fase")]
    [SerializeField] internal Dialogue faseEndDialogue;

}
