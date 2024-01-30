using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/DodgeTutorialFaseData")]
public class DodgeTutorialFaseData : TutorialFaseData
{
    [Header("Dialoghi pre-tutorial schivata dei personaggi")]
    [SerializeField] public Dialogue dpsDodgeDialogue;
    [SerializeField] public Dialogue rangedDodgeDialogue;

    [Header("Dialoghi pre-tutorial schivata perfetta dei personaggi")]
    [SerializeField] public Dialogue dpsPerfectDodgeDialogue;
    [SerializeField] public Dialogue rangedPerfectDodgeDialogue;
}
