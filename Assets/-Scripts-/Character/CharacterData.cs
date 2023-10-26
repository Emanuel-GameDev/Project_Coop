using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField] protected CharacterClass characterClass;
    [SerializeField] protected AnimatorController animator;
    [SerializeField] protected float maxHP;
    [SerializeField] protected float damage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float uniqueAbilityCooldown;
    [SerializeField] protected float uniqueAbilityCooldownIncreaseAtUse;

    public float MaxHp => maxHP;
    public float Damage => damage;
    public float AttackSpeed => attackSpeed;
    public float MoveSpeed => moveSpeed;
    public float UniqueAbilityCooldown => uniqueAbilityCooldown;
    public float UniqueAbilityCooldownIncreaseAtUse => uniqueAbilityCooldownIncreaseAtUse;  
    public void Inizialize(Character character)
    {
        CharacterClass cClass = Instantiate(characterClass.gameObject, character.gameObject.transform).GetComponent<CharacterClass>();
        cClass.Inizialize(this, character);
        character.SetCharacterClass(cClass);
        character.SetAnimatorController(animator);
    }
}
