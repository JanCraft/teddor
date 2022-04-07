using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {
    public PlayerCombatAbilities combatAbilities;
    public PlayerUI ui;

    [Header("Effects")]
    public GameObject slashPrefab;
    public GameObject burstSlashPrefab;
    public GameObject earthquakePrefab;
    public GameObject kaboomPrefab;
    public GameObject lightningPrefab;
    public GameObject splashPrefab;
    public GameObject firePrefab;

    public Transform slasher;
    public TrailRenderer slasherTrail;
    public ParticleSystem slasherParticles;

    [Header("Audio")]
    public AudioSource hitAudioSource;
    public AudioClip[] hitSoundClips;

    private Vector3 attackLock;
    [HideInInspector] public float attackCD;
    private int attackCnt;
    [HideInInspector] public bool hasDied;
    [HideInInspector] public bool canDie = true;
    [HideInInspector] public float iframes;

    [Header("Stats")]
    public PlayerStats stats;
    public float shieldValue = 0f;
    public float burstModeMult = 0f;
    public float burstModeTime = 0f;
    public float bleedHP, healHP;
    
    [HideInInspector] public Enemy lasthitenemy;

    void Start() {
        stats.Calculate();
        stats.hp = stats.maxhp;
    }

    void Update() {
        if (attackCD > 0f) {
            attackCD -= Time.deltaTime;

            Vector2 toVector = attackLock - transform.position;
            float angleToTarget = Vector2.Angle(transform.forward, toVector);

            float target = FixAngle(angleToTarget + 90f);
            if (attackCnt == 0) {
                slasher.localEulerAngles += Vector3.up * 360f * 2.5f * Time.deltaTime;
            } else {
                slasher.localEulerAngles -= Vector3.up * 360f * 2.5f * Time.deltaTime;
            }
            if (FixAngle(slasher.localEulerAngles.y) >= target) {
                slasherTrail.emitting = false;
                slasherParticles.Stop();
            }
        }

        if (Input.GetMouseButtonDown(0) && attackCD <= 0f) {
            attackCnt++;
            attackCnt %= 2;
            Enemy tohit = Enemy.GetEnemyInRadius(transform.position, 3f);
            if (tohit != null) {
                if (burstModeMult > 0f) {
                    tohit.TakeDamage(GetDamage(true, burstModeMult), this, false);
                } else {
                    tohit.TakeDamage(GetDamage(), this, false);
                }
                attackLock = tohit.transform.position;
                transform.LookAt(tohit.transform);
                Vector2 toVector = attackLock - transform.position;
                float angleToTarget = Vector2.Angle(transform.forward, toVector);
                if (attackCnt == 0) {
                    slasher.localEulerAngles = Vector3.up * (angleToTarget - 90f);
                } else {
                    slasher.localEulerAngles = Vector3.up * (angleToTarget + 90f);
                }

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
                    attackCD = .1f;
                } else {
                    if (attackCnt == 0) {
                        attackCD = .25f;
                    } else {
                        attackCD = .15f;
                    }
                }
            }
        }

        if (burstModeMult > 0f) {
            if (burstModeTime > 0f) {
                burstModeTime -= Time.deltaTime;
            } else {
                burstModeMult = 0f;
                burstModeTime = 0f;
            }
        }

        if (shieldValue > 0f) {
            shieldValue -= shieldValue * .005f * Time.deltaTime;
        }

        if (stats.hp <= 0f && !hasDied) {
            hasDied = true;
            if (canDie) LoadingScreen.SwitchScene(2);
        }

        if (stats.hp < stats.maxhp * .99f && stats.hp > 10f) {
            stats.hp += .001f * stats.maxhp * Time.deltaTime;
        }

        if (bleedHP > 0f) {
            float tobleed = bleedHP * Time.deltaTime;
            stats.hp -= tobleed;
            bleedHP -= tobleed;
        }
        if (healHP > 0f) {
            float toheal = healHP * Time.deltaTime;
            stats.hp += toheal;
            healHP -= toheal;
        }

        stats.hp = Mathf.Clamp(stats.hp, 0f, stats.maxhp);
        if (iframes > 0f) iframes -= Time.deltaTime;
    }

    public static float FixAngle(float angle) {
        if (angle > 180f) return FixAngle(180f - angle);
        return angle;
    }

    public void OnHitEnemy(Enemy e) {
        stats.hp += stats.maxhp * .001f;
        stats.hp = Mathf.Min(stats.hp, stats.maxhp);

        if (e.hp <= 0f) {
            combatAbilities.soulCharge += 50;
        } else {
            combatAbilities.soulCharge++;

            if (combatAbilities.soulCharge >= stats.soulShard.ChargeMax()) {
                if (stats.soulShard != null && stats.soulShard.type != PlayerSoulShardType.NONE) {
                    combatAbilities.soulCharge = 0;
                    PlayerCombatAbilities.soulShardAnimationPause = true;
                    combatAbilities.soulShardAudio.Play();
                    StartCoroutine(combatAbilities.DisableSoulShardAnimPause());
                    combatAbilities.soulShardAnimation.Play();
                    Camera.main.transform.position += Camera.main.transform.forward * 2.5f;
                }
            }
        }
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

    public void HealMax() {
        stats.hp = stats.maxhp;
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

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<BossDamage>() != null) {
            TakeDamage(other.GetComponent<BossDamage>().boss.atk);
        }
        if (other.CompareTag("Water")) {
            stats.hp = 0f;
            Instantiate(splashPrefab, transform.position, Quaternion.identity);
        }
    }
}