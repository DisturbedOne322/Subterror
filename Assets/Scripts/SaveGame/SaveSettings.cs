using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveSettings : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    private const string MASTER_VOLUME = "Master Volume";
    private const string EFFECTS_VOLUME = "Effects Volume";
    private const string MUSIC_VOLUME = "Music Volume";

    [SerializeField]
    private OptionsSO options;

    public void SaveSoundSettings()
    {
        SettingsSaveData data = new SettingsSaveData();
        audioMixer.GetFloat(MASTER_VOLUME, out data.masterVolume);
        audioMixer.GetFloat(EFFECTS_VOLUME, out data.effectsVolume);
        audioMixer.GetFloat(MUSIC_VOLUME, out data.musicVolume);

        data.enableBreathingSound = options.breatingEnabled;

        string settings = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/Settings.json", settings);
    }
}
