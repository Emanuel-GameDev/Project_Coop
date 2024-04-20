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

    private Dictionary<IInteractable, InteractionNotification> notifications = new();

    public void ActivateNotification(IInteracter interacter, LocalizedString localizedString, IInteractable interactable)
    {
        InteractionNotification interaction;

        if (notifications.ContainsKey(interactable))
        {
            interaction = notifications[interactable];
        }
        else
        {
            GameObject newNotification = Instantiate(notificationPrefab, notificationParent);
            interaction = newNotification.GetComponent<InteractionNotification>();
            interaction.SetDescription(localizedString);
            if (interacter is PlayerCharacter character)
                interaction.SetCharacterSprite(GameManager.Instance.GetCharacterData(character.Character).DialogueSprite);
        }

        interaction.AddToCount();

        
    }

}
