using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class AudioMixerSettings : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private TextMeshProUGUI currentVolumeLabel;

    [SerializeField]
    private TextMeshProUGUI volumeLabel;

    [SerializeField]
    private string volumeLabelText;

    [SerializeField]
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        volumeLabel.text = volumeLabelText;

        float value;
        audioMixer.GetFloat(volumeLabelText, out value);
        slider.value = Mathf.Exp(value / 20);


        UpdateOnValueChanged(slider.value);
        slider.onValueChanged.AddListener(delegate {
            UpdateOnValueChanged(slider.value);
        }); 
    }

    private void UpdateOnValueChanged(float value)
    {
        if (audioMixer != null)
            audioMixer.SetFloat(volumeLabelText, Mathf.Log(value) * 20f);
        
        if(volumeLabel != null)
            currentVolumeLabel.text = Mathf.RoundToInt(value * 100) + "%";
    }
}
