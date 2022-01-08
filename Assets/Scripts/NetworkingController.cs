using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class NetworkingController : MonoBehaviour {
    public static NetworkingController instance;
    public Transform localPlayer;
    [Range(1f, 10f)]
    public float smoothing = 7.5f;

    private WebSocket ws;

    void Start() {
        instance = this;

        if (PlayerPrefs.GetInt("teddor.coop", 0) != 1) {
            enabled = false;
            return;
        }
        
        ws = new WebSocket("wss://relay.jdev.com.es/");
        ws.SslConfiguration.ServerCertificateValidationCallback = (a, b, c, d) => true;
        ws.OnOpen += (sender, e) => {
            Debug.Log("bws > connected to relay");
            ws.Send("{}");
        };
        ws.OnClose += (sender, e) => {
            Debug.Log("bws > connection cut with relay\n\n" + e.Code + ": " + e.Reason);
        };
        ws.OnError += (sender, e) => {
            Debug.LogError(e.Exception);
        };
        ws.OnMessage += (sender, e) => {
            
        };
        ws.Connect();
    }

    void Update() {
    }

    void OnDisable() {
        if (ws != null && ws.ReadyState == WebSocketState.Open) ws.Close();
    }
}
