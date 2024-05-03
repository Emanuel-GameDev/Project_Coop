using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ChallengeUI : MonoBehaviour
{
    
    [SerializeField] private LocalizeStringEvent challegeDescription;
    [SerializeField] private LocalizeStringEvent challengeName;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image challengeCompletedImage;
    public Challenge challengeSelected;
    private bool selected;

    public void SetUpUI()
    {
        if (!challengeSelected.challengeCompleted)
        {
            challengeName.StringReference = challengeSelected.challengeName;
            challegeDescription.StringReference = challengeSelected.challengeDescription;
            selectButton.onClick.AddListener(MenuManager.Instance.CloseMenu);
            selectButton.onClick.AddListener(challengeSelected.ActivateGameobject);
            selectButton.onClick.AddListener(challengeSelected.Initiate);
        }
        else
        {
            challengeCompletedImage.gameObject.SetActive(true);
          
            selectButton.enabled = false;
        }
    }

}
