using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Press : MonoBehaviour
{
    [SerializeField] GameObject leftPress;
    [SerializeField] GameObject rightPress;

    private float speed;
    private float previewTimer;
    private bool started;
    private float tempTimer = 0;
    

    internal void Activate(float speed,float previewTimer)
    {
        this.speed = speed;
        this.previewTimer = previewTimer;
        started = true;
        tempTimer = 0;
    }

    private void Update()
    {
        if (started) 
        {
            //fine preview InizioAttacco
            if(tempTimer > previewTimer)
            {
                started = false;
                
            }
            else
            {
                tempTimer +=Time.deltaTime;
            }

        }
    }
    
}
