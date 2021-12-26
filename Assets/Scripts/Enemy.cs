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
    public bool canGrantXP = true;

    private float attackCD = 2.5f;
    private bool attacking;

    public float shield = 0f;
    public float shieldReduction = .5f;

    public bool gravityEnabled = true;

    public float baseSpeed = 2f;
    public float baseAttackDST = 1.5f;
    public float baseAttackCD = .75f;
    public float baseHealthMLT = 1f;
    public float baseAttackDMG = 1f;
    public bool stationary;
    public LongRangeMode longRange;

    public GameObject dmgIndicatorPrefab;
    public Animation anim;
    private PlayerController player;
    private Rigidbody rb;
    public AudioSource hitsound;

    void Start() {
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

    void Update() {
        if (transform.position.y < -250) Destroy(gameObject);
        if (PauseMenu.open || PlayerController.soulShardAnimationPause) return;

        Vector3 dir = (player.transform.position - transform.position).normalized;
        float dst = Vector3.Distance(player.transform.position, transform.position);

        if (!stationary && !attacking) {
            if (dst > 1.5f) {
                transform.position += dir * baseSpeed * Time.deltaTime;
            } else if (dst < 1.45f) {
                transform.position -= dir * baseSpeed * Time.deltaTime;
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
    }

    IEnumerator ShortRangeAttack() {
        anim.Play();
        yield return new WaitForSeconds(baseAttackCD);

        hitsound.Play();
        if (Vector3.Distance(player.transform.position, transform.position) < baseAttackDST) {
            player.TakeDamage(atk);
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

    public void TakeDamage(float amount, PlayerController player) {
        if (shield > 0f) {
            shield -= (amount - amount * shieldReduction);
            if (shield < 0) {
                hp += shield;
                shield = 0f;
            }
        } else {
            hp -= amount;
            SpawnDamageNumber((int) amount, transform.position + Vector3.up * 1f);
        }

        hp = Mathf.Max(hp, 0f);
        if (hp <= 0f) {
            if (canGrantXP) {
                player.stats.xp += 100f;
                player.stats.Calculate();
                player.GetComponent<ResourceController>().coins += 500;
                player.GetComponent<ResourceController>().bmatter += 5;
                if (UnityEngine.Random.value < .025f)
                    player.GetComponent<ResourceController>().bstars += 1;
            }

            Destroy(gameObject);
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