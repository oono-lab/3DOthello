using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenFadeOut : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 10f;
    public static bool isOnline=false;
    private int OnlineScene = 1;
    private void Start()
    {
        fadeImage.gameObject.SetActive(false);
    }

    public void TransitionToScene(int sceneNumber)
    {   

        fadeImage.gameObject.SetActive(true);
        if(!isOnline) StartCoroutine(FadeOut(sceneNumber));
        else StartCoroutine(FadeOut(sceneNumber+OnlineScene));
    }
    public void isOnlineBool(bool hantei)
    {
        isOnline = hantei;
    }
    

    private IEnumerator FadeOut(int sceneNumber)
    {
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime*4;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1); 
        SceneManager.LoadScene(sceneNumber); 
    }

}
