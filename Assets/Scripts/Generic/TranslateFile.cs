using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

public class TranslateFile : MonoBehaviour {
    public TextAsset[] assets;

    void Awake() {
        foreach (TextAsset asset in assets) {
            var deserializer = new Deserializer();

            var p = deserializer.Deserialize<Dictionary<string, TranslateKeyObject>>(asset.text);
            if (p == null) continue;

            foreach (string key in p.Keys) {
                if (!TranslateKey.keys.ContainsKey(key))
                    TranslateKey.keys.Add(key, p[key]);
            }
        }
    }
}

[System.Serializable]
public class TranslateKeyObject {
    public string en;
    public string es;
}
