using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Challenge> challengesList;
    [SerializeField] private List<GameObject> challengePanelList;
    [SerializeField] private GameObject challengeUIPrefab;
    private Challenge selectedChallenge;
    private List<Challenge> tempList;
    private int index1;
    private int index2;
    private int index3;


    private void OnEnable()
    {
        index1 = Random.Range(0, challengesList.Count-1);
        tempList.Add(challengesList[index1]);

        index2 = Random.Range(0, challengesList.Count - 1);
        while (index2 == index1) 
        {
            index2 = Random.Range(0, challengesList.Count - 1);
        }
        tempList.Add(challengesList[index2]);

        index3 = Random.Range(0, challengesList.Count - 1);
        while (index3 == index2 || index3 == index1)
        {
            index3 = Random.Range(0, challengesList.Count - 1);
        }
        tempList.Add(challengesList[index3]);

        for(int i = 0; i < challengePanelList.Count; i++) 
        {
            GameObject cObj = Instantiate(challengeUIPrefab, challengePanelList[i].transform);
            ChallengeUI cUI = cObj.GetComponent<ChallengeUI>();
            cUI.challengeSelected = tempList[i];
           
        }
        
    }

    private Challenge SelectChallenge()
    {
        foreach (Challenge challenge in challengesList)
        {
            challenge.gameObject.SetActive(false);

        }
        Challenge selectedChallenge = challengesList[Random.Range(0, challengesList.Count)];
        selectedChallenge.gameObject.SetActive(true);
        return selectedChallenge;
    }

    public void Interact(IInteracter interacter)
    {
        ActiatePanels(true);
        //selectedChallenge = SelectChallenge();
        //selectedChallenge.Initiate();
    }

    private void ActiatePanels(bool activate)
    {
        foreach (GameObject panel in challengePanelList)
        {
            panel.SetActive(activate);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.EnableInteraction(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.DisableInteraction(this);
        }
    }
}
