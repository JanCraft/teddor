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

        if (bleed > 0f) {
            bleed = Mathf.Lerp(bleed, 0f, .75f * Time.deltaTime);
            boss.hp -= Mathf.Min((1.5f + bleed) * maxhp * .1f, maxhp * .1f) * Time.deltaTime;
        }

        if (flaming > 0f) {
            flaming = Mathf.Lerp(flaming, 1f, 1.5f * Time.deltaTime);
            boss.hp -= Mathf.Min((2.5f + flaming) * maxhp * .25f, maxhp * .125f) * Time.deltaTime;
        }
    }

    public override void TakeDamage(float amount, PlayerController _, bool tdmg) {
        if (tdmg) {
            boss.TakeDamage(amount * 2f);
        } else boss.TakeDamage(amount);
    }
}
