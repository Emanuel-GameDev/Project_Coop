using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData")]
public class TutorialFaseData : ScriptableObject
{
    [SerializeField] public TutorialFaseType faseType;

    [SerializeField] public LocalizedString faseObjective;

    [SerializeField] public Dialogue faseStartDialogue;
    [SerializeField] public Dialogue faseEndDialogue;

}
