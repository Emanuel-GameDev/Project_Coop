using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Tutorial/TutorialFaseData")]
public class TutorialFaseData : ScriptableObject
{
    [SerializeField] public TutorialFaseType faseType;

    [SerializeField] public Dialogue faseStartDialogue;
    [SerializeField] public Dialogue faseEndDialogue;

}
