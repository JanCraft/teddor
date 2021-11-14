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
    private float attackCD;

    public float baseSpeed = 2f;
    public float baseAttackDST = 1.5f;
    public float baseAttackCD = .75f;
    public float baseHealthMLT = 1f;
    public float baseAttackDMG = 1f;
    public float baseStunCD = .25f;
    public float baseFleeSpeed = 2f;

    public GameObject dmgIndicatorPrefab;
    private PlayerController player;

    void Start() {
        player = FindObjectOfType<PlayerController>();
        if (level < 0) {
            int baselvl = player.stats.level;
            level = baselvl + areadanger * 2;
        }

        hp = maxhp = (level * 100 + (Mathf.Floor(level / 10) * 1000)) * baseHealthMLT;
        atk = (level * 10 + (Mathf.Floor(level / 10) * 100)) * baseAttackDMG;
    }

    void Update() {
        if (transform.position.y < -250) Destroy(gameObject);
        if (PauseMenu.open || PlayerController.soulShardAnimationPause) return;

        Vector3 dir = (player.transform.position - transform.position).normalized;
        if (attackCD > 0f) dir *= -baseFleeSpeed;
        float dst = Vector3.Distance(player.transform.position, transform.position);
        if (dst > baseAttackDST && dst < 24f) transform.position += dir * baseSpeed * Time.deltaTime;
        if (dst <= baseAttackDST) {
            if (attackCD <= 0f) {
                player.TakeDamage(atk);
                attackCD = baseAttackCD;
            }
        }
        if (attackCD > 0f) attackCD -= Time.deltaTime;
        transform.LookAt(player.transform);
    }

    public void TakeDamage(float amount, PlayerController player) {
        hp -= amount;
        SpawnDamageNumber((int) amount, transform.position + Vector3.up * 1f);
        attackCD = baseStunCD;

        hp = Mathf.Max(hp, 0f);
        if (hp <= 0f) {
            if (canGrantXP) {
                player.stats.xp += (1000 + (Mathf.Floor(level / 10) * 10000)) / 100;
                player.stats.Calculate();
            }

            Destroy(gameObject);
        }
    }

    public void SpawnDamageNumber(int damage, Vector3 pos) {
        GameObject obj = Instantiate(dmgIndicatorPrefab, pos, Quaternion.identity);
        obj.GetComponentInChildren<UnityEngine.UI.Text>().text = damage.ToString();
    }
}
