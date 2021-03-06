using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossEnemy : MonoBehaviour {
    [Header("Multipliers")]
    public float hpMult = 1.5f;
    public float atkMult = 1.5f;

    [Header("Shield")]
    public float shieldReduction = .5f;

    [Header("Attack Animation")]
    public Animation anim;
    public AnimationClip[] attacks;
    private int nextatt;
    public float attackCD = 2.5f;
    public bool randomAttack;

    [Header("Rewards")]
    public int xp = 1000;
    public int bMatter = 10;
    public int bStars = 1;
    public int uMatter = 0;
    public UnityEvent reward;

    // Internal State
    [HideInInspector]
    public float hp, atk, shield, maxhp;
    [HideInInspector]
    public int lvl;
    private float nextAttack;
    private PlayerCombat pc;

    void OnEnable() {
        pc = FindObjectOfType<PlayerCombat>();
        lvl = pc.stats.level;
        maxhp = hp = pc.stats.maxhp * hpMult;
        atk = pc.stats.atk * atkMult;
        shield = hp;

        nextAttack = attackCD + 2f;

        if (pc.GetComponent<ResourceController>().coins < xp) {
            pc.stats.hp = -100f;
        } else {
            pc.GetComponent<ResourceController>().coins -= xp;
        }
    }

    public void TakeDamage(float amount) {
        if (shield > 0f) {
            shield -= (amount - amount * shieldReduction);
            if (shield < 0) {
                hp += shield;
                shield = 0f;
            }
        } else {
            hp -= amount;
        }
    }

    void Update() {
        if (nextAttack < 0f) {
            AnimationClip c = attacks[nextatt];
            anim.clip = c;
            anim.Play();
            nextAttack = attackCD;
            if (randomAttack) {
                nextatt = Random.Range(0, attacks.Length);
            } else {
                nextatt++;
                nextatt %= attacks.Length;
            }
        } else {
            nextAttack -= Time.deltaTime;
        }

        hp = Mathf.Max(hp, 0f);
        if (hp <= 0f) {
            pc.stats.xp += xp;
            pc.stats.Calculate();
            pc.GetComponent<ResourceController>().bmatter += bMatter;
            pc.GetComponent<ResourceController>().bstars += bStars;
            pc.GetComponent<ResourceController>().umatter += uMatter;
            pc.ui.removeEnemyHUD = true;
            reward.Invoke();

            gameObject.SetActive(false);
        }
    }
}
