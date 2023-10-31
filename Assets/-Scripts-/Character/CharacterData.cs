using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField, Tooltip("La classe del personaggio.")]
    protected CharacterClass characterClass;
    [SerializeField, Tooltip("Il controller dell'animazione.")]
    protected AnimatorController animator;
    [SerializeField, Tooltip("La salute massima del personaggio.")]
    protected float maxHP;
    [SerializeField, Tooltip("Il danno inflitto dal personaggio.")]
    protected float damage;
    [SerializeField, Tooltip("La velocità di attacco del personaggio.")]
    protected float attackSpeed;
    [SerializeField, Tooltip("La velocità di movimento del personaggio.")]
    protected float moveSpeed;
    [SerializeField, Tooltip("Il tempo di attesa per l'abilità unica.")]
    protected float uniqueAbilityCooldown;
    [SerializeField, Tooltip("L'incremento del tempo di attesa dell'abilità unica dopo ogni uso.")]
    protected float uniqueAbilityCooldownIncreaseAtUse;

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
