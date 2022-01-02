using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security.Cryptography;
using SimpleFileBrowser;

public class SettingsMenu : MonoBehaviour {
    public Text versionTxt;
    public Text accountUsername;

    public AudioSource cashSound;

    public Slider volumeF;
    public Slider qualityI;
    public Toggle coop;

    void Start() {
        TranslateKey.Init();
        versionTxt.text = Application.platform + "::" + Application.version + "::" + Application.buildGUID;
        if (Application.buildGUID == null || Application.buildGUID == "") {
            versionTxt.text += "GUIDNULL";
        }

        if (Application.genuineCheckAvailable && !Application.genuine) {
            versionTxt.text += " [ modded ]";
        }

        accountUsername.text = PlayerPrefs.GetString("teddor.user");

        SetVolume(PlayerPrefs.GetFloat("teddor.volume", 1f));
        SetQuality(PlayerPrefs.GetInt("teddor.quality", 5));
        coop.isOn = PlayerPrefs.GetInt("teddor.coop", 0) == 1;
    }

    public void SetVolume(float volume) {
        FileBrowser.HideDialog();
        AudioListener.volume = volume;
        volumeF.value = volume;
        PlayerPrefs.SetFloat("teddor.volume", volume);
        PlayerPrefs.Save();
    }

    public void SetCoop(bool coop) {
        PlayerPrefs.SetInt("teddor.coop", coop ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetQuality(float quality) {
        FileBrowser.HideDialog();
        QualitySettings.SetQualityLevel((int)quality);
        qualityI.value = (int) quality;
        PlayerPrefs.SetInt("teddor.quality", (int) quality);
        PlayerPrefs.Save();
    }

    public void LogOut() {
        FileBrowser.HideDialog();
        StartCoroutine(_LogOut());
    }

    public void OpenWeb(string url) {
        FileBrowser.HideDialog();
        Application.OpenURL(url);
    }

    public void ImportGiftFile() {
        FileBrowser.SetFilters(true, ".tgf");
        FileBrowser.SetDefaultFilter(".tgf");
        FileBrowser.ShowLoadDialog((paths) => {
            if (paths.Length > 0) {
                var path = paths[0];
                string b64enc = System.IO.File.ReadAllText(path);
                string b64dec = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(b64enc));
                TgfRequest tr = JsonUtility.FromJson<TgfRequest>(b64dec);

                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(TgfPublicKey.RSA_PUBLIC_KEY);

                bool ver = key.VerifyData(System.Text.Encoding.UTF8.GetBytes(tr.payload), CryptoConfig.MapNameToOID("SHA256"), System.Convert.FromBase64String(tr.signature));
                if (ver) {
                    if (PlayerPrefs.GetString("teddor.gifts", "").Contains(tr.signature)) {
                        return;
                    }

                    PlayerPrefs.SetString("teddor.gifts", PlayerPrefs.GetString("teddor.gifts") + " // " + tr.signature);

                    TgfPayload tp = JsonUtility.FromJson<TgfPayload>(tr.payload);
                    if (tp.user == PlayerPrefs.GetString("teddor.user") || tp.user == "*") {
                        cashSound.Play();

                        string gamedefstr = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/gamedef.json");
                        GameDef gamedef = JsonUtility.FromJson<GameDef>(gamedefstr);

                        PlayerResourceInfo info = JsonUtility.FromJson<PlayerResourceInfo>(PlayerPrefs.GetString("teddor_save_" + gamedef.channel + "resources"));
                        info.bstars += tp.bstars;

                        PlayerPrefs.SetString("teddor_save_" + gamedef.channel + "resources", JsonUtility.ToJson(info.Verify()));
                    }
                }
            }
        }, () => { }, FileBrowser.PickMode.Files, false);
    }

    public void CloseDialog() {
        FileBrowser.HideDialog();
    }

    public static byte[] StringToByteArray(string hex) {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    private IEnumerator _LogOut() {
        UnityWebRequest www = UnityWebRequest.Get("https://game.jdev.com.es/teddor/logout?token=" + PlayerPrefs.GetString("teddor.token"));
        yield return www.SendWebRequest();

        if (www.responseCode == 200) {
            PlayerPrefs.DeleteKey("teddor.token");
            PlayerPrefs.DeleteKey("teddor.user");
            LoadingScreen.SwitchScene(0);
        }
    }

    public void ChangeLang() {
        FileBrowser.HideDialog();
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

public class TgfPublicKey {
    public static string RSA_PUBLIC_KEY = @"<RSAKeyValue><Modulus>1IXGh1jJ7nGb4bxWvIIiXIjkbFb6FvYc9nDVPa3ZwG3ansZdjSioo57SZYiCpNioEcz0YophTLzjRMY0T3v4wyTi+/jwzaIoFw4peP15FP//m1OvJa4nnfjotmseg29ojkje9NVAuVA6arW1NNYfRZgi5hcGpdXN26ptrJ237iQXgEOLoiUrSBVqjrovdnd4ICwFnQAvo6AVAbBkx7AK7NzU6ZMa4zzdQUIGO/1GWpa9wqluwZXcRpXn+NtpCUzVsZrWGvEDkGlDjoZL/uevCQWSySOPk0xGoOhAGuNw+xghapT9ENKlCQ6XFYki2LUuqdaRtRGLH0zgIW20+hK99w==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
}

[System.Serializable]
public class TgfRequest {
    public string payload;
    public string signature;
}

[System.Serializable]
public class TgfPayload {
    public string user;
    public int bstars;
    public int time;
}