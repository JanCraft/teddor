using System.Collections.Generic;
using System.Collections;
using UnityEngine;

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

    public float xptonext { get; private set; }

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
        xptonext = level * 100f + Mathf.Floor(level / 25) * 1000f + Mathf.Floor(level / 50) * 1000f + Mathf.Floor(level / 75) * 1000f + Mathf.Floor(Mathf.Max(0, level - 90)) * 1000f * Mathf.Min(Mathf.Max(level - 99, 0), 1);

        if (level >= 99) return; // level limited to 99 (this version)

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
