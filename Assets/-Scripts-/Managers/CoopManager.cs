using UnityEngine;

public class CoopManager : MonoBehaviour
{
    [SerializeField] private CharacterData switchPlayerUp;
    [SerializeField] private CharacterData switchPlayerRight;
    [SerializeField] private CharacterData switchPlayerDown;
    [SerializeField] private CharacterData switchPlayerLeft;

    public void SwitchCharacter(Character characterToSwitch, int switchInto)
    {
        switch (switchInto)
        {
            case 0:
                characterToSwitch.SetCharacterData(switchPlayerUp);
                break;
            case 1:
                characterToSwitch.SetCharacterData(switchPlayerRight);
                break;
            case 2:
                characterToSwitch.SetCharacterData(switchPlayerDown);
                break;
            case 3:
                characterToSwitch.SetCharacterData(switchPlayerLeft);
                break;
        }
    }
}
