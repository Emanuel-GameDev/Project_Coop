using System.Collections.Generic;
using UnityEngine;

public class HPHandler : MonoBehaviour
{
    [SerializeField] GameObject HPContainer;

    [SerializeField] Transform[] HpContainerTransform = new Transform[4];

    [SerializeField] Sprite dpsContainerSprite;
    [SerializeField] Sprite healerContainerSprite;
    [SerializeField] Sprite rangedContainerSprite;
    [SerializeField] Sprite tankContainerSprite;

    private static HPHandler _instance;
    public static HPHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HPHandler>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("HPHandler");
                    _instance = singletonObject.AddComponent<HPHandler>();
                }
            }

            return _instance;
        }
    }


    List<PlayerCharacter> players;
    Dictionary<PlayerCharacter, CharacterHUDContainer> containersAssociations;

    private void OnEnable()
    {
        players = new List<PlayerCharacter>();
        containersAssociations = new Dictionary<PlayerCharacter, CharacterHUDContainer>();
        PubSub.Instance.RegisterFunction(EMessageType.characterDamaged, UpdateContainer);
        PubSub.Instance.RegisterFunction(EMessageType.characterJoined, AddContainer);
        PubSub.Instance.RegisterFunction(EMessageType.characterSwitched, SetCharacter);
    }


    int id = 0;
    public void SetActivePlayers()
    {
        foreach (PlayerInputHandler inputHandler in GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            if (inputHandler.CurrentReceiver.GetReceiverObject().GetComponent<PlayerCharacter>() != null)
                AddContainer(inputHandler.CurrentReceiver.GetReceiverObject().GetComponent<PlayerCharacter>());
        }
    }

    public void AddContainer(object obj)
    {
        if (obj is not PlayerCharacter)
            return;

        PlayerCharacter player = (PlayerCharacter)obj;
        players.Add(player);

        GameObject hpContainerObject = Instantiate(HPContainer, HpContainerTransform[id]);
        hpContainerObject.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);

        CharacterHUDContainer hpContainer = hpContainerObject.GetComponent<CharacterHUDContainer>();
        hpContainer.referredCharacter = players[id];
        containersAssociations.Add(players[id], hpContainer);

        hpContainer.referredPlayerID = (ePlayerID)id + 1;

        hpContainer.SetCharacterContainer(GetSpriteContainerFromClass(players[id].CharacterClass));
        hpContainer.SetUpHp();
        hpContainer.UpdateHp(players[id].CurrentHp);

        id++;



    }

    public void SetCharacter(object obj)
    {
        if (obj is PlayerCharacter)
        {
            PlayerCharacter playerCharacter = (PlayerCharacter)obj;
            if(containersAssociations.ContainsKey(playerCharacter))
                containersAssociations[playerCharacter].SetCharacterContainer(GetSpriteContainerFromClass(playerCharacter.CharacterClass));
        }
    }

    public void UpdateContainer(object obj)
    {
        if (obj is PlayerCharacter)
        {
            PlayerCharacter playerCharacter = (PlayerCharacter)obj;
            containersAssociations[playerCharacter].UpdateHp(playerCharacter.CurrentHp);
        }
    }

    private Sprite GetSpriteContainerFromClass(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case DPS:
                return dpsContainerSprite;
            case Healer:
                return healerContainerSprite;
            case Ranged:
                return rangedContainerSprite;
            case Tank:
                return tankContainerSprite;
        }

        return null;
    }

    private void OnDisable()
    {
        players.Clear();
        containersAssociations.Clear();
    }
}
