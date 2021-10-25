using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TeddorBuilder : EditorWindow {
    public TeddorBuildChannel channel = TeddorBuildChannel.DEV;
    public int version = 1;
    public string versionStr = "v1.0.0 (dev)";

    [MenuItem("Window/Tedd'or Builder")]
    static void Init() {
        TeddorBuilder window = (TeddorBuilder) EditorWindow.GetWindow(typeof(TeddorBuilder));
        window.Show();
    }

    void OnEnable() {
        titleContent.text = "Tedd'or Buider";
        titleContent.tooltip = "Build pipeline for Tedd'or";

        LoadSettings();
    }

    void OnGUI() {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        
        GUILayout.Label("Channel");
        channel = (TeddorBuildChannel) EditorGUILayout.EnumPopup(channel);

        GUILayout.Label("Version");
        version = EditorGUILayout.IntField(version);
        versionStr = EditorGUILayout.TextField(versionStr);

        GUILayout.Label("Actions", EditorStyles.boldLabel);
        if (GUILayout.Button("Load previous settings")) LoadSettings();
        if (GUILayout.Button("Build current settings")) BuildSettings();
    }

    void LoadSettings() {
        string gamedefstr = File.ReadAllText(Application.dataPath + "/../gamedef.json");
        GameDef gamedef = JsonUtility.FromJson<GameDef>(gamedefstr);

        channel = gamedef.channel == "dev" ? TeddorBuildChannel.DEV : TeddorBuildChannel.STABLE;
        version = gamedef.version;
        versionStr = gamedef.version_str;
    }

    void BuildSettings() {

    }
}

[System.Serializable]
public class GameDef {
    public string channel;
    public int version;
    public string version_str;
}

public enum TeddorBuildChannel {
    STABLE, DEV
}