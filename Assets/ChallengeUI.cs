using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class ChallengeUI : MonoBehaviour
{
    
    [SerializeField] private LocalizeStringEvent challegeDescription;
    [SerializeField] private LocalizeStringEvent challengeName;
    public Challenge challengeSelected;

    public void SetUpUI()
    {
        challengeName.StringReference = challengeSelected.challengeName;
        challegeDescription.StringReference = challengeSelected.challengeDescription;
    }
}
