using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public enum ClassCharacter
{
    tank,
    dps,
    ranged,
    healer
}
public class PlayerCharacter : Character
{
    [SerializeField] private ClassCharacter _class;
    [SerializeField] private PlayerInputSystem playerInputSystem;

   
    private void OnEnable()
    {
        rb= GetComponent<Rigidbody>();
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.Player.Enable();
        playerInputSystem.Player.Attack.performed += Attack_performed;
    }

    private void Update()
    {
        Move(ReadInput());
    }

    private Vector2 ReadInput()
    {
        Vector2 moveInput = playerInputSystem.Player.Move.ReadValue<Vector2>();
        return moveInput;
   
    }


    private void Attack_performed(InputAction.CallbackContext obj)
    {
        Attack();
    }
    

    protected override void Attack()
    {
        Attack attackInfo = skillTree.GetAttackData(this) as Attack;
        foreach (PowerUp p in powerPool)
        {

            //cerca i potenziamenti d'attacco
        }
        Debug.Log(attackInfo.damage + " " + attackInfo.velocity + " " + attackInfo.ranged);
        //Play animazione attacco
    }
    protected override void Defend()
    {
        skillTree.GetDefendData(this);
    }

    public void UniqueAbility()
    {
        skillTree.UseUniqueData(this);
    }



}
