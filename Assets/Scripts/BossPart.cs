using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPart : Enemy {
    [Header("Boss Part Settings")]
    public BossEnemy boss;

    public override void Start() {}
    public override void Update() {
        hp = boss.hp;
        maxhp = boss.maxhp;
        shield = boss.shield;
        level = boss.lvl;

        // bleed and flaming slightly reduced
        if (bleed > 0f) {
            bleed = Mathf.Lerp(bleed, 0f, .75f * Time.deltaTime);
            boss.hp -= (bleed * Mathf.Min(maxhp * .015f, 1000)) * Time.deltaTime;
        }

        if (flaming > 0f) {
            flaming = Mathf.Lerp(flaming, 1f, 1.5f * Time.deltaTime);
            boss.hp -= (flaming * Mathf.Min(maxhp * .005f, 2500)) * Time.deltaTime;
        }
    }

    public override void TakeDamage(float amount, PlayerController _, bool tdmg) {
        if (tdmg) {
            boss.TakeDamage(amount * 2f);
        } else boss.TakeDamage(amount);
    }
}
