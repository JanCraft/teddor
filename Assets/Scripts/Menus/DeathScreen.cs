using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour {
    IEnumerator Start() {
        yield return new WaitForSecondsRealtime(2.5f);
        LoadingScreen.SwitchScene(1);
    }
}
