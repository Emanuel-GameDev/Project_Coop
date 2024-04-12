using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/SoundsDatabase")]
public class SoundsDatabase : ScriptableObject
{
    [ReorderableList, SerializeField]
    List<AudioClip> hitSounds = new();

    [ReorderableList, SerializeField]
    List<AudioClip> attackSounds = new();

    [ReorderableList, SerializeField]
    List<AudioClip> walkSounds = new();

    [ReorderableList, SerializeField]
    List<AudioClip> deathSounds = new();

    [ReorderableList, SerializeField]
    List<AudioClip> dodgeSounds = new();

    [ReorderableList, SerializeField]
    List<AudioClip> healingSounds = new();

    [ReorderableList, SerializeField]
    List<AudioClip> blockSounds = new();

    [ReorderableList, SerializeField]
    List<AudioClip> specialEffectsSounds = new();

}
