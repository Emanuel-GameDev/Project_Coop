using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class InteractionNotificationHandler : MonoBehaviour
{
    private static InteractionNotificationHandler _instance;
    public static InteractionNotificationHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InteractionNotificationHandler>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("InteractionNotificationHandler");
                    _instance = singletonObject.AddComponent<InteractionNotificationHandler>();
                }
            }

            return _instance;
        }
    }


    [SerializeField] 
    private GameObject notificationPrefab;

    [SerializeField]
    private Transform notificationParent;

    private Dictionary<IInteractable, InteractionNotification> notifications = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void ActivateNotification(IInteracter interacter, LocalizedString localizedString, IInteractable interactable)
    {
        InteractionNotification interaction = null;

        if (notifications.ContainsKey(interactable))
        {
            interaction = notifications[interactable];
        }
        else if(notificationPrefab != null && notificationParent != null)
        {
            GameObject newNotification = Instantiate(notificationPrefab, notificationParent);
            newNotification.SetActive(true);

            interaction = newNotification.GetComponent<InteractionNotification>();
            interaction.SetDescription(localizedString);
            interaction.ChangeFirstInteracter(interacter, interactable);

            notifications.Add(interactable, interaction);
        }

        if(interaction != null)
            interaction.AddToCount();

    }

    public void CancelNotification(IInteracter interacter, IInteractable interactable)
    {
        if (notifications.ContainsKey(interactable))
        {
            notifications[interactable].RemoveFromCount();
            notifications[interactable].ChangeFirstInteracter(interacter, interactable);
        }
    }

    public void DeactivateNotification(IInteractable interactable)
    {
        if (notifications.ContainsKey(interactable))
        {
            Destroy(notifications[interactable].gameObject);
            notifications.Remove(interactable);
        }
    }


}
