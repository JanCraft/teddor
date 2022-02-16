using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusRush : MonoBehaviour {
    [Header("Enemies")]
    public GameObject[] easyPrefabs;
    public GameObject[] normalPrefabs;
    public GameObject[] hardPrefabs;

    [Header("Difficulty Doors")]
    public GameObject easyDoor;
    public GameObject normalDoor;
    public GameObject hardDoor;

    [Header("Bonus Doors")]
    public GameObject healDoor;
    public GameObject burstDoor;
    public GameObject cdDoor;
    public GameObject ssDoor;
    public GameObject shieldDoor;
    public GameObject easyChainDoor;

    [Header("Positions")]
    public Transform doorPosA;
    public Transform doorPosB;
    public Transform startPos;
    public Transform enemyPos;
    public Transform spawnPos;

    [Header("UI")]
    public GameObject brUI;
    public Text roundText;
    public Text diffText;

    private BonusRushDifficulty diff, nextdiff;
    private int round, enemiesLeft, enemiesSpawnLeft;
    private bool onRound, running, timeRunning;
    private float time, hideTime;
    private List<Enemy> enemies = new List<Enemy>();
    private PlayerController pc;

    void Start() {
        pc = FindObjectOfType<PlayerController>();
    }

    void Update() {
        if (hideTime > 0f) hideTime -= Time.deltaTime;
        brUI.SetActive(running || hideTime > 0f);
        roundText.text = round + "/" + (8 + (int) (diff - 1) * 4);
        diffText.text = diff.ToString()[0] + ". " + FormatTime(time);

        if (timeRunning) time += Time.deltaTime;

        if (onRound) {
            if (enemiesLeft <= 0) {
                onRound = false;
                round++;
                enemies.Clear();

                if (round > 8 + (int) (diff - 1) * 4) {
                    pc.Teleport(spawnPos);
                    FindObjectOfType<ResourceController>().GiveStars(5 + (int) diff * 2);
                    FindObjectOfType<ResourceController>().GiveUMatter(5 + (int) diff * 2);
                    FindObjectOfType<ResourceController>().GiveMatter(15 + (int) diff * 5);
                    running = false;
                    timeRunning = false;
                    hideTime = 2.5f;
                    return;
                }
                
                BonusRushDoor[] arr = new BonusRushDoor[] {
                    BonusRushDoor.HEAL, BonusRushDoor.HEAL,
                    BonusRushDoor.BURST, BonusRushDoor.BURST, BonusRushDoor.BURST,
                    BonusRushDoor.CD,
                    BonusRushDoor.SS, BonusRushDoor.SS, BonusRushDoor.SS,
                    BonusRushDoor.SHIELD, BonusRushDoor.SHIELD,
                    BonusRushDoor.EASY_CHAIN
                };
                BonusRushDoor da = (BonusRushDoor) arr.GetValue(Random.Range(0, arr.Length));
                BonusRushDoor db = da;
                while (db == da) {
                    db = (BonusRushDoor) arr.GetValue(Random.Range(0, arr.Length));
                }

                pc.Teleport(startPos);
                ShowDoor(da, doorPosA);
                ShowDoor(db, doorPosB);
                timeRunning = false;

                nextdiff = diff;
            } else {
                foreach (Enemy e in enemies) {
                    if (e == null) enemiesLeft--;
                }
                enemies.RemoveAll((e) => e == null);

                if (enemiesSpawnLeft > 0 && enemies.Count < 1) {
                    SpawnEnemy();
                    enemiesSpawnLeft--;
                }
            }
        }
    }

    private string FormatTime(float time) {
        return ((int) (time / 60f)).ToString().PadLeft(2, '0') + ":" + (time % 60f).ToString("00.00");
    }

    private void SpawnEnemy() {
        List<GameObject> opt = new List<GameObject>();
        if (nextdiff == BonusRushDifficulty.EASY) opt.AddRange(easyPrefabs);
        if (nextdiff == BonusRushDifficulty.NORMAL) opt.AddRange(normalPrefabs);
        if (nextdiff == BonusRushDifficulty.HARD) opt.AddRange(hardPrefabs);

        GameObject obj = Instantiate(opt[Random.Range(0, opt.Count)], enemyPos.position, Quaternion.identity);
        enemies.Add(obj.GetComponent<Enemy>());
    }

    private void ShowDoor(BonusRushDoor door, Transform pos) {
        if (door == BonusRushDoor.HEAL) {
            healDoor.SetActive(true);
            healDoor.transform.position = pos.position;
        } else if (door == BonusRushDoor.BURST) {
            burstDoor.SetActive(true);
            burstDoor.transform.position = pos.position;
        } else if (door == BonusRushDoor.CD) {
            cdDoor.SetActive(true);
            cdDoor.transform.position = pos.position;
        } else if (door == BonusRushDoor.SS) {
            ssDoor.SetActive(true);
            ssDoor.transform.position = pos.position;
        } else if (door == BonusRushDoor.SHIELD) {
            shieldDoor.SetActive(true);
            shieldDoor.transform.position = pos.position;
        } else if (door == BonusRushDoor.EASY_CHAIN) {
            easyChainDoor.SetActive(true);
            easyChainDoor.transform.position = pos.position;
        }
    }

    public void Init() {
        round = 0;
        easyDoor.SetActive(true);
        normalDoor.SetActive(true);
        hardDoor.SetActive(true);
        pc.TeleportFade(startPos);
        running = true;
        timeRunning = false;
        time = 0;
    }

    public void SetDifficulty(int diff) {
        this.diff = (BonusRushDifficulty) diff;
        nextdiff = (BonusRushDifficulty) diff;
        easyDoor.SetActive(false);
        normalDoor.SetActive(false);
        hardDoor.SetActive(false);
        onRound = true;
        enemiesLeft = 0;
        enemiesSpawnLeft = 0;
        pc.Teleport(startPos);
        timeRunning = true;
    }

    public void SetBonusDoor(int door) {
        healDoor.SetActive(false);
        burstDoor.SetActive(false);
        cdDoor.SetActive(false);
        ssDoor.SetActive(false);
        shieldDoor.SetActive(false);
        easyChainDoor.SetActive(false);
        onRound = true;
        enemiesLeft = 5 + (int) (nextdiff-1) * 5;
        enemiesSpawnLeft = 5 + (int) (nextdiff-1) * 5;
        pc.Teleport(startPos);
        timeRunning = true;

        if ((BonusRushDoor) door == BonusRushDoor.HEAL) {
            pc.HealMax();
        } else if ((BonusRushDoor) door == BonusRushDoor.BURST) {
            pc.burstModeTime = 15f;
            pc.burstModeMult = 2f;
        } else if ((BonusRushDoor) door == BonusRushDoor.CD) {
            pc.ReduceCD(1000f, 15f);
        } else if ((BonusRushDoor) door == BonusRushDoor.SS) {
            pc.soulCharge = 10000;
        } else if ((BonusRushDoor) door == BonusRushDoor.SHIELD) {
            pc.shieldValue += pc.stats.maxhp * .75f;
            pc.shieldValue = Mathf.Min(pc.shieldValue, pc.stats.maxhp);
        } else if ((BonusRushDoor) door == BonusRushDoor.EASY_CHAIN) {
            nextdiff = BonusRushDifficulty.EASY;
        }
    }
}

public enum BonusRushDifficulty {
    ZERO, EASY, NORMAL, HARD
}

public enum BonusRushDoor {
    HEAL, BURST, CD, SS, SHIELD, EASY_CHAIN
}
