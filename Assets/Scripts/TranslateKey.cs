using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslateKey : MonoBehaviour {
    public static string lang = "en";
    public string key = "";
    private Text text;

    public static Dictionary<string, TranslateKeyObject> keys = new Dictionary<string, TranslateKeyObject>();

    public static void Init() {
        lang = PlayerPrefs.GetString("teddor.lang", "en");
    }

    void Start() {
        Init();
        text = GetComponent<Text>();
    }

    void Update() {
        text.text = Translate(key);
    }

    public static string Translate(string key) {
        if (keys.ContainsKey(key)) return lang == "es" ? keys[key].es : keys[key].en;

        return "<???>";
    }
}
