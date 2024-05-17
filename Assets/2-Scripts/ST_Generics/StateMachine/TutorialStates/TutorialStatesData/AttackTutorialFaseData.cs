using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/AttackTutorialFaseData")]
public class AttackTutorialFaseData : TutorialFaseData
{
    [Header("Special objective")]
    [Tooltip("Localized string for dps objective")]
    [SerializeField] internal LocalizedString faseObjectiveBrutus;

    [Header("Pre-tutorial dialogues")]
    [Tooltip("Pre-tutorial dialogues for dps")]
    [SerializeField] internal Dialogue dpsDialogue;
    [Tooltip("Pre-tutorial dialogues for healer")]
    [SerializeField] internal Dialogue healerDialogue;
    [Tooltip("Pre-tutorial dialogues for ranged")]
    [SerializeField] internal Dialogue rangedDialogue;
    [Tooltip("Pre-tutorial dialogues for tank")]
    [SerializeField] internal Dialogue tankDialogue;

    [Header("Fase info")]
    [Tooltip("Number of times to hit the enemy to pass the fase")]
    [SerializeField] internal int timesToHit = 3;
}
