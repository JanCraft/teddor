using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulShardController : MonoBehaviour {
    public Text alldata;
    private int cursor;

    public PlayerController player;
    public List<PlayerSoulShard> shards;

    void OnEnable() {
        alldata.text = GetShardsAsText();
    }

    void Update() {
        bool changed = false;

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            cursor--;
            changed = true;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            cursor++;
            changed = true;
        } else if (Input.GetKeyDown(KeyCode.Return)) {
            if (shards.Count > 0) {
                PlayerSoulShard shard = shards[cursor];
                player.stats.soulShard = shard;
                changed = true;
            }
        }

        if (changed) {
            if (cursor < 0) cursor = shards.Count - 1;
            if (cursor >= shards.Count) cursor = 0;
            alldata.text = GetShardsAsText();
        }
    }

    string GetShardsAsText() {
        string outp = "";

        int start = 0;
        if (cursor > 10) {
            start = cursor - 10;
        }
        int end = start + Mathf.Min(shards.Count-start, 10);
        for (int i = start; i < end; i++) {
            PlayerSoulShard shard = shards[i];
            if (i == cursor) outp += "> ";
            if (player.stats.soulShard.Equality(shard))
                outp += "<color=green>*</color> SS. ";
            outp += shard.type.ToString().Replace('_', ' ') + " <color=red>[ Lv. " + shard.level.ToString() + " ]</color>\n";
        }

        if (shards.Count == 0) {
            outp += "<color=red>no shards available</color>";
        }

        return outp;
    }
}
