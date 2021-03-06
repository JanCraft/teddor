using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Banner : MonoBehaviour {
    public Text bannerItem;
    public Image bannerNewIcon;
    public Text bannerNewItem;
    public Animation bannerNewAnim;
    public GameObject bannerPanel;

    public Sprite bmatter;
    public Sprite[] ability;
    public Sprite soulshard;

    public ResourceController res;
    public PlayerCombat player;
    public AbilityController abc;
    public SoulShardController ssc;

    public AudioSource sfxPay;
    public AudioSource sfxPayGG;
    public AudioSource sfxError;

    public static bool playing;
    public bool open;

    public static int NEW_BANNER = 2;

    void Update() {
        playing = bannerNewAnim.isPlaying;
        bannerPanel.SetActive(open);
        if (open) {
            if (Input.GetKeyDown(KeyCode.Escape) && !bannerNewAnim.isPlaying) open = false;
            if (Input.GetKeyDown(KeyCode.Space) && !bannerNewAnim.isPlaying) open = false;

            if (!bannerNewAnim.isPlaying) bannerItem.text = "Soul Shard of the " + GetCSoulShard() + "\n\n" + PlayerPrefs.GetInt("teddor.rolls", 0) + "/40";

            if (Input.GetKeyDown(KeyCode.Return) && !bannerNewAnim.isPlaying) {
                if (Input.GetKey(KeyCode.LeftShift)) {
                    StartCoroutine(DoMultiPull());
                } else {
                    DoSinglePull();
                }
            }
        }
    }

    private IEnumerator DoMultiPull() {
        if (res.bstars > 9) {
            bannerNewAnim.Play("BannerLong");

            List<BannerType> bannr = new List<BannerType>();
            for (int i = 0; i < 10; i++) {
                bannr.Add(RollType());
            }
            bannr.Sort((a, b) => b - a);
            if (bannr[0] == BannerType.SOULSHARD) sfxPayGG.Play(); else sfxPay.Play();

            yield return new WaitForSeconds(4.5f);
            bool isFirst = true;
            foreach (BannerType type in bannr) {
                if (type == BannerType.BMATTER) {
                    res.bmatter += 15 + Random.Range(0, 11);
                    bannerNewItem.text = "bMatter";
                    bannerNewIcon.sprite = bmatter;
                } else if (type == BannerType.ABILITY4) {
                    PlayerAbilityType t = GetRandom4();
                    AddAbility(t);
                    bannerNewItem.text = "4* " + t;
                    bannerNewIcon.sprite = ability[(int) t];
                } else if (type == BannerType.ABILITY5) {
                    PlayerAbilityType t = GetRandom5();
                    AddAbility(t);
                    bannerNewItem.text = "5* " + t;
                    bannerNewIcon.sprite = ability[(int) t];
                } else if (type == BannerType.ABILITY6) {
                    PlayerAbilityType t = GetRandom6();
                    AddAbility(t);
                    bannerNewItem.text = "6* " + t;
                    bannerNewIcon.sprite = ability[(int) t];
                } else if (type == BannerType.SOULSHARD) {
                    PlayerSoulShardType ss = GetCSoulShard();
                    PlayerPrefs.SetInt("teddor.css", PlayerPrefs.GetInt("teddor.css", 0) + 1);
                    AddSoulShard(ss);
                    bannerNewItem.text = "Soul Shard of the " + ss;
                    bannerNewIcon.sprite = soulshard;
                }
                res.bstars--;
                SaveController.instance.SaveAll();
                if (isFirst) {
                    isFirst = false;
                    yield return new WaitForSeconds(.15f);
                }
                yield return new WaitForSeconds(1.75f);
            }
        } else {
            sfxError.Play();
        }
    }

    private void DoSinglePull() {
        if (res.bstars > 0) {
            res.bstars--;
            bannerNewAnim.Play("Banner");

            BannerType type = RollType();
            if (type == BannerType.BMATTER) {
                res.bmatter += 15 + Random.Range(0, 11);
                bannerNewItem.text = "bMatter";
                bannerNewIcon.sprite = bmatter;
                sfxPay.Play();
            } else if (type == BannerType.ABILITY4) {
                PlayerAbilityType t = GetRandom4();
                AddAbility(t);
                bannerNewItem.text = "4* " + t;
                bannerNewIcon.sprite = ability[(int) t];
                sfxPay.Play();
            } else if (type == BannerType.ABILITY5) {
                PlayerAbilityType t = GetRandom5();
                AddAbility(t);
                bannerNewItem.text = "5* " + t;
                bannerNewIcon.sprite = ability[(int) t];
                sfxPay.Play();
            } else if (type == BannerType.ABILITY6) {
                PlayerAbilityType t = GetRandom6();
                AddAbility(t);
                bannerNewItem.text = "6* " + t;
                bannerNewIcon.sprite = ability[(int) t];
                sfxPay.Play();
            } else if (type == BannerType.SOULSHARD) {
                PlayerSoulShardType ss = GetCSoulShard();
                PlayerPrefs.SetInt("teddor.css", PlayerPrefs.GetInt("teddor.css", 0) + 1);
                AddSoulShard(ss);
                bannerNewItem.text = "Soul Shard of the " + ss;
                bannerNewIcon.sprite = soulshard;
                sfxPayGG.Play();
            }

            SaveController.instance.SaveAll();
        } else {
            sfxError.Play();
        }
    }

    private PlayerSoulShardType GetCSoulShard() {
        PlayerSoulShardType[] choice = {
            PlayerSoulShardType.MAGE,
            PlayerSoulShardType.CROWNED,
            PlayerSoulShardType.DEPTHS,
            PlayerSoulShardType.DEMON,
            PlayerSoulShardType.WINGED,
            PlayerSoulShardType.TIDES,
            PlayerSoulShardType.FLAMES,
        };
        if (PlayerPrefs.GetInt("teddor.newbanner", 0) < NEW_BANNER) {
            PlayerPrefs.SetInt("teddor.css", choice.Length - 1);
            PlayerPrefs.SetInt("teddor.newbanner", NEW_BANNER);
        }
        int cur = PlayerPrefs.GetInt("teddor.css", 0);
        return choice[cur % choice.Length];
    }

    private void AddAbility(PlayerAbilityType type) {
        PlayerAbility ab = new PlayerAbility();
        ab.type = type;
        ab.level = 1;

        foreach (PlayerAbility abb in abc.abilities) {
            if (abb.type == type) {
                abb.level++;
                if (player.stats.ability.type == type) {
                    player.stats.ability.level = abb.level;
                }
                return;
            }
        }

        abc.abilities.Add(ab);
    }

    private void AddSoulShard(PlayerSoulShardType type) {
        PlayerSoulShard ss = new PlayerSoulShard();
        ss.type = type;
        ss.level = 1;

        foreach (PlayerSoulShard ssb in ssc.shards) {
            if (ssb.type == type) {
                ssb.level++;
                if (player.stats.soulShard.type == type) {
                    player.stats.soulShard.level = ssb.level;
                }
                return;
            }
        }

        ssc.shards.Add(ss);
    }

    private PlayerAbilityType GetRandom4() {
        PlayerAbilityType[] choice = {
            PlayerAbilityType.BLINK,
            PlayerAbilityType.HEAL,
            PlayerAbilityType.RANGED,
            PlayerAbilityType.SHIELD
        };
        return choice[Random.Range(0, choice.Length)];
    }

    private PlayerAbilityType GetRandom5() {
        PlayerAbilityType[] choice = {
            PlayerAbilityType.EARTHQUAKE,
            PlayerAbilityType.METEOR
        };
        return choice[Random.Range(0, choice.Length)];
    }

    private PlayerAbilityType GetRandom6() {
        PlayerAbilityType[] choice = {
            PlayerAbilityType.BOLT,
            PlayerAbilityType.WATERJET,
            PlayerAbilityType.IGNITION
        };
        return choice[Random.Range(0, choice.Length)];
    }

    private BannerType RollType() {
        if (PlayerPrefs.GetInt("teddor.rolls", 0) >= 39) {
            PlayerPrefs.SetInt("teddor.rolls", 0);
            return BannerType.SOULSHARD;
        }

        float rtype = Random.value;
        
        if (rtype < .05f) {
            PlayerPrefs.SetInt("teddor.rolls", 0);
            return BannerType.SOULSHARD;
        } else if (rtype < .15f) {
            PlayerPrefs.SetInt("teddor.rolls", PlayerPrefs.GetInt("teddor.rolls", 0) + 1);
            return BannerType.ABILITY6;
        } else if (rtype < .20f) {
            PlayerPrefs.SetInt("teddor.rolls", PlayerPrefs.GetInt("teddor.rolls", 0) + 1);
            return BannerType.ABILITY6;
        } else if (rtype < .40f) {
            PlayerPrefs.SetInt("teddor.rolls", PlayerPrefs.GetInt("teddor.rolls", 0) + 1);
            return BannerType.ABILITY5;
        } else if (rtype < .70f) {
            PlayerPrefs.SetInt("teddor.rolls", PlayerPrefs.GetInt("teddor.rolls", 0) + 1);
            return BannerType.ABILITY4;
        }
        PlayerPrefs.SetInt("teddor.rolls", PlayerPrefs.GetInt("teddor.rolls", 0) + 1);
        return BannerType.BMATTER;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            open = true;
        }
    }
}

public enum BannerType {
    BMATTER, ABILITY4, ABILITY5, ABILITY6, SOULSHARD
}