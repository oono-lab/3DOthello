using UnityEngine;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance; 
    private AudioSource audioSource; 

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        
        instance = this;
        DontDestroyOnLoad(gameObject); // シーン遷移しても破棄されないように設定


        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }


        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// 指定したBGMを再生します
    /// </summary>
    /// <param name="newClip">再生するBGMのAudioClip</param>
    public void PlayBGM(AudioClip newClip)
    {
        if (audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }

    /// <summary>
    /// BGMを停止します
    /// </summary>
    public void StopBGM()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }

    /// <summary>
    /// 現在再生中のBGMをフェードアウトして停止します
    /// </summary>
    /// <param name="fadeDuration">フェードアウトの時間（秒）</param>
    public void FadeOutBGM(float fadeDuration)
    {
        StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    private System.Collections.IEnumerator FadeOutCoroutine(float fadeDuration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; 
    }
}
