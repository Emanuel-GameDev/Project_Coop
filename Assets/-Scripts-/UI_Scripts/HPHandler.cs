using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPHandler : MonoBehaviour
{
    [SerializeField] GameObject HPContainerLeft;
    [SerializeField] GameObject HPContainerRight;

    [SerializeField] public Transform[] HpContainerTransform = new Transform[4];



    Dictionary<ePlayerID, CharacterHUDContainer> containersAssociations;
    bool dictionaryCreated = false;

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
        if (!dictionaryCreated)
        {
            containersAssociations = new Dictionary<ePlayerID, CharacterHUDContainer>();
            dictionaryCreated = true;
        }

        PubSub.Instance.RegisterFunction(EMessageType.characterDamaged, UpdateContainer);
        PubSub.Instance.RegisterFunction(EMessageType.characterJoined, AddContainer);
        //PubSub.Instance.RegisterFunction(EMessageType.characterSwitched, SetCharacter);

    }

    public void AddContainer(object obj)
    {
        if (obj is not PlayerCharacter)
            return;

        PlayerCharacter player = (PlayerCharacter)obj;

        StartCoroutine(Wait(player));

    }

    public void RemoveLastContainer(object obj)
    {
        if (obj is not PlayerCharacter)
            return;

        PlayerCharacter player = (PlayerCharacter)obj;

        CharacterHUDContainer container;

        containersAssociations.Remove((ePlayerID)id, out container);

        Destroy(container.gameObject);
        id--;
    }

    //Da rivedere
    IEnumerator Wait(PlayerCharacter player)
    {
        yield return new WaitForSeconds(0.1f);

        if (player.characterController != null)
        {
            if (containersAssociations.ContainsKey(player.GetInputHandler().playerID))
            {
                SetCharacter(player);
                yield break;
            }

        }

        if (id < HpContainerTransform.Length)
        {
            GameObject hpContainerObject;
            if (HpContainerTransform[id].GetComponentInChildren<RewardContainer>().right)
            {
                 hpContainerObject = Instantiate(HPContainerRight, HpContainerTransform[id]);              
            }
            else
            {
                hpContainerObject = Instantiate(HPContainerLeft, HpContainerTransform[id]);         
            }


            hpContainerObject.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);

            CharacterHUDContainer hpContainer = hpContainerObject.GetComponent<CharacterHUDContainer>();
            hpContainer.right = HpContainerTransform[id].gameObject.GetComponentInChildren<RewardContainer>().right;
            hpContainer.referredCharacter = player;


            if (player.characterController != null)
            {

                containersAssociations.Add(player.GetInputHandler().playerID, hpContainer);

                Debug.Log(player.GetInputHandler().playerID);

                hpContainer.referredPlayerID = player.GetInputHandler().playerID;
            }
            else
            {
                containersAssociations.Add((ePlayerID)id + 1, hpContainer);
                hpContainer.referredPlayerID = (ePlayerID)id + 1;
            }

            hpContainer.referredCharacter = player;
            hpContainer.SetCharacterContainer(GetSpriteContainerFromCharacter(player, hpContainer.right));
            hpContainer.SetUpHp();
            hpContainer.UpdateHp(player.CurrentHp);

            //CONTROLLARE COOLDOWN ABILITA

            hpContainer.SetUpAbility(player.UniqueAbilityCooldown);

            id++;
        }

    }

    public void NotifyUseAbility(PlayerCharacter player, float cooldown)
    {
        containersAssociations[player.GetInputHandler().playerID].SetAbilityTimer(cooldown);
    }

    public void SetCharacter(object obj)
    {
        if (obj is PlayerCharacter)
        {
            PlayerCharacter playerCharacter = (PlayerCharacter)obj;

            if (containersAssociations.ContainsKey(playerCharacter.GetInputHandler().playerID))
            {
                containersAssociations[playerCharacter.GetInputHandler().playerID].referredCharacter = playerCharacter;
                containersAssociations[playerCharacter.GetInputHandler().playerID].SetCharacterContainer(GetSpriteContainerFromCharacter(playerCharacter, containersAssociations[playerCharacter.GetInputHandler().playerID].right));
                containersAssociations[playerCharacter.GetInputHandler().playerID].SetUpHp();
                containersAssociations[playerCharacter.GetInputHandler().playerID].UpdateHp(playerCharacter.CurrentHp);
                containersAssociations[playerCharacter.GetInputHandler().playerID].SetUpAbility(playerCharacter.UniqueAbilityCooldown);
            }
        }
    }

    public void UpdateContainer(object obj)
    {
        if (obj is PlayerCharacter)
        {
            PlayerCharacter playerCharacter = (PlayerCharacter)obj;

            //if (playerCharacter.characterController != null)
            //    containersAssociations[playerCharacter.GetInputHandler().playerID].UpdateHp(playerCharacter.CurrentHp);
            //else
            //{
                foreach (CharacterHUDContainer cont in gameObject.GetComponentsInChildren<CharacterHUDContainer>())
                {
                    if (cont.referredCharacter == playerCharacter)
                    {
                        containersAssociations[cont.referredPlayerID].UpdateHp(cont.referredCharacter.CurrentHp);
                        break;
                    }
                }

            //}

        }
    }

    private Sprite GetSpriteContainerFromCharacter(PlayerCharacter character, bool right)
    {
        if (right)
        {
            return GameManager.Instance.GetCharacterData(character.Character).HpContainerRight;
        }
        else
        {
            return GameManager.Instance.GetCharacterData(character.Character).HpContainerLeft;
        }


    }


}
