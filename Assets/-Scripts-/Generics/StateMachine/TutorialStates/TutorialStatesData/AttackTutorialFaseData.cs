using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/AttackTutorialFaseData")]
public class AttackTutorialFaseData : TutorialFaseData
{
    [SerializeField] public LocalizedString faseObjectiveBrutus;

    [Header("Dialoghi pre-tutorial dei personaggi")]
    [SerializeField] public Dialogue dpsDialogue;
    [SerializeField] public Dialogue healerDialogue;
    [SerializeField] public Dialogue rangedDialogue;
    [SerializeField] public Dialogue tankDialogue;
}
