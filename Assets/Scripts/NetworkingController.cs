using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using TinyJson;

public class NetworkingController : MonoBehaviour {
    public static NetworkingController instance;
    public PlayerController localPlayer;
    [Range(1f, 10f)]
    public float smoothing = 7.5f;
    [Range(1f, 100f)]
    public float packetRate = 75f;
    public GameObject playerPrefab;

    public Transform colliseum;

    private WebSocket ws;
    private string username;
    private float npi;
    private Dictionary<string, object> pkt = new Dictionary<string, object>();
    private Dictionary<string, NPlayerData> players = new Dictionary<string, NPlayerData>();
    private Dictionary<string, Transform> objects = new Dictionary<string, Transform>();
    private Queue<float> damage = new Queue<float>();

    void Start() {
        username = PlayerPrefs.GetString("teddor.user");
        instance = this;

        if (PlayerPrefs.GetInt("teddor.coop", 1) != 1) {
            enabled = false;
            return;
        }
        
        ws = new WebSocket("wss://relay.jdev.com.es/");
        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        ws.OnOpen += (sender, e) => {
            Debug.Log("bws > connected to relay");
            ws.Send("{\"clientID\":\"" + ByteToHexBitFiddle(System.Guid.NewGuid().ToByteArray()) + "\",\"listens\":[\"teddor:mp/move\",\"teddor:mp/damage\",\"teddor:ui/profile\",\"teddor:ui/fetch\"]}");
        };
        ws.OnClose += (sender, e) => {
            Debug.Log("bws > connection cut with relay\n\n" + e.Code + ": " + e.Reason);
        };
        ws.OnError += (sender, e) => {
            Debug.LogError("bws > error " + e.Message);
        };
        ws.OnMessage += (sender, e) => {
            Dictionary<string, object> data = e.Data.FromJson< Dictionary<string, object>>();

            if (data["event"].ToString() == "teddor:mp/move") {
                if (players.ContainsKey((string) data["name"])) {
                    players[(string) data["name"]].Update(pf(data["x"]), pf(data["y"]), pf(data["z"]), pf(data["r"]));
                } else {
                    players.Add((string) data["name"], new NPlayerData().Update(pf(data["x"]), pf(data["y"]), pf(data["z"]), pf(data["r"])));
                }
            } else if (data["event"].ToString() == "teddor:mp/damage") {
                Debug.Log(data["name"].ToString() + " " + username);
                if (data["name"].ToString() == username) {
                    Debug.Log(pf(data["dmg"]));
                    damage.Enqueue(pf(data["dmg"]));
                }
            }
        };
        ws.Connect();
    }

    void Update() {
        if (npi <= 0f) {
            pkt.Clear();
            pkt.Add("name", PlayerPrefs.GetString("teddor.user", "<unknown>"));
            pkt.Add("x", localPlayer.transform.position.x);
            pkt.Add("y", localPlayer.transform.position.y);
            pkt.Add("z", localPlayer.transform.position.z);
            pkt.Add("r", localPlayer.transform.eulerAngles.y);

            SendPacket("teddor:mp/move", pkt);
            npi = packetRate / 1000f;
        } else npi -= Time.deltaTime;

        foreach (string player in players.Keys) {
            if (objects.ContainsKey(player)) {
                objects[player].position = Vector3.Lerp(objects[player].position, players[player].pos, smoothing * Time.deltaTime);
                objects[player].eulerAngles = Vector3.up * Mathf.LerpAngle(objects[player].eulerAngles.y, players[player].rot, smoothing * Time.deltaTime);
            } else {
                GameObject obj = Instantiate(playerPrefab, players[player].pos, Quaternion.AngleAxis(players[player].rot, Vector3.up));
                obj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = player;
                obj.GetComponent<PlayerProfile>().username = player;
                objects.Add(player, obj.transform);
            }
        }

        bool candmg = Vector3.Distance(colliseum.position, localPlayer.transform.position) < 25f;
        while (damage.Count > 0) {
            float f = damage.Dequeue();
            Debug.Log(f);
            Debug.Log(candmg);
            if (candmg) localPlayer.TakeDamage(f);
        }
    }

    public void SendPacket(string packetID, Dictionary<string, object> kv) {
        kv["event"] = packetID;
        if (ws.ReadyState == WebSocketState.Open)
            ws.Send(kv.ToJson());
    }

    void OnDisable() {
        if (ws != null && ws.ReadyState == WebSocketState.Open) ws.Close();
    }

    static string ByteToHexBitFiddle(byte[] bytes) {
        char[] c = new char[bytes.Length * 2];
        int b;
        for (int i = 0; i < bytes.Length; i++) {
            b = bytes[i] >> 4;
            c[i * 2] = (char) (55 + b + (((b - 10) >> 31) & -7));
            b = bytes[i] & 0xF;
            c[i * 2 + 1] = (char) (55 + b + (((b - 10) >> 31) & -7));
        }
        return new string(c);
    }

    static float pf(object flt) {
        return float.Parse(flt.ToString(), System.Globalization.CultureInfo.InvariantCulture);
    }
}

public class NPlayerData {
    public Vector3 pos;
    public float rot;

    public NPlayerData Update(float x, float y, float z, float r) {
        pos = new Vector3(x, y, z);
        rot = r;
        return this;
    }
}
