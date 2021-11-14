using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    public Text versionTxt;

    public Slider volumeF;
    public Slider qualityI;

    void Start() {
        TranslateKey.Init();
        versionTxt.text = Application.platform + "::" + Application.version + "::" + Application.buildGUID;
        if (Application.buildGUID == null || Application.buildGUID == "") {
            versionTxt.text += "GUIDNULL";
        }

        if (Application.genuineCheckAvailable && !Application.genuine) {
            versionTxt.text += " [ modded ]";
        }

        SetVolume(PlayerPrefs.GetFloat("teddor.volume", 1f));
        SetQuality(PlayerPrefs.GetInt("teddor.quality", 5));
    }

    public void SetVolume(float volume) {
        AudioListener.volume = volume;
        volumeF.value = volume;
        PlayerPrefs.SetFloat("teddor.volume", volume);
    }

    public void SetQuality(float quality) {
        QualitySettings.SetQualityLevel((int)quality);
        qualityI.value = (int) quality;
        PlayerPrefs.SetInt("teddor.quality", (int) quality);
    }

    public void ChangeLang() {
        string curr = TranslateKey.lang;
        string trgt = curr;
        if (curr == "en") {
            trgt = "es";
        } else if (curr == "es") {
            trgt = "en";
        }
        PlayerPrefs.SetString("teddor.lang", trgt);
        LoadingScreen.SwitchScene(3);
    }
}
