using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
   public DialogueBox dialogueBox;
    public abstract void Initiate();
    public abstract void StartChallenge();

}
