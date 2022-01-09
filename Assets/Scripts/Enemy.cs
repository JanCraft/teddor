using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float hp = 100f;
    public float maxhp = 100f;
    public float atk = 10f;
    public int areadanger = 0;
    public int level = -1;
    public bool canGrantStars = true;

    public float bleed;
    public float flaming;
    public float paralyze;

    private float attackCD = 2.5f;
    private bool attacking;

    public float shield = 0f;
    public float shieldReduction = .5f;

    public bool gravityEnabled = true;
    public bool bleedATKs;

    public float baseSpeed = 2f;
    public float baseAttackDST = 1.5f;
    public float baseAttackCD = .75f;
    public float baseHealthMLT = 1f;
    public float baseAttackDMG = 1f;
    public float effSpeed = 1f;
    public bool stationary;
    public LongRangeMode longRange;

    public GameObject dmgIndicatorPrefab;
    public Animation anim;
    private PlayerController player;
    private Rigidbody rb;
    public AudioSource hitsound;

    public virtual void Start() {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody>();
        if (level < 0) {
            int baselvl = player.stats.level;
            level = baselvl + areadanger * 2;
        }

        hp = maxhp = (level * 100 + (Mathf.Floor(level / 10) * 100)) * baseHealthMLT * Mathf.Max(Mathf.Min(15f, level * .5f), 1f);
        shield = shield * maxhp;
        atk = (level * 10f + (Mathf.Floor(level / 10) * 100)) * baseAttackDMG * Mathf.Max(Mathf.Min(2.5f, level * .15f), 1f);
    }

    public virtual void Update() {
        if (transform.position.y < -250) Destroy(gameObject);
        if (PauseMenu.open || PlayerController.soulShardAnimationPause) return;

        Vector3 dir = (player.transform.position - transform.position).normalized;
        float dst = Vector3.Distance(player.transform.position, transform.position);

        if (!stationary && !attacking) {
            if (dst > 1.5f) {
                transform.position += dir * baseSpeed * effSpeed * Time.deltaTime;
            } else if (dst < 1.45f) {
                transform.position -= dir * baseSpeed * effSpeed * Time.deltaTime;
            }
        }

        if (attackCD <= 0f) {
            attackCD = baseAttackCD + UnityEngine.Random.value * 2f;
            attacking = true;
            if (dst <= baseAttackDST) {
                StartCoroutine(ShortRangeAttack());
                attackCD += 1.5f;
            } else {
                if (longRange == LongRangeMode.OVERHEAD) {
                    StartCoroutine(LongRangeAttack());
                } else if (longRange == LongRangeMode.NORMAL) {
                    StartCoroutine(ShortRangeAttack());
                } else if (longRange == LongRangeMode.SPEEDUP) {
                    baseSpeed *= 1.25f;
                    baseSpeed = Mathf.Min(baseSpeed, 8f);
                    attacking = false;
                }
                attackCD += 3.5f;
            }
        }
        if (attackCD > 0f) attackCD -= Time.deltaTime;
        transform.LookAt(player.transform);

        rb.useGravity = gravityEnabled;
        if (!gravityEnabled) {
            Vector3 v = rb.velocity;
            v.y = 0f;
            rb.velocity = v;
        }

        if (paralyze > 0f) {
            paralyze -= Time.deltaTime;
            attackCD = 1f;
        }

        if (bleed > 0f) {
            bleed = Mathf.Lerp(bleed, 0f, .75f * Time.deltaTime);
            hp -= Mathf.Min((1.5f + bleed) * maxhp * .1f, maxhp * .1f) * Time.deltaTime;
            CheckDeath();
        }

        if (flaming > 0f) {
            flaming = Mathf.Lerp(flaming, 1f, 1.5f * Time.deltaTime);
            hp -= Mathf.Min((2.5f + flaming) * maxhp * .25f, maxhp * .125f) * Time.deltaTime;
            CheckDeath();
        }
    }

    IEnumerator ShortRangeAttack() {
        anim.Play();
        yield return new WaitForSeconds(baseAttackCD);

        hitsound.Play();
        if (Vector3.Distance(player.transform.position, transform.position) < baseAttackDST) {
            player.TakeDamage(atk);
            if (bleedATKs) player.bleedHP = atk * .65f;
        }

        attacking = false;
        gravityEnabled = true;
    }

    IEnumerator LongRangeAttack() {
        anim.Play();

        transform.position = player.transform.position + Vector3.up * 3.5f;
        gravityEnabled = false;
        yield return new WaitForSeconds(baseAttackCD);
        gravityEnabled = true;

        hitsound.Play();
        if (Vector3.Distance(player.transform.position, transform.position) < baseAttackDST) {
            player.TakeDamage(atk);
        }

        attacking = false;
    }

    public virtual void TakeDamage(float amount, PlayerController player, bool trueDMG) {
        if (shield > 0f && !trueDMG) {
            shield -= (amount - amount * shieldReduction);
            if (shield < 0) shield = 0f;
        } else {
            hp -= amount;
            SpawnDamageNumber((int) amount, transform.position + Vector3.up * 1f);
        }

        if (bleed > 0f) bleed += 1f;

        CheckDeath();
    }

    private void CheckDeath() {
        hp = Mathf.Max(hp, 0f);
        if (hp <= 0f) {
            Destroy(gameObject);
            player.stats.xp += 100f;
            player.stats.Calculate();
            player.GetComponent<ResourceController>().coins += 500;
            player.GetComponent<ResourceController>().bmatter += 5;
            if (canGrantStars) {
                if (UnityEngine.Random.value < .075f) {
                    player.GetComponent<ResourceController>().bstars += 1;
                }
            }
        }
    }

    public void SpawnDamageNumber(int damage, Vector3 pos) {
        GameObject obj = Instantiate(dmgIndicatorPrefab, pos, Quaternion.identity);
        obj.GetComponentInChildren<UnityEngine.UI.Text>().text = damage.ToString();
    }
}

public enum LongRangeMode {
    OVERHEAD, SPEEDUP, NORMAL
}