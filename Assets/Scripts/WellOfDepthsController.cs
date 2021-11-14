using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WellOfDepthsController : MonoBehaviour {
    public PlayerController player;

    public GameObject floorUI;
    public Text floorLevelUI;
    public Text floorRankUI;

    public int level = 0;
    public bool isInChallenge;
    public bool triggerChallenge;
    private bool loadingLevel;

    public GameObject[] enemyPool;
    private List<Enemy> activeEnemies = new List<Enemy>();
    public Transform enemySpawnPoint;

    private void Start() {
        TranslateKey.Init();
    }

    void Update() {
        floorUI.SetActive(isInChallenge);
        if (isInChallenge) {
            floorLevelUI.text = TranslateKey.Translate("ui.wod.floor") + " " + level;
            floorRankUI.text = GetRank();
        }

        if (isInChallenge) {
            activeEnemies.RemoveAll(item => item == null);
            if (activeEnemies.Count == 0 && !loadingLevel) {
                level++;
                StartCoroutine(LoadLevel());
            }
        }

        if (triggerChallenge) {
            level = player.stats.level - 2;

            triggerChallenge = false;
            isInChallenge = true;
        }
    }

    IEnumerator LoadLevel() {
        loadingLevel = true;
        LoadingScreen.FadeInOutTeleport(1f, player, enemySpawnPoint.position);

        player.stats.hp = player.stats.maxhp * .95f;
        player.ReduceCD(888.888f, 1f);
        player.soulCharge += (int) (player.stats.soulShard.ChargeMax() * .25f);

        yield return new WaitForSeconds(2.5f);

        int enemies = 2 + Mathf.Min(Mathf.FloorToInt(level / 10f), 13);
        for (int i = 0; i < enemies; i++) {
            GameObject o = Instantiate(enemyPool[Random.Range(0, enemyPool.Length)], enemySpawnPoint.position + Random.insideUnitSphere * 5f, Quaternion.identity);
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
        } else if (level < player.stats.level + 5) {
            return "B";
        } else if (level < player.stats.level + 8) {
            return "A";
        } else if (level < player.stats.level + 10) {
            return "S";
        } else if (level < player.stats.level + 12) {
            return "SS";
        } else if (level < player.stats.level + 15) {
            return "SSS";
        }
        return "SSS+";
    }
}