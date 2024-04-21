using System.Collections;
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

    Dictionary<ePlayerID, CharacterHUDContainer> containersAssociations;

    int id = 0;


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


    private void OnEnable()
    {
        containersAssociations = new Dictionary<ePlayerID, CharacterHUDContainer>();
        PubSub.Instance.RegisterFunction(EMessageType.characterDamaged, UpdateContainer);
        PubSub.Instance.RegisterFunction(EMessageType.characterJoined, AddContainer);
        PubSub.Instance.RegisterFunction(EMessageType.characterSwitched, SetCharacter);
    }


    public void SetActivePlayers()
    {
        foreach (PlayerInputHandler inputHandler in GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            if (inputHandler.CurrentReceiver.GetGameObject().GetComponent<PlayerCharacter>() != null)
                AddContainer(inputHandler.CurrentReceiver.GetGameObject().GetComponent<PlayerCharacter>());
        }
    }

    public void AddContainer(object obj)
    {
        if (obj is not PlayerCharacter)
            return;

        PlayerCharacter player = (PlayerCharacter)obj;

        StartCoroutine(Wait(player));
    }

    //Da rivedere
    IEnumerator Wait(PlayerCharacter player)
    {
        yield return new WaitForSeconds(0.1f);
        
        if(player.characterController != null)
        {
            if (containersAssociations.ContainsKey(player.GetInputHandler().playerID))
            {
                SetCharacter(player);
                yield break;
            }

        }
        
        if(id < HpContainerTransform.Length)
        {
            GameObject hpContainerObject = Instantiate(HPContainer, HpContainerTransform[id]);
            hpContainerObject.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);

            CharacterHUDContainer hpContainer = hpContainerObject.GetComponent<CharacterHUDContainer>();
            hpContainer.referredCharacter = player;

            if (player.characterController != null)
            {
                containersAssociations.Add(player.GetInputHandler().playerID, hpContainer);
                hpContainer.referredPlayerID = player.GetInputHandler().playerID;
            }
            else
            {
                containersAssociations.Add((ePlayerID)id + 1, hpContainer);
                hpContainer.referredPlayerID = (ePlayerID)id + 1;
            }

            hpContainer.referredCharacter = player;
            hpContainer.SetCharacterContainer(GetSpriteContainerFromCharacter(player));
            hpContainer.SetUpHp();
            hpContainer.UpdateHp(player.CurrentHp);

            id++;
        }
        
    }

    public void SetCharacter(object obj)
    {
        if (obj is PlayerCharacter)
        {
            PlayerCharacter playerCharacter = (PlayerCharacter)obj;
            
            if (containersAssociations.ContainsKey(playerCharacter.GetInputHandler().playerID))
            {
                containersAssociations[playerCharacter.GetInputHandler().playerID].referredCharacter = playerCharacter;
                containersAssociations[playerCharacter.GetInputHandler().playerID].SetCharacterContainer(GetSpriteContainerFromCharacter(playerCharacter));
                containersAssociations[playerCharacter.GetInputHandler().playerID].SetUpHp();
                containersAssociations[playerCharacter.GetInputHandler().playerID].UpdateHp(playerCharacter.CurrentHp);
            }
        }
    }

    public void UpdateContainer(object obj)
    {
        if (obj is PlayerCharacter)
        {
            PlayerCharacter playerCharacter = (PlayerCharacter)obj;
            if (playerCharacter.characterController != null)
                containersAssociations[playerCharacter.GetInputHandler().playerID].UpdateHp(playerCharacter.CurrentHp);
            else
            {
                foreach(CharacterHUDContainer cont in gameObject.GetComponentsInChildren<CharacterHUDContainer>())
                {
                    if(cont.referredCharacter == playerCharacter)
                    {
                        containersAssociations[cont.referredPlayerID].UpdateHp(cont.referredCharacter.CurrentHp);
                        break;
                    }
                }

            }
        }
    }

    private Sprite GetSpriteContainerFromCharacter(PlayerCharacter character)
    {
        switch (character)
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
        containersAssociations.Clear();
    }
}
