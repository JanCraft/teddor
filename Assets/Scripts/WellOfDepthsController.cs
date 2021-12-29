using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class WellOfDepthsController : MonoBehaviour {
    public PlayerController player;
    public WellOfDepthsScores scores;

    public GameObject floorUI;
    public Text floorLevelUI;
    public Text floorRankUI;

    public int level = 0;
    private float floorStart;
    public bool isInChallenge;
    public bool triggerChallenge;
    private bool loadingLevel;

    private int score;

    public GameObject[] enemyPool;
    private List<Enemy> activeEnemies = new List<Enemy>();
    public Transform enemySpawnPoint;
    public Transform respawnPoint;

    private void Start() {
        TranslateKey.Init();
        Debug.Log("WoD Score: " + PlayerPrefs.GetInt("teddor.wod.score", 0));
    }

    void Update() {
        floorUI.SetActive(isInChallenge);
        if (isInChallenge) {
            floorLevelUI.text = TranslateKey.Translate("ui.wod.floor") + " " + level;
            floorRankUI.text = GetRank() + "  <color>" + score + " pts.</color>";
        }

        if (isInChallenge) {
            activeEnemies.RemoveAll(item => item == null);
            if (activeEnemies.Count == 0 && !loadingLevel) {
                level++;
                score += Mathf.Max(0, 1200 - Mathf.FloorToInt((Time.time - floorStart) * Mathf.Min(40, level))) + (level * 10);
#if !UNITY_EDITOR
                if (score > PlayerPrefs.GetInt("teddor.wod.score", 0)) {
                    PlayerPrefs.SetInt("teddor.wod.score", score);

                    UnityWebRequest.Get("https://game.jdev.com.es/teddor/updatescore?score=" + score + "&token=" + PlayerPrefs.GetString("teddor.token")).SendWebRequest();
                }
#endif
                StartCoroutine(LoadLevel());
            }
        }

        if (triggerChallenge) {
            level = player.stats.level - 2;
            score = 0;

            floorStart = float.MaxValue;
            triggerChallenge = false;
            isInChallenge = true;
            player.canDie = false;
        }

        if (isInChallenge && player.hasDied) {
            player.stats.hp = player.stats.maxhp;
            player.hasDied = false;
            player.canDie = true;
            isInChallenge = false;
            foreach (Enemy e in activeEnemies) {
                if (e != null) Destroy(e.gameObject);
            }
            activeEnemies.Clear();
            scores.StartCoroutine(scores.Fetch());
            LoadingScreen.FadeInOutTeleport(1f, player, respawnPoint.position);
        }
    }

    IEnumerator LoadLevel() {
        loadingLevel = true;
        LoadingScreen.FadeInOutTeleport(1f, player, enemySpawnPoint.position);
        floorStart = Time.time;

        player.stats.hp = player.stats.maxhp * .95f;
        player.ReduceCD(888.888f, 1f);
        player.soulCharge += (int) (player.stats.soulShard.ChargeMax() * .25f);

        yield return new WaitForSeconds(2.5f);

        int enemies = 2 + Mathf.Min(Mathf.FloorToInt(level / 10f), 1);
        for (int i = 0; i < enemies; i++) {
            Vector3 pos = enemySpawnPoint.position + Random.insideUnitSphere * 5f;
            pos.y = player.transform.position.y;
            GameObject o = Instantiate(enemyPool[Random.Range(0, enemyPool.Length)], pos, Quaternion.identity);
            Enemy e = o.GetComponent<Enemy>();
            e.level = level * 2 - player.stats.level;
            e.canGrantXP = false;
            activeEnemies.Add(e);
        }

        loadingLevel = false;
    }

    public void Trigger() {
        triggerChallenge = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Trigger();
        }
    }

    string GetRank() {
        if (level < player.stats.level) {
            return "D";
        } else if (level < player.stats.level + 2) {
            return "C";
        } else if (level < player.stats.level + 4) {
            return "B";
        } else if (level < player.stats.level + 7) {
            return "A";
        } else if (level < player.stats.level + 10) {
            return "S";
        } else if (level < player.stats.level + 14) {
            return "SS";
        } else if (level < player.stats.level + 19) {
            return "SSS";
        }
        return "SSS+";
    }
}