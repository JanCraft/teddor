using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private float velocity = 0f;
    public Transform cam;
    public CharacterController controller;

    [Header("Effects")]
    public GameObject slashPrefab;
    public GameObject burstSlashPrefab;

    public Transform slasher;
    public TrailRenderer slasherTrail;
    public ParticleSystem slasherParticles;
    private float attackCD;

    [Header("Stats")]
    public PlayerStats stats;
    public float shieldValue = 0f;
    public Text lvltxt;
    public Transform hpbar;
    public Transform hpdmgbar;

    [Header("Ability")]
    public Image abilityProgress;
    public Image abilityIcon;
    private float abilityCD = 0f;

    void Start() {
        stats.Calculate();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        if (!PauseMenu.open) Controls();
        if (!PauseMenu.open) AbilityControls();

        Vector3 hpbarscale = new Vector3(stats.hp / stats.maxhp, 1f, 1f);
        hpbar.localScale = hpbarscale;
        hpdmgbar.localScale = Vector3.Lerp(hpdmgbar.localScale, hpbarscale, Time.deltaTime * 2.5f);
        lvltxt.text = "Lv. " + stats.level;
    }

    void AbilityControls() {
        abilityProgress.fillAmount = 1f - (abilityCD / stats.ability.GetCooldown());
        if (abilityCD > 0f) abilityCD -= Time.deltaTime;
        if (Input.GetMouseButtonDown(1) && abilityCD <= 0f) {
            abilityCD = stats.ability.GetCooldown();
            stats.ability.Perform(this);
        }
    }

    void Controls() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;
        if (dir.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (attackCD > 0f) {
            attackCD -= Time.deltaTime;

            float initial = slasher.localEulerAngles.y;
            slasher.localEulerAngles += Vector3.up * 360f * 5f * Time.deltaTime;
            if (initial > slasher.localEulerAngles.y) {
                slasherTrail.emitting = false;
            }
        }
        if (Input.GetMouseButtonDown(0) && attackCD <= 0f) {
            slasher.localEulerAngles = Vector3.up * Random.value * 90f;
            slasherTrail.Clear();
            slasherTrail.emitting = true;
            slasherParticles.Play();

            Enemy[] enemies = FindObjectsOfType<Enemy>();
            List<Enemy> hittable = new List<Enemy>();
            foreach (Enemy enemy in enemies) {
                if (Vector3.Distance(transform.position, enemy.transform.position) < 2.25f) {
                    hittable.Add(enemy);
                }
            }
            foreach (Enemy enemy in hittable) {
                enemy.TakeDamage(GetDamage(), transform.position);
                GameObject obj = Instantiate(slashPrefab);
                Vector3 midp = Vector3.Lerp(transform.position, enemy.transform.position, .6f);
                midp.y += .5f;
                obj.transform.position = midp;
            }

            attackCD = .25f;
        }

        velocity -= Time.deltaTime * 9.81f;
        controller.Move(Vector3.up * velocity * Time.deltaTime);
        if (controller.isGrounded) {
            velocity = -.2f;
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

    public float GetDamage(bool burst = false, float multiplier = 1f) {
        float final = stats.atk;

        final *= multiplier;

        if (burst)
            final *= 1f + stats.GetTotalBuff(PlayerBuffType.BURST_DMG);

        if (Random.value < stats.critrate)
            final *= 1f + stats.critdmg;

        return final;
    }
}

public enum PlayerBuffType {
    HP, ATK, CRIT_RATE, CRIT_DMG, BURST_DMG
}

[System.Serializable]
public class PlayerBuff {
    public PlayerBuffType type;
    public float value;
    public bool reforged;
    public int identifier = new System.Random().Next();

    public bool Equality(PlayerBuff obj) {
        return obj.value == value && obj.type == type && obj.reforged == reforged && obj.identifier == identifier;
    }
}

public enum PlayerAbilityType {
    NONE, RANGED, SHIELD, HEAL, BLINK, METEOR, EARTHQUAKE, BOLT
}

[System.Serializable]
public class PlayerAbility {
    public PlayerAbilityType type;
    public int level;

    public int GetStarCount() {
        if (type == PlayerAbilityType.METEOR || type == PlayerAbilityType.EARTHQUAKE) {
            return 5;
        } else if (type == PlayerAbilityType.BOLT) {
            return 6;
        }

        return 4;
    }

    public float GetCooldown() {
        switch (type) {
            case PlayerAbilityType.RANGED:
                return 5f;
            case PlayerAbilityType.SHIELD:
                return 12.5f;
            case PlayerAbilityType.HEAL:
                return 15f;
            case PlayerAbilityType.BLINK:
                return 4.5f;
            case PlayerAbilityType.METEOR:
                return 13.5f;
            case PlayerAbilityType.EARTHQUAKE:
                return 14.5f;
            case PlayerAbilityType.BOLT:
                return 18.5f;
            default:
                return 5f;
        }
    }

    public void Perform(PlayerController player) {
        if (type == PlayerAbilityType.RANGED) {
            Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
            Enemy targetenemy = null;
            float dstenemy = 25f;
            foreach (Enemy enemy in enemies) {
                float dst = Vector3.Distance(player.transform.position, enemy.transform.position);
                if (dst < dstenemy) {
                    dstenemy = dst;
                    targetenemy = enemy;
                }
            }
            if (targetenemy != null) {
                targetenemy.TakeDamage(player.GetDamage(true, 1f + (level-1) * .5f), player.transform.position);
                GameObject.Instantiate(player.burstSlashPrefab).transform.position = targetenemy.transform.position;
            }
        }
    }

    public bool Equality(PlayerAbility obj) {
        return obj.level == level && obj.type == type;
    }
}

public enum PlayerSoulShardType {
    NONE, CROWNED, WINGED, DEPTHS, MAGE, DEMON
}

[System.Serializable]
public class PlayerSoulShard {
    public PlayerSoulShardType type;
    public int level = 1;

    public bool Equality(PlayerSoulShard obj) {
        return obj.level == level && obj.type == type;
    }
}

[System.Serializable]
public class PlayerStats {
    public int level = 1;
    public float xp = 0;

    public List<PlayerBuff> buffs;
    public PlayerAbility ability = null;
    public PlayerSoulShard soulShard = null;

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
        critrate += GetTotalBuff(PlayerBuffType.CRIT_RATE);
        critdmg += GetTotalBuff(PlayerBuffType.CRIT_DMG);
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
