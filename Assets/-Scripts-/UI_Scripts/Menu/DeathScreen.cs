using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeInBackgroundTime = 1f;
    [SerializeField] private float waitTimeBeforeFadeInText = 1f;
    [SerializeField] private float fadeInTextTime = 1f;
    [SerializeField] private float waitTimeBeforeRetryMenu = 1f;
    [SerializeField] private string exitSceneName;
    
    [Header("References")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private MenuInfo retryMenu;

    public void ShowDeathScreen()
    {
       gameObject.SetActive(true);
        StartCoroutine(FadeInBackgroundAndText());
    }

    public void OpenConfirmationMenu()
    {
        MenuManager.Instance.OpenMenu(retryMenu, CoopManager.Instance.GetFirstPlayer());
    }

    IEnumerator FadeInBackgroundAndText()
    {
        float timer = 0f;

        Color initialBackgroundColor = background.color;
        Color initialTextColor = deathText.color;

        while (timer < fadeInBackgroundTime || timer < fadeInTextTime + waitTimeBeforeFadeInText)
        {
            timer += Time.deltaTime;
            float backgroundNormalizedTime = Mathf.Clamp01(timer / fadeInBackgroundTime);
            float textNormalizedTime = Mathf.Clamp01(timer - waitTimeBeforeFadeInText / fadeInTextTime);

            background.color = Color.Lerp(Color.clear, initialBackgroundColor, backgroundNormalizedTime);
            deathText.color = Color.Lerp(Color.clear, initialTextColor, textNormalizedTime);

            yield return null;
        }

        yield return new WaitForSeconds(waitTimeBeforeRetryMenu);
        OpenConfirmationMenu();

    }

    public void Retry()
    {
        GameManager.Instance.ChangeScene(SceneManager.GetActiveScene().name);
    }

    public void QuitScene()
    {
        GameManager.Instance.ChangeScene(exitSceneName);
    }

}
