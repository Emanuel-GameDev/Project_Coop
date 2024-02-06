using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData/HealTutorialFaseData")]
public class HealTutorialFaseData : TutorialFaseData
{
    [Header("Dialogo speciale")]
    [SerializeField] public Dialogue specialDialogue;
}
