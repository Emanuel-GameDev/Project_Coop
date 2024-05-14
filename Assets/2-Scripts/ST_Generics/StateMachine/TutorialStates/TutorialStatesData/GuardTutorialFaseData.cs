using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/GuardTutorialFaseData")]
public class GuardTutorialFaseData : TutorialFaseData
{
    [SerializeField] public LocalizedString faseObjectivePerfect;
    [SerializeField] public int numberOfBlockToPass = 3;

    [Header("Dialoghi tutorial")]
    [SerializeField] public Dialogue tankPreGuardDialogue;
    [SerializeField] public Dialogue tankPerfectGuardDialogue;
}
