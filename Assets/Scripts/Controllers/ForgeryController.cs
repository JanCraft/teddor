using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeryController : MonoBehaviour {
    public GameObject menu;
    public Text alldata;
    public Text buyinfo;
    private int cursor;

    public PlayerCombat player;
    public ResourceController resources;
    public BuffController buffs;

    public AudioSource sfxCash;
    public AudioSource sfxSlide;
    public AudioSource sfxClick;

    private List<ForgeryOption> options = new List<ForgeryOption>();

    private void Start() {
        TranslateKey.Init();
    }

    void OnEnable() {
        alldata.text = GetOptionsAsText();
        menu.SetActive(true);
    }

    void OnDisable() {
        menu.SetActive(false);
    }

    void Update() {
        bool changed = false;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)) enabled = false;

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            cursor--;
            changed = true;
            sfxSlide.Play();
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            cursor++;
            changed = true;
            sfxSlide.Play();
        } else if (Input.GetKeyDown(KeyCode.Return)) {
            if (options.Count > 0) {
                ForgeryOption opt = options[cursor];

                bool can_spend = true;
                if (opt.reforge) {
                    int bufidx = buffs.buffs.FindIndex((x) => x.type == opt.type);
                    float bval = buffs.buffs[bufidx].value;
                    float aval = 0f;
                    if (buffs.buffs[bufidx].reforged) {
                        aval = player.stats.level + 35;
                    } else {
                        aval = player.stats.level + 25;
                    }
                    if (bval >= aval) can_spend = false;
                }

                int priceCoins = opt.reforge ? 2250 : 1750;
                int priceMatter = opt.reforge ? 125 : 75;

                if (can_spend && resources.coins >= priceCoins && resources.bmatter >= priceMatter) {
                    if (opt.reforge) {
                        buffs.buffs.Sort((x, y) => {
                            return x.value.CompareTo(y.value);
                        });
                        
                        buffs.buffs.Sort((x, y) => {
                            return x.reforged.CompareTo(y.reforged);
                        });

                        int bufidx = buffs.buffs.FindIndex((x) => x.type == opt.type);
                        PlayerBuff bufi = FindInReturn(buffs.buffs[bufidx], player.stats.buffs);

                        if (buffs.buffs[bufidx].reforged) {
                            buffs.buffs[bufidx].value = player.stats.level + 35;
                        } else {
                            buffs.buffs[bufidx].value = player.stats.level + 25;
                            buffs.buffs[bufidx].reforged = true;
                        }

                        if (bufi != null) {
                            bufi.value = buffs.buffs[bufidx].value;
                            bufi.reforged = true;

                            player.stats.Calculate();
                        }
                    } else {
                        PlayerBuff pb = new PlayerBuff();
                        pb.type = opt.type;
                        pb.value = player.stats.level + 10;
                        pb.reforged = false;

                        buffs.buffs.Add(pb);
                    }
                    resources.coins -= priceCoins;
                    resources.bmatter -= priceMatter;
                    sfxCash.Play();
                } else {
                    sfxClick.Play();
                }
                changed = true;
                CalcOptions();
            }
        }

        if (changed) {
            if (cursor < 0) cursor = options.Count - 1;
            if (cursor >= options.Count) cursor = 0;
            alldata.text = GetOptionsAsText();
        }
    }

    void CalcOptions() {
        options.Clear();

        buffs.buffs.Sort((x, y) => {
            return x.value.CompareTo(y.value);
        });

        buffs.buffs.Sort((x, y) => {
            return x.reforged.CompareTo(y.reforged);
        });

        foreach (PlayerBuffType type in System.Enum.GetValues(typeof(PlayerBuffType))) {
            int cnt = CountOfType(type);
            int bufidx = buffs.buffs.FindIndex((x) => x.type == type);
            if (bufidx != -1) {
                float bval = buffs.buffs[bufidx].value;
                float aval = 0f;
                if (buffs.buffs[bufidx].reforged) {
                    aval = player.stats.level + 35;
                } else {
                    aval = player.stats.level + 25;
                }
                if (bval >= aval) continue;
            }
            if (cnt < 3)
                options.Add(new ForgeryOption(type, false));
            if (cnt > 0)
                options.Add(new ForgeryOption(type, true));
        }
    }

    string GetOptionsAsText() {
        string outp = "";
        buyinfo.text = "";

        CalcOptions();

        int end = Mathf.Min(options.Count, 10);
        int drawn = 0;
        for (int i = 0; i < end; i++) {
            ForgeryOption opt = options[i];
            if (opt.reforge) {
                int bufidx = buffs.buffs.FindIndex((x) => x.type == opt.type);
                float bval = buffs.buffs[bufidx].value;
                float aval = 0f;
                if(buffs.buffs[bufidx].reforged) {
                    aval = player.stats.level + 35;
                } else {
                    aval = player.stats.level + 25;
                }
                if (bval >= aval) continue;
            }

            drawn++;

            if (i == cursor) {
                outp += "> ";
                int priceCoins = opt.reforge ? 2250 : 1750;
                int priceMatter = opt.reforge ? 125 : 75;

                if (resources.coins < priceCoins) buyinfo.text += "<color=red>";
                buyinfo.text += priceCoins;
                if (resources.coins < priceCoins) buyinfo.text += "</color>";
                buyinfo.text += "\n";
                if (resources.bmatter < priceMatter) buyinfo.text += "<color=red>";
                buyinfo.text += priceMatter;
                if (resources.bmatter < priceMatter) buyinfo.text += "</color>";
                buyinfo.text += "\n\n";

                if (opt.reforge) {
                    int bufidx = buffs.buffs.FindIndex((x) => x.type == opt.type);
                    buyinfo.text += "<color=red>";
                    buyinfo.text += buffs.buffs[bufidx].value;
                    buyinfo.text += "%</color> <color=yellow>>></color> <color=green>";
                    if (buffs.buffs[bufidx].reforged) {
                        buyinfo.text += (player.stats.level + 35).ToString();
                    } else {
                        buyinfo.text += (player.stats.level + 25).ToString();
                    }
                    buyinfo.text += "%</color>";
                }

                buyinfo.text += "\n\n";

                if (resources.coins >= priceCoins && resources.bmatter >= priceMatter) {
                    buyinfo.text += TranslateKey.Translate("ui.foundry.forge") + "\n";
                }
            }

            outp += opt.type.ToString().Replace('_', ' ') + " <color=yellow>";
            if (opt.reforge) outp += TranslateKey.Translate("ui.foundry.reforge");
            outp += "</color>\n";
        }

        if (drawn == 0) {
            outp += "<color=red>all upgrades maxed</color>";
            buyinfo.text += "\n\n\n";
        }

        buyinfo.text += TranslateKey.Translate("ui.foundry.back");

        return outp;
    }

    private int CountOfType(PlayerBuffType type) {
        int count = 0;
        foreach (PlayerBuff itm in buffs.buffs)
            if (itm.type == type) count++;
        return count;
    }

    private PlayerBuff FindInReturn(PlayerBuff obj, List<PlayerBuff> list) {
        foreach (PlayerBuff itm in list)
            if (itm.Equality(obj)) return itm;
        return null;
    }
}

[System.Serializable]
public class ForgeryOption {
    public PlayerBuffType type;
    public bool reforge;

    public ForgeryOption(PlayerBuffType type, bool reforge) {
        this.type = type;
        this.reforge = reforge;
    }
}