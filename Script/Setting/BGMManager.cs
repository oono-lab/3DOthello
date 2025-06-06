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
        DontDestroyOnLoad(gameObject); // �V�[���J�ڂ��Ă��j������Ȃ��悤�ɐݒ�


        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }


        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// �w�肵��BGM���Đ����܂�
    /// </summary>
    /// <param name="newClip">�Đ�����BGM��AudioClip</param>
    public void PlayBGM(AudioClip newClip)
    {
        if (audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }

    /// <summary>
    /// BGM���~���܂�
    /// </summary>
    public void StopBGM()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }

    /// <summary>
    /// ���ݍĐ�����BGM���t�F�[�h�A�E�g���Ē�~���܂�
    /// </summary>
    /// <param name="fadeDuration">�t�F�[�h�A�E�g�̎��ԁi�b�j</param>
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
