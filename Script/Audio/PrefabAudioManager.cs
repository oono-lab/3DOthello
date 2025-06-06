using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabAudioManager : MonoBehaviour
{
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            SoundManager.Instance.PlaySound(clip);
        }
        else
        {
            Debug.LogError("AudioClip is null!");
        }
    }
}