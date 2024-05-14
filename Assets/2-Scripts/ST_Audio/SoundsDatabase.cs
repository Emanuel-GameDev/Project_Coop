using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/SoundsDatabase")]
public class SoundsDatabase : ScriptableObject
{
    [ReorderableList, SerializeField]
    public List<AudioClip> hitSounds = new();

    [ReorderableList, SerializeField]
    public List<AudioClip> attackSounds = new();

    [ReorderableList, SerializeField]
    public List<AudioClip> walkSounds = new();

    [ReorderableList, SerializeField]
    public List<AudioClip> deathSounds = new();

    [ReorderableList, SerializeField]
    public List<AudioClip> dodgeSounds = new();

    [ReorderableList, SerializeField]
    public List<AudioClip> healingSounds = new();

    [ReorderableList, SerializeField]
    public List<AudioClip> blockSounds = new();

    [ReorderableList, SerializeField]
    public List<AudioClip> specialEffectsSounds = new();

}
