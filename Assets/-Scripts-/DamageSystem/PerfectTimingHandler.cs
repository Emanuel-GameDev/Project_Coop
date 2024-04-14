using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectTimingHandler : MonoBehaviour
{
    [SerializeField] GameObject alertObject;
    [SerializeField] IPerfectTimeReceiver receiver;

    private void Awake()
    {
        receiver ??= GetComponentInParent<IPerfectTimeReceiver>();
        if(receiver == null)
            Debug.LogError("No receiver set on perfect timing handler");
        else
            receiver.SetPerfectTimingHandler(this);


        DeactivateAlert();

        if(alertObject == null)
            Debug.LogError("No alert object set on perfect timing handler");
    }

    private void OnTriggerEnter(Collider other)
    {
        receiver.PerfectTimeStarted();
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
