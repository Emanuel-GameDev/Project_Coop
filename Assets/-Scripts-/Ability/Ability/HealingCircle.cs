using UnityEngine;

[CreateAssetMenu(menuName = "Ability/HealingCircle")]
public class HealingCircle : Ability
{
    public float heal;
    public float radius;
    public GameObject prefab;


    public override void Use(MonoBehaviour parent)
    {
        PlaceHeal();
    }

    private void PlaceHeal()
    {
       
    }
}
