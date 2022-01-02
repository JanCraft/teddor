using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingController : MonoBehaviour {
    public static NetworkingController instance;
    public Transform localPlayer;
    [Range(1f, 10f)]
    public float smoothing = 7.5f;

    void Start() {
        instance = this;

        if (PlayerPrefs.GetInt("teddor.coop", 0) != 1) {
            enabled = false;
            return;
        }
    }

    void Update() {
        
    }
}
