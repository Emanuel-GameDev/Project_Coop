using UnityEngine;

public class AbilityMenuButton : MonoBehaviour
{
    [SerializeField]
    private GameObject abilityTent;

    public void Activate()
    {
        abilityTent.SetActive(true);
    }

    public void Deactivate()
    {
        abilityTent.SetActive(false);
    }

}
