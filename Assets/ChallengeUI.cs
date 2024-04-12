using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ChallengeUI : MonoBehaviour
{
    
    [SerializeField] private LocalizeStringEvent challegeDescription;
    [SerializeField] private LocalizeStringEvent challengeName;
    [SerializeField] private Button selectButton;
    public Challenge challengeSelected;

    public void SetUpUI()
    {
        challengeName.StringReference = challengeSelected.challengeName;
        challegeDescription.StringReference = challengeSelected.challengeDescription;
        selectButton.onClick.AddListener(MenuManager.Instance.CloseMenu);
        selectButton.onClick.AddListener(challengeSelected.ActivateGameobject);
        selectButton.onClick.AddListener(challengeSelected.Initiate);
    }
}
