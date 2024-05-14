using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class InteractionNotification : MonoBehaviour
{
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private LocalizeStringEvent description;
    [SerializeField]
    private TextMeshProUGUI countText;
    [SerializeField]
    private float baseOffset;
    [SerializeField]
    private float activatedOffset;
    [SerializeField]
    private List<CharacterReference> characterReferences = new();

    private IInteracter firstInteracter;

    private int Count = 0;
    public void SetBackgroundSprite(Sprite sprite)
    {
        backgroundImage.sprite = sprite;
    }

    public void SetDescription(LocalizedString descriptionString)
    {
        this.description.StringReference = descriptionString;
    }

    private void SetCount()
    {
        string players = CoopManager.Instance.GetActiveHandlers().Count.ToString();

        string countText = Count.ToString() + "/" + players;
        this.countText.text = countText;
    }

    public bool AddToCount(IInteracter interacter)
    {
        Count++;
        int maxPlayers = CoopManager.Instance.GetActiveHandlers().Count;
        if (Count > maxPlayers)
            Count = maxPlayers;
        SetCount();
        SetCharaterFlag(interacter, true);
        return Count == maxPlayers;
    }

    public void RemoveFromCount(IInteracter interacter)
    {
        Count--;
        if (Count < 0)
            Count = 0;
        SetCount();
        SetCharaterFlag(interacter, false);
    }

    public void ChangeFirstInteracter(IInteracter interacter, IInteractable interactable)
    {
        if (firstInteracter == null)
            SetFirstInteracter(interacter);
        else if (firstInteracter == interacter)
            SetFirstInteracter(interactable.GetFirstInteracter());
    }

    private void SetFirstInteracter(IInteracter interacter)
    {
        firstInteracter = interacter;
        if (interacter is PlayerCharacter character)
            SetBackgroundSprite(GameManager.Instance.GetCharacterData(character.Character).NotificationBackground);
    }

    private void SetCharaterFlag(IInteracter interacter, bool state)
    {
        if (interacter is PlayerCharacter character)
        {
            foreach (CharacterReference reference in characterReferences)
            {
                if(reference.Character == character.Character)
                {
                    reference.GetComponent<RectTransform>().anchoredPosition = new Vector2(reference.GetComponent<RectTransform>().anchoredPosition.x, state ? activatedOffset : baseOffset);
                }
            }
        }
            
    }


}
