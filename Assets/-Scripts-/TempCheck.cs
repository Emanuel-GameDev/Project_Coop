using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCheck : MonoBehaviour
{
    public Transform target;
    public float angle;

   

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Vector3 forward = transform.forward;
            Vector3 toOther = target.position - transform.position;

            angle =  Vector3.Angle(forward, toOther);
            Debug.Log(angle);
           
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        Gizmos.color= Color.red;
        Gizmos.DrawLine (target.position, transform.position);
    }
}
