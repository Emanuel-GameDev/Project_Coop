using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(menuName = "Character/Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField] protected CharacterClass characterClass;
    [SerializeField] protected float maxHP;
    [SerializeField] protected float damage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float uniqueAbilityCooldown;

    public float MaxHp => maxHP;
    public float Damage => damage;
    public float AttackSpeed => attackSpeed;
    public float MoveSpeed => moveSpeed;
    public float UniqueAbilityCooldown => uniqueAbilityCooldown;

    internal void Inizialize(Character character)
    {
        CharacterClass cClass = Instantiate(characterClass.gameObject, character.gameObject.transform).GetComponent<CharacterClass>();
        cClass.Inizialize(this);
        character.SetCharacterClass(cClass);
        
    }
}
