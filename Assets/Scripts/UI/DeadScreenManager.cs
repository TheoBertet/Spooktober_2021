using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadScreenManager : MonoBehaviour
{
    public Image blackBackground;
    public TMP_Text deadText;
    public GameObject retryBTN;
    public GameObject menuBTN;

    private void Awake()
    {
        HideScreen();
    }

    private void HideScreen()
    {
        blackBackground.color = new Color(0, 0, 0, 0);
        deadText.color = new Color(1, 1, 1, 0);
        retryBTN.SetActive(false);
        menuBTN.SetActive(false);
    }

    public void Activate()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        for(float alpha = 0; alpha <= 1; alpha += Time.deltaTime)
        {
            blackBackground.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackBackground.color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(1f);

        for (float alpha = 0; alpha <= 1; alpha += Time.deltaTime)
        {
            deadText.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        deadText.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(1f);

        retryBTN.SetActive(true);
        menuBTN.SetActive(true);
    }

    public void Retry()
    {
        GameManager.Instance.ReloadLevel();
    }

    public void BackToMenu()
    {
        GameManager.Instance.GoToMenu();
    }
}
