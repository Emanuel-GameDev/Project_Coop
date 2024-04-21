using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectTimingHandler : MonoBehaviour
{
    [SerializeField] GameObject alertObject;
    [SerializeField] IPerfectTimeReceiver receiver;
    [SerializeField] LayerMask layerMask;

    private void Awake()
    {
        receiver ??= GetComponentInParent<IPerfectTimeReceiver>();
        if(receiver == null)
            Debug.LogWarning("No receiver set on perfect timing handler");
        else
            receiver.SetPerfectTimingHandler(this);


        DeactivateAlert();

        if(alertObject == null)
            Debug.LogWarning("No alert object set on perfect timing handler");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Damager>() && Utility.IsInLayerMask(collision.gameObject, layerMask))
        {
            receiver.PerfectTimeStarted();
            Debug.Log("Perfect time started");
        }
    }

    public void ActivateAlert()
    {
        if (alertObject != null)
            alertObject.SetActive(true);
    }

    public void DeactivateAlert()
    {
        if (alertObject != null)
            alertObject.SetActive(false);
    }

}
