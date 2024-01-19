using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<LabirintPlayer>())
        {
            other.gameObject.GetComponent<LabirintPlayer>().PickKey();
            gameObject.SetActive(false);
        }
    }
}
