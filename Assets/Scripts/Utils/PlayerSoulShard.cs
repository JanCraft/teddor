using System.Collections.Generic;
using System.Collections;
using UnityEngine;

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
        if (type == PlayerSoulShardType.TIDES) return 1750;
        if (type == PlayerSoulShardType.FLAMES) return 1650;

        return 5000; // in case I forget to add new ones
    }

    public int ChargeMax() {
        float cb = ChargeBase();
        return (int) Mathf.Max(cb * (1f - (level - 1) * .05f), 100);
    }

    public void Perform(PlayerCombat player) {
        if (type == PlayerSoulShardType.CROWNED) {
            player.burstModeMult = 2f;
            player.burstModeTime = 10f;
        } else if (type == PlayerSoulShardType.WINGED) {
            player.burstModeMult = 1f;
            player.burstModeTime = 15f;
            player.GetComponent<PlayerMovement>().AddSpeedMult(.5f, 10f);
            player.combatAbilities.ReduceCD(5f, 10f);
        } else if (type == PlayerSoulShardType.DEPTHS) {
            float dmg = ReverseAoE(player, 10f) * .35f;
            AoE(player, 10f, dmg);
        } else if (type == PlayerSoulShardType.MAGE) {
            player.combatAbilities.ReduceCD(1.5f, 10f);
        } else if (type == PlayerSoulShardType.DEMON) {
            AoE(player, 10f, player.stats.maxhp * 1.5f);
            player.stats.hp *= .7f;
        } else if (type == PlayerSoulShardType.TIDES) {
            AoE(player, 10f, player.stats.atk * (1f + player.stats.GetTotalBuff(PlayerBuffType.BURST_DMG)));
            Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemies) {
                if (Vector3.Distance(player.transform.position, enemy.transform.position) < 10f) {
                    enemy.bleed = 15f;
                }
            }
        } else if (type == PlayerSoulShardType.FLAMES) {
            player.StartCoroutine(_FlamesSS(player));
        }
    }

    private IEnumerator _FlamesSS(PlayerCombat player) {
        for (int i = 0; i < 5; i++) {
            AoE(player, 10f, player.stats.atk * .25f, true);
            Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemies) {
                if (Vector3.Distance(player.transform.position, enemy.transform.position) < 10) {
                    enemy.flaming = 25f;
                    GameObject.Instantiate(player.firePrefab, enemy.transform.position, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(2.5f);
        }
    }

    private float ReverseAoE(PlayerCombat pc, float radius) {
        float hpacc = 0f;
        List<Enemy> hittable = Enemy.GetEnemiesInRadius(pc.transform.position, radius);
        foreach (Enemy enemy in hittable) {
            hpacc += enemy.maxhp;
        }
        return hpacc;
    }

    private void AoE(PlayerCombat pc, float radius, float directdmg, bool trueDMG = false) {
        List<Enemy> hittable = Enemy.GetEnemiesInRadius(pc.transform.position, radius);
        foreach (Enemy enemy in hittable) {
            enemy.TakeDamage(directdmg, pc, trueDMG);
            pc.OnHitEnemy(enemy);
            pc.lasthitenemy = enemy;
        }
    }

    public bool Equality(PlayerSoulShard obj) {
        return obj.level == level && obj.type == type;
    }
}