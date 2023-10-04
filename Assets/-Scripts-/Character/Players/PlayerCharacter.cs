using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ClassCharacter
{
    tank,
    dps,
    ranged,
    healer
}
public class PlayerCharacter : Character
{
    public Rigidbody rb;
    [SerializeField] private ClassCharacter _class;
    [SerializeField] private PlayerInputSystem playerInputSystem;

   
    private void OnEnable()
    {
        rb= GetComponent<Rigidbody>();
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.Player.Enable();
        playerInputSystem.Player.Move.performed += Move_performed;
        playerInputSystem.Player.Attack.performed += Attack_performed;
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        Attack();
    }
    private void Move_performed(InputAction.CallbackContext obj)
    {
       Movement(obj.ReadValue<Vector2>());
    }

    public override void Attack()
    {
        Attack attackInfo = skillTree.GetAttackData(this) as Attack;
        foreach (PowerUp p in powerPool)
        {

            //cerca i potenziamenti d'attacco
        }
        Debug.Log(attackInfo.damage + " " + attackInfo.velocity + " " + attackInfo.ranged);
        //Play animazione attacco
    }
    public override void Defend()
    {
        skillTree.GetDefendData(this);
    }
    public override void Move()
    {
        skillTree.GetMoveData(this);
       
    }
    public void UniqueAbility()
    {
        skillTree.UseUniqueData(this);
    }

    public void Movement(Vector2 direction)
    {
        rb.velocity=new Vector3(direction.x*speed, transform.position.y, direction.y*speed).normalized;
    }



}
