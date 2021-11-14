using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public class TeddorBuilder : EditorWindow {
    public TeddorBuildChannel channel = TeddorBuildChannel.DEV;
    public int version = 1;
    public string versionStr = "v1.0.0 (dev)";

    [MenuItem("Window/Tedd'or Builder")]
    public static void Init() {
        TeddorBuilder window = (TeddorBuilder) GetWindow(typeof(TeddorBuilder));
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

        EditorGUILayout.HelpBox(new GUIContent("The button above will perform a build for StandaloneWindows64, StandaloneLinux64 and StandaloneOSX. This process is quite slow."));
    }

    void LoadSettings() {
        string gamedefstr = File.ReadAllText(Application.streamingAssetsPath + "/gamedef.json");
        GameDef gamedef = JsonUtility.FromJson<GameDef>(gamedefstr);

        channel = gamedef.channel == "dev" ? TeddorBuildChannel.DEV : TeddorBuildChannel.STABLE;
        version = gamedef.version;
        versionStr = gamedef.version_str;
    }

    void BuildSettings() {
        Debug.ClearDeveloperConsole();

        GameDef def = new GameDef();
        def.channel = channel.ToString().ToLowerInvariant();
        def.version = version;
        def.version_str = versionStr;
        File.WriteAllText(Application.streamingAssetsPath + "/gamedef.json", JsonUtility.ToJson(def, true));

        PlayerSettings.bundleVersion = versionStr;

        BuildFor(BuildTarget.StandaloneWindows64, "Win64/Teddor.exe", "Win64");
        BuildFor(BuildTarget.StandaloneLinux64, "Linux64/Teddor", "Linux64");
        BuildFor(BuildTarget.StandaloneOSX, "MacOSX/Teddor", "MacOSX");
    }

    private void BuildFor(BuildTarget target, string locpath, string platf) {
        BuildPlayerOptions opt = new BuildPlayerOptions();
        opt.locationPathName = Application.dataPath + "/../Builds/" + locpath;
        opt.target = target;
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        opt.scenes = scenes;
        BuildReport rep = BuildPipeline.BuildPlayer(opt);
        if (rep.summary.result == BuildResult.Succeeded) {
            int seconds = (int) rep.summary.totalTime.TotalSeconds;
            string platform = rep.summary.platform.ToString();
            int sizemb = (int) (rep.summary.totalSize / 1024 / 1024);

            using (ZipOutputStream s = new ZipOutputStream(File.Create(Application.dataPath + "/../Builds/Teddor" + platf + ".zip"))) {
                string[] filenames = Directory.GetFiles(Application.dataPath + "/../Builds/" + platf + "/", "*", SearchOption.AllDirectories);
                byte[] buffer = new byte[4096];
                foreach (string file in filenames) {
                    string filen = file.Substring((Application.dataPath + "/../Builds/" + platf + "/").Length);
                    ZipEntry entry = new ZipEntry(filen);
                    entry.DateTime = System.DateTime.Now;
                    s.PutNextEntry(entry);
                    using (FileStream fs = File.OpenRead(file)) {
                        int sourceBytes;
                        do {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                s.Finish();
                s.Close();
            }

            Debug.Log("Build finished in " + seconds + "s, for platform " + platform + ", total size " + sizemb + "MB");
        } else {
            Debug.Log("Build for platform " + rep.summary.platform + " failed!");
        }
    }
}