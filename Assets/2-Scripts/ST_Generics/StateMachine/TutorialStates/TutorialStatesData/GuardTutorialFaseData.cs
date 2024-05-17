using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/GuardTutorialFaseData")]
public class GuardTutorialFaseData : TutorialFaseData
{
    [Header("Pre-tutorial dialogue")]
    [Tooltip("Pre-tutorial dialogue for tank")]
    [SerializeField] internal Dialogue tankPreGuardDialogue;

    [Header("Fase info")]
    [SerializeField] internal int timesToBlock = 3;

    //potrebbero essere rimossi
    //[Tooltip("Localized string for dps objective")]
    //[SerializeField] internal LocalizedString faseObjectivePerfect;
    //[SerializeField] internal Dialogue tankPerfectGuardDialogue;
}
