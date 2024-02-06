using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/HealTutorialFaseData")]
public class HealTutorialFaseData : TutorialFaseData
{
    [Header("Dialoghi personaggi")]
    [SerializeField] public Dialogue DPSDialogue;
    [SerializeField] public Dialogue rangedDialogue;
    [SerializeField] public Dialogue tankDialogue;

    [Header("Dialogo speciale")]
    [SerializeField] public Dialogue specialDialogue;
}
