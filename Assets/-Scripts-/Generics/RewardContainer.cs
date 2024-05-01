using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardContainer : MonoBehaviour
{
    [Header("Transition")]
    
    
    [SerializeField] public Transform targetPosition;
    
    [SerializeField] public bool right;

    [HideInInspector] public GameObject rewardPopUp;

  
    public IEnumerator MoveAndFadeRoutine()
    {
        float moveDuration = RewardManager.Instance.moveDuration;
        float popUpDuration = RewardManager.Instance.popUpDuration;

        Vector3 initialPosition = rewardPopUp.transform.position;       
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            rewardPopUp.transform.position = Vector3.Lerp(initialPosition, transform.position, elapsedTime / moveDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }     
        rewardPopUp.transform.position = transform.position;
       

        yield return new WaitForSeconds(popUpDuration);

         initialPosition = transform.position;
        //Color initialColor = spriteRenderer.color;

         elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            rewardPopUp.transform.position = Vector3.Lerp(initialPosition, targetPosition.position, elapsedTime / moveDuration);
            //spriteRenderer.color = Color.Lerp(initialColor, Color.clear, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and color
        rewardPopUp.transform.position = targetPosition.position;
        // spriteRenderer.color = Color.clear;

        // Optionally destroy or deactivate the GameObject after fading out
        gameObject.SetActive(false);
    }

}
