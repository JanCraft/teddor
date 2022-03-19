using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
    public Text statusText;
    public bool canplay;
    public bool willplay;

    public LoginController login;

    private string statustarget = "";
    public AudioSource statusDelay;
    private string statuscurrent = "";

    private void Start() {
        TranslateKey.Init();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        AudioListener.volume = PlayerPrefs.GetFloat("teddor.volume", 1f);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("teddor.quality", 5));
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F11)) {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (statuscurrent.Length < statustarget.Length) {
            if (!statusDelay.isPlaying) {
                statuscurrent += statustarget[statuscurrent.Length];
                statusDelay.Play();
                statusDelay.pitch = Random.Range(1f, 1.5f);
            }
        }

        statusText.text = "< " + statuscurrent + " >";

        if (Input.GetKeyDown(KeyCode.Space) && canplay && !willplay && !login.open) {
            willplay = true;
            SetStatusTarget(" ");
            LoadingScreen.SwitchScene(1);
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space) && !canplay) {
            TriggerCanPlay();
            SetStatusTarget("Developer mode");
        }
#endif
    }

    public void SetStatusTarget(string target) {
        statuscurrent = "";
        statustarget = target;
    }

    public void SetStatusTargetKey(string targetKey) {
        statuscurrent = "";
        statustarget = TranslateKey.Translate(targetKey);
    }

    public void TriggerCanPlay() {
        canplay = true;
    }
}
