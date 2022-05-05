using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.IO;

public class UpdateChecker : MonoBehaviour {
    public float delay = 0f;
    public UnityEvent onUpToDate;
    public UnityEvent onUpdateFound;
    public UnityEvent onRequestFailed;
    public UnityEvent onChannelMismatch;
    public UnityEvent onMaintenance;

    IEnumerator Start() {
        TranslateKey.Init();
        yield return new WaitForSecondsRealtime(delay);
        string gamedefstr = File.ReadAllText(Application.streamingAssetsPath + "/gamedef.json");
        GameDef gamedef = JsonUtility.FromJson<GameDef>(gamedefstr);

        string maintenanceurl = "https://raw.githubusercontent.com/jdev-com-es/jdev.com.es/main/maintenance.json";
        UnityWebRequest mwww = UnityWebRequest.Get(maintenanceurl);
        yield return mwww.SendWebRequest();
        var mobj = TinyJson.JSONParser.FromJson<Maintenance>(mwww.downloadHandler.text);
        if (mobj.enabled) {
            onMaintenance.Invoke();
            yield break;
        } else if (mobj.start >= (int) System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds) {
            onMaintenance.Invoke();
            yield break;
        }

        string checkurl = "https://raw.githubusercontent.com/JanCraft/teddor/" + GetGitBranch(gamedef.channel) + "/Assets/StreamingAssets/gamedef.json";

        UnityWebRequest www = UnityWebRequest.Get(checkurl);
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success) {
            GameDef remote = JsonUtility.FromJson<GameDef>(www.downloadHandler.text);
            if (remote.channel == gamedef.channel) {
                if (remote.version > gamedef.version) {
                    Debug.Log("Update found!");
                    onUpdateFound.Invoke();
                } else {
                    Debug.Log("Client up to date!");
                    onUpToDate.Invoke();
                }
            } else {
                Debug.Log("Update request channel mismatch found!");
                onChannelMismatch.Invoke();
            }
        } else {
            Debug.Log("Update request failed!");
            onRequestFailed.Invoke();
        }
    }

    string GetGitBranch(string channel) {
        if (channel == "dev") return "dev";
        return "main";
    }
}

// These utility classes are used by the editor too. dont touch.
[System.Serializable]
public class GameDef {
    public string channel;
    public int version;
    public string version_str;
}

[System.Serializable]
public class Maintenance {
    public bool enabled;
    public string reason;
    public long start;
    public long end;
}

public enum TeddorBuildChannel {
    STABLE, DEV
}