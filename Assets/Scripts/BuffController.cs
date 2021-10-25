using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffController : MonoBehaviour {
    public Text alldata;
    private int cursor;

    public PlayerController player;
    public List<PlayerBuff> buffs;

    void OnEnable() {
        alldata.text = GetBuffsAsText();
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
            if (buffs.Count > 0) {
                PlayerBuff buff = buffs[cursor];
                if (player.stats.buffs.Contains(buff)) {
                    player.stats.buffs.Remove(buff);
                } else {
                    if (player.stats.buffs.Count < 6) {
                        player.stats.buffs.Add(buff);
                    }
                }
                player.stats.Calculate();
                changed = true;
            }
        }

        if (changed) {
            if (cursor < 0) cursor = buffs.Count - 1;
            if (cursor >= buffs.Count) cursor = 0;
            alldata.text = GetBuffsAsText();
        }
    }

    string GetBuffsAsText() {
        string outp = "";

        int start = 0;
        if (cursor > 10) {
            start = cursor - 10;
        }
        int end = start + Mathf.Min(buffs.Count, 10);
        for (int i = start; i < end; i++) {
            PlayerBuff buff = buffs[i];
            if (i == cursor) outp += "> ";
            if (FindIn(buff, player.stats.buffs))
                outp += "<color=green>*</color> ";
            outp += buff.type.ToString().Replace('_', ' ') + " <color=red>+" + ((int) buff.value).ToString() + "%</color> <color=yellow>";
            if (buff.reforged) outp += "[ RF ]";
            outp += "</color>\n";
        }

        if (buffs.Count == 0) {
            outp += "<color=red>no buffs available</color>";
        }

        return outp;
    }

    private bool FindIn(PlayerBuff obj, List<PlayerBuff> list) {
        foreach (PlayerBuff itm in list)
            if (itm.Equality(obj)) return true;
        return false;
    }
}
