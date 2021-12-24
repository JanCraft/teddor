using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TinyJson;
using System.Linq;

public class WellOfDepthsScores : MonoBehaviour {
    public int interval = 60;
    public Text side;
    private float time;

    void Start() {
        StartCoroutine(Fetch());
    }
 
    void Update() {
        time += Time.deltaTime;
        if (time > interval) {
            time = 0;
            StartCoroutine(Fetch());
        }
    }

    public IEnumerator Fetch() {
        UnityWebRequest www = UnityWebRequest.Get("https://game.jdev.com.es/teddor/topscores?token=" + PlayerPrefs.GetString("teddor.token"));
        yield return www.SendWebRequest();

        Dictionary<string, int> scores = www.downloadHandler.text.FromJson<Dictionary<string, int>>();
        string[] keys = scores.Keys.ToArray();

        side.text = "<color=yellow>1. ";
        try {
            side.text += keys[0] + " (";
            side.text += scores[keys[0]] + ")";
        } catch(System.Exception) {}
        side.text += "</color>\n<color=orange>2. ";

        try {
            side.text += keys[1] + " (";
            side.text += scores[keys[1]] + ")";
        } catch (System.Exception) { }

        side.text += "</color>\n3. ";
        try {
            side.text += keys[2] + " (";
            side.text += scores[keys[2]] + ")";
        } catch (System.Exception) { }
        side.text += "\n4. ";

        try {
            side.text += keys[4] + " (";
            side.text += scores[keys[4]] + ")";
        } catch (System.Exception) { }
        side.text += "\n5. ";

        try {
            side.text += keys[5] + " (";
            side.text += scores[keys[5]] + ")";
        } catch (System.Exception) { }

        side.text += "\n\n<color>" + PlayerPrefs.GetInt("teddor.wod.score", 0) + " pts.</color>";
    }
}
