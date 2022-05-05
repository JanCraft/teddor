using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {
    public static LoadingScreen instance;
    public Image img;

    void Start() {
        instance = this;
        img.color = Color.black;
        StartCoroutine(fade(1f, 0f, 1f));
    }

    IEnumerator fade(float from, float to, float time) {
        float tt = 0f;

        while (tt < 1f) {
            tt += Time.deltaTime / time;
            img.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, tt));
            yield return null;
        }

        tt = 1f;
        img.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, tt));
    }

    IEnumerator _SwitchScene(int buildIndex) {
        yield return fade(0f, 1f, 1f);
        SceneManager.LoadSceneAsync(buildIndex);
    }

    IEnumerator _QuitGame() {
        yield return fade(0f, 1f, 1f);
        Application.Quit();
    }

    public static void SwitchScene(int buildIndex) {
        instance.StartCoroutine(instance._SwitchScene(buildIndex));
    }

    public static void QuitGame() {
        instance.StartCoroutine(instance._QuitGame());
    }

    IEnumerator _FadeInOut(float totalTime) {
        yield return fade(0f, 1f, totalTime / 2f);
        yield return fade(1f, 0f, totalTime / 2f);
    }

    IEnumerator _FadeInOutTeleport(float totalTime, PlayerMovement pc, Vector3 loc) {
        yield return fade(0f, 1f, totalTime / 2f);
        pc.Teleport(loc);
        yield return fade(1f, 0f, totalTime / 2f);
    }

    public static void FadeInOut(float totalTime) {
        instance.StartCoroutine(instance._FadeInOut(totalTime));
    }

    public static void FadeInOutTeleport(float totalTime, PlayerMovement pc, Vector3 loc) {
        instance.StartCoroutine(instance._FadeInOutTeleport(totalTime, pc, loc));
    }
}
