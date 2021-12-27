using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    public float speed = 6f;
    public float bSpeed = 6f;
    private float speedmult = 1f;
    private float speedmulttime;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private float velocity = 0f;
    private float dashTime = 0f;
    public Transform cam;
    public CharacterController controller;
    public Animation anim;

    public AudioSource hitAudioSource;
    public AudioClip[] hitSoundClips;

    [Header("Effects")]
    public GameObject slashPrefab;
    public GameObject burstSlashPrefab;
    public GameObject earthquakePrefab;
    public GameObject kaboomPrefab;
    public GameObject lightningPrefab;

    public Transform slasher;
    public TrailRenderer slasherTrail;
    public ParticleSystem slasherParticles;

    public Banner banner;
    public ForgeryController forgery;
    private Vector3 attackLock;
    private float attackCD;
    public bool hasDied;
    public bool canDie = true;
    private float iframes;

    [Header("Stats")]
    public PlayerStats stats;
    public float shieldValue = 0f;
    public Text lvltxt;
    public Transform hpbar;
    public Transform hpdmgbar;
    public Transform hpshieldbar;
    public float burstModeMult = 0f;
    public float burstModeTime = 0f;

    [Header("Enemy HUD")]
    public GameObject enemyhud;
    public Text enemylvl;
    public Transform enemyhp;
    public Transform enemydmg;
    public Transform enemyshield;
    [HideInInspector]
    public Enemy lasthitenemy;

    [Header("Storyline")]
    public DialogController dialog;
    public StorylineController story;

    [Header("Ability")]
    public Image abilityProgress;
    public Image abilityIcon;
    public Color abilityActiveColor;
    private float abilityCD = 0f;

    private float cdreduce = 0f;
    private float cdreducetime = 0f;

    [Header("Soul Shard")]
    public int soulCharge;
    public Transform soulShardGraphics;
    public Image soulChargeProgress;
    public Animation soulShardAnimation;
    public AudioSource soulShardAudio;
    public static bool soulShardAnimationPause;

    public GameObject ingameUI;
    public GameObject cinematicUI;

    void Start() {
        stats.Calculate();
        stats.hp = stats.maxhp;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        if (!PauseMenu.open && !dialog.isOpen && !soulShardAnimationPause && !hasDied && !forgery.enabled && !banner.open) Controls();
        if (!PauseMenu.open && !dialog.isOpen && !soulShardAnimationPause && !hasDied && !forgery.enabled && !banner.open) AbilityControls();

        cinematicUI.SetActive(soulShardAnimationPause);
        ingameUI.SetActive(!soulShardAnimationPause);

        Vector3 hpbarscale = new Vector3(stats.hp / stats.maxhp, 1f, 1f);
        hpbar.localScale = hpbarscale;
        hpshieldbar.localScale = Vector3.Lerp(hpshieldbar.localScale, new Vector3(Mathf.Min(shieldValue / stats.maxhp, 1f), 1f, 1f), 5f * Time.deltaTime);
        hpdmgbar.localScale = Vector3.Lerp(hpdmgbar.localScale, hpbarscale, Time.deltaTime * 2.5f);
        lvltxt.text = "Lv. " + stats.level;

        if (shieldValue > 0f) {
            shieldValue -= shieldValue * .025f * Time.deltaTime;
        }

        if (lasthitenemy != null) {
            enemylvl.text = "Lv. " + lasthitenemy.level;
            enemyhp.localScale = new Vector3(lasthitenemy.hp / lasthitenemy.maxhp, 1f, 1f);
            enemydmg.localScale = new Vector3(Mathf.Lerp(enemydmg.localScale.x, lasthitenemy.hp / lasthitenemy.maxhp, 2.5f * Time.deltaTime), 1f, 1f);
            enemyshield.localScale = new Vector3(lasthitenemy.shield / lasthitenemy.maxhp, 1f, 1f);
        }
        enemyhud.SetActive(lasthitenemy != null);

        if (stats.hp <= 0f && !hasDied) {
            hasDied = true;
            if (canDie) LoadingScreen.SwitchScene(2);
        }

        int ssgc = soulShardGraphics.transform.childCount;
        for (int i = 0; i < ssgc; i++) {
            if (stats.soulShard.type <= 0) {
                soulShardGraphics.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }
            soulShardGraphics.transform.GetChild(i).gameObject.SetActive((int) stats.soulShard.type == i + 1);
        }

        float tfill = (float) soulCharge / (float) stats.soulShard.ChargeMax();
        if (soulChargeProgress.fillAmount > tfill) {
            soulChargeProgress.fillAmount = Mathf.Lerp(soulChargeProgress.fillAmount, 0f, 5f * Time.deltaTime);
        } else {
            soulChargeProgress.fillAmount = tfill;
        }

        if (stats.hp < stats.maxhp * .99f && stats.hp > 10f) {
            stats.hp += .001f * stats.maxhp * Time.deltaTime;
        }

        if (iframes > 0f) iframes -= Time.deltaTime;
    }

    public void Teleport(Vector3 worldPos) {
        controller.enabled = false;
        transform.position = worldPos;
        controller.enabled = true;
    }

    public void Teleport(Transform worldPos) {
        controller.enabled = false;
        transform.position = worldPos.position;
        controller.enabled = true;
    }

    void AbilityControls() {
        if (abilityCD > stats.ability.GetCooldown()) abilityCD = stats.ability.GetCooldown();

        float tfill = 1f - (abilityCD / stats.ability.GetCooldown());
        if (abilityProgress.fillAmount > tfill) {
            abilityProgress.fillAmount = Mathf.Lerp(abilityProgress.fillAmount, 0f, 5f * Time.deltaTime);
        } else {
            abilityProgress.fillAmount = tfill;
        }
        Color targetCol = Color.white;
        if (abilityCD <= 0f)
            targetCol = abilityActiveColor;
        
        abilityProgress.color = Color.Lerp(abilityProgress.color, targetCol, 15f * Time.deltaTime);
        if (abilityCD > 0f) abilityCD -= Time.deltaTime;
        if (Input.GetMouseButtonDown(1) && abilityCD <= 0f) {
            abilityCD = stats.ability.GetCooldown();
            abilityCD -= cdreduce;
            if (abilityCD < 1f) abilityCD = 1f;
            stats.ability.Perform(this);
        }

        if (cdreduce > 0f && cdreducetime > 0f) {
            cdreducetime -= Time.deltaTime;
        } else if (cdreduce > 0f) {
            cdreduce = 0f;
        }

        if (burstModeMult > 0f) {
            if (burstModeTime > 0f) {
                burstModeTime -= Time.deltaTime;
            } else {
                burstModeMult = 0f;
                burstModeTime = 0f;
            }
        }
    }

    void Controls() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;
        bSpeed = Mathf.Lerp(bSpeed, speed, 7.5f * Time.deltaTime);
        if (dir.magnitude >= 0.1f) {
            if (!anim.isPlaying) anim.Play();
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * bSpeed * speedmult * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashTime <= 0f) {
            iframes = .25f;
            bSpeed *= 5f;
            attackCD = .1f;
            dashTime = .75f;
        }

        if (attackCD > 0f) {
            attackCD -= Time.deltaTime;

            Vector2 toVector = attackLock - transform.position;
            float angleToTarget = Vector2.Angle(transform.forward, toVector);

            float target = FixAngle(angleToTarget + 25f);
            slasher.localEulerAngles += Vector3.up * 360f * 2.5f * Time.deltaTime;
            if (FixAngle(slasher.localEulerAngles.y) >= target) {
                slasherTrail.emitting = false;
                slasherParticles.Stop();
            }
        }

        if (Input.GetMouseButtonDown(0) && attackCD <= 0f) {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            Enemy tohit = null;
            float tohitdst = 2.25f;

            foreach (Enemy enemy in enemies) {
                float dst = Vector3.Distance(transform.position, enemy.transform.position);
                if (dst < tohitdst) {
                    tohitdst = dst;
                    tohit = enemy;
                }
            }
            if (tohit != null) {
                if (burstModeMult > 0f) {
                    tohit.TakeDamage(GetDamage(true, burstModeMult), this);
                } else {
                    tohit.TakeDamage(GetDamage(), this);
                }
                attackLock = tohit.transform.position;
                transform.LookAt(tohit.transform);
                Vector2 toVector = attackLock - transform.position;
                float angleToTarget = Vector2.Angle(transform.forward, toVector);
                slasher.localEulerAngles = Vector3.up * (angleToTarget - 25f);

                OnHitEnemy(tohit);
                GameObject obj = Instantiate(burstModeMult > 0f ? burstSlashPrefab : slashPrefab);
                Vector3 midp = Vector3.Lerp(transform.position, tohit.transform.position, .6f);
                midp.y += .5f;
                obj.transform.position = midp;
                lasthitenemy = tohit;

                slasherTrail.Clear();
                slasherTrail.emitting = true;
                slasherParticles.Play();

                hitAudioSource.clip = hitSoundClips[Random.Range(0, hitSoundClips.Length)];
                hitAudioSource.Play();

                if (burstModeMult > 0f) {
                    attackCD = .05f;
                } else {
                    attackCD = .25f;
                }
            }
        }

        velocity -= Time.deltaTime * 9.81f;
        controller.Move(Vector3.up * velocity * Time.deltaTime);
        if (controller.isGrounded) {
            if (velocity < 0f) velocity = -.2f;

            if (Input.GetKeyDown(KeyCode.Space)) {
                velocity = 9.81f * .66f;
            }
        }

        if (dashTime > 0f) dashTime -= Time.deltaTime;

        if (speedmult > 1f) {
            if (speedmulttime > 0f) {
                speedmulttime -= Time.deltaTime;
            } else {
                speedmult = 1f;
                speedmulttime = 0f;
            }
        }
    }

    public static float FixAngle(float angle) {
        if (angle > 180f) return FixAngle(180f - angle);
        return angle;
    }

    public void AddSpeedMult(float power, float time) {
        speedmult += power;
        speedmulttime = Mathf.Max(speedmulttime, time);
    }

    public void ReduceCD(float reduce, float time) {
        if (cdreduce == 888.888f) return;
        cdreduce += reduce;
        cdreducetime = Mathf.Max(cdreducetime, time);
        abilityCD -= reduce;
        if (abilityCD < 1f) abilityCD = 1f;
    }

    public void OnHitEnemy(Enemy e) {
        stats.hp += stats.maxhp * .001f;
        stats.hp = Mathf.Min(stats.hp, stats.maxhp);

        if (e.hp <= 0f) {
            soulCharge += 50;
        } else {
            soulCharge++;

            if (soulCharge >= stats.soulShard.ChargeMax()) {
                if (stats.soulShard != null && stats.soulShard.type != PlayerSoulShardType.NONE) {
                    soulCharge = 0;
                    soulShardAnimationPause = true;
                    soulShardAudio.Play();
                    StartCoroutine(DisableSoulShardAnimPause());
                    soulShardAnimation.Play();
                    cam.transform.position += cam.transform.forward * 2.5f;
                }
            }
        }
    }

    private IEnumerator DisableSoulShardAnimPause() {
        yield return new WaitForSeconds(1f);
        soulShardAnimationPause = false;
        stats.soulShard.Perform(this);
    }

    public void TakeDamage(float amount) {
        if (amount <= 0f) return;
        if (iframes > 0f) return;
        if (shieldValue > 0f) {
            shieldValue -= amount * .5f;
            if (shieldValue < 0f) {
                stats.hp -= shieldValue * -1f;
                shieldValue = 0f;
            }
        } else {
            stats.hp -= amount;
        }

        GameObject obj = Instantiate(slashPrefab);
        obj.transform.position = transform.position + Random.insideUnitSphere;

        stats.hp = Mathf.Max(stats.hp, 0f);
    }

    public void Heal(float amount) {
        stats.hp += amount;
        stats.hp = Mathf.Min(stats.hp, stats.maxhp);
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

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Water")) {
            stats.hp = 0f;
        }
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
                return 1.5f;
            case PlayerAbilityType.SHIELD:
                return 25f;
            case PlayerAbilityType.HEAL:
                return 10f;
            case PlayerAbilityType.BLINK:
                return 4.5f;
            case PlayerAbilityType.METEOR:
                return 8.5f;
            case PlayerAbilityType.EARTHQUAKE:
                return 9.5f;
            case PlayerAbilityType.BOLT:
                return 5.5f;
            default:
                return 5f;
        }
    }

    public void Perform(PlayerController player) {
        float level = Mathf.Min(player.stats.level, this.level);

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
                player.lasthitenemy = targetenemy;
                targetenemy.TakeDamage(player.GetDamage(true, .5f + (level-1) * .02f), player);
                player.OnHitEnemy(targetenemy);
                GameObject.Instantiate(player.burstSlashPrefab).transform.position = targetenemy.transform.position;
            }
        } else if (type == PlayerAbilityType.SHIELD) {
            float mult = Mathf.Clamp01(.25f + (level - 1) * .05f);
            player.shieldValue = player.stats.maxhp * mult;
        } else if (type == PlayerAbilityType.HEAL) {
            float mult = .1f + (level - 1) * .05f;
            float value = player.stats.maxhp * mult;
            player.Heal(value);
        } else if (type == PlayerAbilityType.BLINK) {
            float mult = 5f + (level - 1) * .1f;
            player.bSpeed = player.speed * mult;
        } else if (type == PlayerAbilityType.METEOR) {
            float mult = 1f + (level - 1) * .05f;
            GameObject.Instantiate(player.kaboomPrefab, player.transform.position, Quaternion.identity);
            AoE(player, 5f, mult, false);
        } else if (type == PlayerAbilityType.EARTHQUAKE) {
            float mult = 1f + (level - 1) * .05f;
            GameObject.Instantiate(player.earthquakePrefab, player.transform.position, Quaternion.identity);
            AoE(player, 10f, mult, false);
        } else if (type == PlayerAbilityType.BOLT) {
            float mult = 2f + (level - 1) * .08f;
            GameObject.Instantiate(player.lightningPrefab, player.transform.position, Quaternion.identity);
            AoE(player, 5f, mult, false);
        }
    }

    private void AoE(PlayerController pc, float radius, float mult, bool particles=true) {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        List<Enemy> hittable = new List<Enemy>();
        foreach (Enemy enemy in enemies) {
            if (Vector3.Distance(pc.transform.position, enemy.transform.position) < radius) {
                hittable.Add(enemy);
            }
        }
        foreach (Enemy enemy in hittable) {
            enemy.TakeDamage(pc.GetDamage(true, mult), pc);
            pc.OnHitEnemy(enemy);
            if (particles) {
                GameObject obj = GameObject.Instantiate(pc.burstSlashPrefab);
                obj.transform.position = enemy.transform.position;
            }
            pc.lasthitenemy = enemy;
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

    private int ChargeBase() {
        if (type == PlayerSoulShardType.CROWNED) return 2000;
        if (type == PlayerSoulShardType.WINGED) return 2000;
        if (type == PlayerSoulShardType.DEPTHS) return 1500;
        if (type == PlayerSoulShardType.MAGE) return 500;
        if (type == PlayerSoulShardType.DEMON) return 1500;

        return 5000; // in case I forget to add new ones
    }

    public int ChargeMax() {
        float cb = ChargeBase();
        return (int) Mathf.Max(cb * (1f - (level-1) * .05f), 100);
    }

    public void Perform(PlayerController player) {
        if (type == PlayerSoulShardType.CROWNED) {
            player.burstModeMult = 2f;
            player.burstModeTime = 10f;
        }
        if (type == PlayerSoulShardType.WINGED) {
            player.burstModeMult = 1f;
            player.burstModeTime = 15f;
            player.AddSpeedMult(.5f, 10f);
            player.ReduceCD(5f, 10f);
        }
        if (type == PlayerSoulShardType.DEPTHS) {
            float dmg = ReverseAoE(player, 10f) * .35f;
            AoE(player, 10f, dmg);
        }
        if (type == PlayerSoulShardType.MAGE) {
            player.ReduceCD(1.5f, 10f);
        }
        if (type == PlayerSoulShardType.DEMON) {
            AoE(player, 10f, player.stats.maxhp * 1.5f);
            player.stats.hp *= .7f;
        }
    }

    private float ReverseAoE(PlayerController pc, float radius) {
        float hpacc = 0f;
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        List<Enemy> hittable = new List<Enemy>();
        foreach (Enemy enemy in enemies) {
            if (Vector3.Distance(pc.transform.position, enemy.transform.position) < radius) {
                hittable.Add(enemy);
            }
        }
        foreach (Enemy enemy in hittable) {
            hpacc += enemy.maxhp;
        }
        return hpacc;
    }

    private void AoE(PlayerController pc, float radius, float directdmg) {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        List<Enemy> hittable = new List<Enemy>();
        foreach (Enemy enemy in enemies) {
            if (Vector3.Distance(pc.transform.position, enemy.transform.position) < radius) {
                hittable.Add(enemy);
            }
        }
        foreach (Enemy enemy in hittable) {
            enemy.TakeDamage(directdmg, pc);
            pc.OnHitEnemy(enemy);
            pc.lasthitenemy = enemy;
        }
    }

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

    public long checksum;

    public void Calculate() {
        CheckLevelUp();

        maxhp = level * 100 + (Mathf.Floor(level / 10) * 1000);
        atk = level * 10 + (Mathf.Floor(level / 10) * 100);
        critrate = .25f;
        critdmg = .5f;

        atk *= 1f + GetTotalBuff(PlayerBuffType.ATK);
        maxhp *= 1f + GetTotalBuff(PlayerBuffType.HP);
        critrate += GetTotalBuff(PlayerBuffType.CRIT_RATE);
        critdmg += GetTotalBuff(PlayerBuffType.CRIT_DMG);
    }

    public void CheckLevelUp() {
        if (level >= 99) return; // level limited to 99 (this version)

        float xptonext = level * 100f + Mathf.Floor(level / 10) * 1000f;
        if (xp >= xptonext) {
            xp -= xptonext;
            level++;
            if (level % 10 == 0) 
                GameObject.FindObjectOfType<ResourceController>().bstars += 5;
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
            if (cnt >= 3) break;
        }

        if (cnt == 3) acc += 50f;
        acc = Mathf.Min(acc, (level + 30) * 3f + 50f);
        return acc / 100f;
    }

    public PlayerStats Verify() {
        checksum = Checksum();
        return this;
    }

    public long Checksum() {
        return (long) (level * (1 + xp) * 32873 * 758364572) % 758364572743;
    }
}
