using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    private static RumbleManager _instance;

    public static RumbleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RumbleManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("CoopManager");
                    _instance = singletonObject.AddComponent<RumbleManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Imposta la velocità dei motori di un controller: "lowFreq" controlla il motore SX mentre "highFreq" quello DX,
    /// "duration" rappresenta la durata della vibrazione, "pad" se omesso viene impostato a Gamepad.current 
    /// </summary>
    /// <param name="lowFreq"></param>
    /// <param name="highFreq"></param>
    /// <param name="pad"></param>
    /// <param name="duration"></param>
    public void RumblePulse(float lowFreq, float highFreq, Gamepad pad, float duration)
    {
        if (pad == null) return;

        pad.SetMotorSpeeds(lowFreq, highFreq);

        StartCoroutine(StopRumble(duration, pad));
    }

    public void RumblePulse(float lowFreq, float highfreq, float duration)
    {
        Gamepad pad = Gamepad.current;

        RumblePulse(lowFreq, highfreq, pad, duration);
    }

    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        pad.SetMotorSpeeds(0f, 0f);
    }
}
