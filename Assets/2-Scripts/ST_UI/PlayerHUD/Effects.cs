using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Effects : MonoBehaviour
{
    [SerializeField] Image effectImage;
    [SerializeField] TMP_Text textNumbers;
    [SerializeField] bool hasCooldown = false;
    [SerializeField] float cooldownTime = 0;

    private bool cooldownIsActive = true;
    private float timer = 0;
    private CharacterHUDContainer container;

    public void Update()
    {
        if(hasCooldown && cooldownIsActive)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                DismissEffect();
            }
        }
    }

    public void Inizialize(CharacterHUDContainer container)
    {
        this.container = container;
    }

    public void SetCooldownTime(float time)
    {
        cooldownTime = time;
        timer = time;
    }

    public void StartCooldown()
    {
        cooldownIsActive = true;
        timer = cooldownTime;
    }

    private void DismissEffect()
    {
        container.RemoveEffect();
    }
}
