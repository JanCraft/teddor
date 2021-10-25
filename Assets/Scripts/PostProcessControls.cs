using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessControls : MonoBehaviour {
    public Volume volume;
    private LensDistortion lensdistort;
    public bool triggerLensFallback;

    void Start() {
        volume.profile.TryGet(out lensdistort);
    }

    void Update() {
        if (triggerLensFallback) {
            triggerLensFallback = false;
            StartCoroutine(LensFallback());
        }
    }

    IEnumerator LensFallback() {
        float x = 0f;

        while (x < 5f) {
            x += Time.deltaTime * 10;
            lensdistort.center.value = new Vector2(x, .5f);
            yield return null;
        }

        x *= -1;

        while (x < 0f) {
            x += Time.deltaTime * 10;
            lensdistort.center.value = new Vector2(x, .5f);
            yield return null;
        }

        lensdistort.center.value = new Vector2(0f, .5f);
    }
}
