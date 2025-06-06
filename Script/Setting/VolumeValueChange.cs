using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VolumeValueChange : MonoBehaviour
{
    public AudioSource targetAudioSource;
    public TextMeshProUGUI currentValueText; 
    public UnityEngine.UI.Scrollbar scrollbar;
    public string variableKey = "";

    // Start is called before the first frame update
    void Start()
    {
        
        scrollbar.onValueChanged.AddListener(delegate { OnValueChangedVolume(); });

        if (PlayerPrefs.HasKey(variableKey))
        {
            float savedValue = PlayerPrefs.GetFloat(variableKey);
            scrollbar.value = savedValue;
            targetAudioSource.volume = savedValue;
            currentValueText.text = savedValue.ToString("F1");
        }
    }

    void Update()
    {
        
        targetAudioSource.volume = scrollbar.value;
        currentValueText.text = scrollbar.value.ToString("F1");
    }

    public void OnValueChangedVolume()
    {
        PlayerPrefs.SetFloat(variableKey, scrollbar.value);
        PlayerPrefs.Save();
    }
}
