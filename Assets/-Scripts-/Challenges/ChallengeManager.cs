using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    [SerializeField] private List<Challenge> ChallengesObjectsParents;
    private Challenge selectedChallenge;


    private void Start()
    {
        selectedChallenge = SelectChallenge();
        selectedChallenge.Initiate();
        
    }
   
    private Challenge SelectChallenge()
    {
        foreach (Challenge challenge in ChallengesObjectsParents)
        {
            challenge.gameObject.SetActive(false);
            
        }
        Challenge selectedChallenge = ChallengesObjectsParents[Random.Range(0, ChallengesObjectsParents.Count)];
        selectedChallenge.gameObject.SetActive(true);
        return selectedChallenge;
    }
}
