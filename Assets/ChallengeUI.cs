using TMPro;
using UnityEngine;

public class ChallengeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI challegeName;
    [SerializeField] private TextMeshProUGUI challegeDescription;
    public Challenge challengeSelected;

    public void SetUpUI()
    {
        challegeName.text = challengeSelected.challengeName;
        challegeDescription.text = challengeSelected.challengeDescription;
    }
}
