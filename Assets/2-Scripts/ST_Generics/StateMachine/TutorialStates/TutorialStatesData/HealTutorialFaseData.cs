using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/HealTutorialFaseData")]
public class HealTutorialFaseData : TutorialFaseData
{
    [Header("Healed dialogues")]
    [Tooltip("Dialogue to play when dps is healed")]
    [SerializeField] internal Dialogue DPSDialogue;
    [Tooltip("Dialogue to play when ranged is healed")]
    [SerializeField] internal Dialogue rangedDialogue;
    [Tooltip("Dialogue to play when tank is healed")]
    [SerializeField] internal Dialogue tankDialogue;
    [Tooltip("Dialogue to play when Dumpy is healed")]
    [SerializeField] internal Dialogue dumpyDialogue;
}
