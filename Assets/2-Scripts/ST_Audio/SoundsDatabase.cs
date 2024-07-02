using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/SoundsDatabase")]
public class SoundsDatabase : ScriptableObject
{
    [ReorderableList, SerializeField]
    public List<AudioClip> hitSounds = new();
    [SerializeField] public float hitSoundsVolume = 1f;

    [ReorderableList, SerializeField]
    public List<AudioClip> attackSounds = new();
    [SerializeField] public float attackSoundsVolume = 1f;

    [ReorderableList, SerializeField]
    public List<AudioClip> walkSounds = new();
    [SerializeField] public float walkSoundsVolume = 1f;

    [ReorderableList, SerializeField]
    public List<AudioClip> deathSounds = new();
    [SerializeField] public float deathSoundsVolume = 1f;

    [ReorderableList, SerializeField]
    public List<AudioClip> dodgeSounds = new();
    [SerializeField] public float dodgeSoundsVolume = 1f;

    [ReorderableList, SerializeField]
    public List<AudioClip> healingSounds = new();
    [SerializeField] public float healingSoundsVolume = 1f;

    [ReorderableList, SerializeField]
    public List<AudioClip> blockSounds = new();
    [SerializeField] public float blockSoundsVolume = 1f;

    [ReorderableList, SerializeField]
    public List<AudioClip> specialEffectsSounds = new();
    [SerializeField] public float specialEffectsSoundsVolume = 1f;

}
