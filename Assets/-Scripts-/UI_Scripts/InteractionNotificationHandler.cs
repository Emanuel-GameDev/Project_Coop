using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class InteractionNotificationHandler : MonoBehaviour
{
    [SerializeField] 
    private GameObject notificationPrefab;

    [SerializeField]
    private Transform notificationParent;

    public void ActivateNotification(IInteracter interacter, LocalizedString localizedString)
    {
        GameObject newNotification = Instantiate(notificationPrefab, notificationParent);
        InteractionNotification interaction = newNotification.GetComponent<InteractionNotification>();
        interaction.SetDescription(localizedString);
        if (interacter is PlayerCharacter character)
            interaction.SetCharacterSprite(GameManager.Instance.GetCharacterData(character.Character).DialogueSprite);
        interaction.SetCount("pop");
    }

}
