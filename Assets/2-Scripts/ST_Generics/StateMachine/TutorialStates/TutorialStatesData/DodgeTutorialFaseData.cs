using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/DodgeTutorialFaseData")]
public class DodgeTutorialFaseData : TutorialFaseData
{
    [Header("Fase dialogues")]
    [Tooltip("Pre-Tutorial dps dialogue")]
    [SerializeField] internal Dialogue dpsDodgeDialogue;
    [Tooltip("Pre-Tutorial ranged dialogue")]
    [SerializeField] internal Dialogue rangedDodgeDialogue;

    [Header("Fase info")]
    [Tooltip("Times to dodge to complete the fase")]
    [SerializeField] internal int timesToDodge = 3;

    //forse da rimuovere
    //[SerializeField] public LocalizedString faseObjectivePerfect;
    //[Header("Dialoghi pre-tutorial schivata perfetta dei personaggi")]
    //[SerializeField] public Dialogue dpsPerfectDodgeDialogue;
    //[SerializeField] public Dialogue rangedPerfectDodgeDialogue;
}
