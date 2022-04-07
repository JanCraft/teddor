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
    private bool startOnly = false;

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
            if (activeEnemies.Count <= 1 && !loadingLevel) {
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
            level = player.combat.stats.level - 2;
            score = 0;

            floorStart = float.MaxValue;
            triggerChallenge = false;
            isInChallenge = true;
            player.combat.canDie = false;
            startOnly = true;
            LoadingScreen.FadeInOutTeleport(.25f, player.movement, enemySpawnPoint.position);
        }

        if (isInChallenge && player.combat.hasDied) {
            player.combat.stats.hp = player.combat.stats.maxhp;
            player.combat.hasDied = false;
            player.combat.canDie = true;
            isInChallenge = false;
            foreach (Enemy e in activeEnemies) {
                if (e != null) Destroy(e.gameObject);
            }
            activeEnemies.Clear();
            scores.StartCoroutine(scores.Fetch());
            LoadingScreen.FadeInOutTeleport(1f, player.movement, respawnPoint.position);
        }
    }

    IEnumerator LoadLevel() {
        loadingLevel = true;
        floorStart = Time.time;

        player.combat.healHP += .33f * player.combat.stats.maxhp;
        player.abilities.ReduceCD(1f, .5f);
        player.abilities.soulCharge += (int) (player.combat.stats.soulShard.ChargeMax() * .1f);

        if (startOnly) {
            yield return new WaitForSeconds(2.5f);
            startOnly = false;
        }
        yield return new WaitForSeconds(.1f);

        int enemies = 2 + Mathf.Min(Mathf.FloorToInt(level / 10f), 1);
        for (int i = 0; i < enemies; i++) {
            Vector3 pos = enemySpawnPoint.position + Random.insideUnitSphere * 5f;
            pos.y = player.transform.position.y;
            GameObject o = Instantiate(enemyPool[Random.Range(0, enemyPool.Length)], pos, Quaternion.identity);
            Enemy e = o.GetComponent<Enemy>();
            e.level = level * 2 - player.combat.stats.level;
            e.canGrantStars = false;
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
        if (level < player.combat.stats.level) {
            return "D";
        } else if (level < player.combat.stats.level + 4) {
            return "C";
        } else if (level < player.combat.stats.level + 8) {
            return "B";
        } else if (level < player.combat.stats.level + 14) {
            return "A";
        } else if (level < player.combat.stats.level + 20) {
            return "S";
        } else if (level < player.combat.stats.level + 28) {
            return "SS";
        } else if (level < player.combat.stats.level + 38) {
            return "SSS";
        }
        return "SSS+";
    }
}