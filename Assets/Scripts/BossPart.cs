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
    }

    public override void TakeDamage(float amount, PlayerController _, bool __) {
        boss.TakeDamage(amount);
    }
}
