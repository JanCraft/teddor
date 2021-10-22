using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    public Direction direction;
    public float speed = 5f;
    public Sprite[] sprites;

    [Header("Effects")]
    public GameObject slashPrefab;
    public GameObject burstSlashPrefab;

    private float animprog = 0f;
    private bool animrun = false;
    private SpriteRenderer sr;
    private Vector2 target = Vector2.zero;

    [Header("Stats")]
    public PlayerStats stats;
    public float shieldValue = 0f;
    public Text lvltxt;
    public Transform hpbar;
    public Transform hpdmgbar;

    void Start() {
        sr = GetComponent<SpriteRenderer>();

        stats.Calculate();
    }

    void Update() {
        sr.sprite = sprites[(int) direction * 3 + GetAnimPoint(animprog)];

        if (animrun) {
            animprog += Time.deltaTime * speed * 3f;
            if (animprog > 4) {
                animprog = 0;
                animrun = false;
            }
        }

        transform.localPosition = Vector2.Lerp(transform.localPosition, target, Time.deltaTime * speed);

        if (!PauseMenu.open) Controls();

        Vector3 hpbarscale = new Vector3(stats.hp / stats.maxhp, 1f, 1f);
        hpbar.localScale = hpbarscale;
        hpdmgbar.localScale = Vector3.Lerp(hpdmgbar.localScale, hpbarscale, Time.deltaTime * 2.5f);
        lvltxt.text = "Lv. " + stats.level;
    }

    void Controls() {
        if (Input.GetKeyDown(KeyCode.A)) {
            direction = Direction.LEFT;
            animrun = true;
            if (CheckCollision(target.x - .16f, target.y)) {
                target.x -= .16f;
            }
        } else if (Input.GetKeyDown(KeyCode.D)) {
            direction = Direction.RIGHT;
            animrun = true;
            if (CheckCollision(target.x + .16f, target.y)) {
                target.x += .16f;
            }
        } else if (Input.GetKeyDown(KeyCode.W)) {
            direction = Direction.UP;
            animrun = true;
            if (CheckCollision(target.x, target.y + .16f)) {
                target.y += .16f;
            }
        } else if (Input.GetKeyDown(KeyCode.S)) {
            direction = Direction.DOWN;
            animrun = true;
            if (CheckCollision(target.x, target.y - .16f)) {
                target.y -= .16f;
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            Enemy[] enemies = FindObjectsOfType<Enemy>();

            Enemy proximity = null;
            float proximitydst = 1.25f;
            foreach (Enemy enemy in enemies) {
                float dst = Vector2.Distance(enemy.transform.position, transform.position);
                if (dst < proximitydst) {
                    proximitydst = dst;
                    proximity = enemy;
                }
            }
            if (proximity != null) {
                GameObject slo = Instantiate(slashPrefab);
                slo.transform.position = proximity.transform.position;
                proximity.hp -= GetDamage();
            }
        }
    }

    public void TakeDamage(float amount) {
        if (amount < 0f) return;
        if (shieldValue > 0f) {
            amount -= shieldValue;
            if (amount < 0)
                shieldValue = amount * -1f;
            else
                stats.hp -= amount;
        } else {
            stats.hp -= amount;
        }

        stats.hp = Mathf.Max(stats.hp, 0f);
    }

    float GetDamage(bool burst = false) {
        float final = stats.atk;

        if (burst)
            final *= 1f + stats.GetTotalBuff(PlayerBuffType.BURSTDMG);

        if (Random.value < stats.critrate)
            final *= 1f + stats.critdmg;

        return final;
    }

    bool CheckCollision(float x, float y) {
        return true;
    }

    int GetAnimPoint(float anim) {
        int floor = Mathf.FloorToInt(anim);
        if (floor == 3) return 1;
        return floor;
    }
}

public enum Direction {
    DOWN, LEFT, RIGHT, UP
}

public enum PlayerBuffType {
    HP, ATK, CRITRATE, CRITDMG, BURSTDMG
}

[System.Serializable]
public class PlayerBuff {
    public PlayerBuffType type;
    public float value;
    public bool reforged;
}

[System.Serializable]
public class PlayerStats {
    public int level = 1;
    public float xp = 0;

    public List<PlayerBuff> buffs;

    public float hp = 100f;
    public float maxhp = 100f;
    public float atk = 10f;
    public float critrate = .25f;
    public float critdmg = .5f;

    public void Calculate() {
        CheckLevelUp();

        maxhp = level * 100 + (Mathf.Floor(level / 10) * 1000);
        hp = maxhp;
        atk = level * 10 + (Mathf.Floor(level / 10) * 100);
        critrate = .25f;
        critdmg = .5f;

        atk *= 1f + GetTotalBuff(PlayerBuffType.ATK);
        maxhp *= 1f + GetTotalBuff(PlayerBuffType.HP);
        critrate += GetTotalBuff(PlayerBuffType.CRITRATE);
        critdmg += GetTotalBuff(PlayerBuffType.CRITDMG);
    }

    public void CheckLevelUp() {
        float xptonext = level * 1000f + (Mathf.Floor(level / 10f) * 10000f);
        if (xp >= xptonext) {
            xp -= xptonext;
            level++;
            CheckLevelUp();
        }
    }

    public float GetTotalBuff(PlayerBuffType type) {
        float acc = 0f;
        int cnt = 0;

        foreach (PlayerBuff buff in buffs) {
            if (buff.type == type) {
                acc += buff.value;
                cnt++;
            }
        }

        if (cnt == 3) acc += 50f;
        return acc / 100f;
    }
}
