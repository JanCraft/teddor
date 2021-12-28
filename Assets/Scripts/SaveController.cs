using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour {
    public static SaveController instance;
    public static readonly int MIGRATION = 1;

    public AbilityController abilityController;
    public BuffController buffController;
    public SoulShardController shardController;
    public PlayerController playerController;
    public StorylineController storyController;
    public ResourceController resourceController;

    void Start() {
        instance = this;

        LoadAll();
    }

    private void OnDisable() {
        SaveAll();
    }

    public void LoadAll() {
        string gamedefstr = File.ReadAllText(Application.streamingAssetsPath + "/gamedef.json");
        GameDef gamedef = JsonUtility.FromJson<GameDef>(gamedefstr);

        if (PlayerPrefs.GetInt("teddor_migrated", 0) == MIGRATION) {
            if (PlayerPrefs.HasKey("teddor_save_" + gamedef.channel + "player")) {
                PlayerStats stats = JsonUtility.FromJson<PlayerStats>(PlayerPrefs.GetString("teddor_save_" + gamedef.channel + "player"));

                if (stats.checksum != stats.Checksum()) {
                    stats.level = 1;
                    stats.xp = 0;
                }

                playerController.stats = stats;
            }

            if (PlayerPrefs.HasKey("teddor_save_" + gamedef.channel + "abilities")) {
                PlayerAbilityList abilities = JsonUtility.FromJson<PlayerAbilityList>(PlayerPrefs.GetString("teddor_save_" + gamedef.channel + "abilities"));

                if (abilities.checksum != abilities.Checksum()) {
                    abilities.array.Clear();
                    PlayerAbility a = new PlayerAbility();
                    a.level = 1;
                    a.type = PlayerAbilityType.RANGED;
                    abilities.array.Add(a);
                    playerController.stats.ability = a;
                }

                abilityController.abilities = abilities.array;
            }

            if (PlayerPrefs.HasKey("teddor_save_" + gamedef.channel + "buffs")) {
                PlayerBuffList buffs = JsonUtility.FromJson<PlayerBuffList>(PlayerPrefs.GetString("teddor_save_" + gamedef.channel + "buffs"));

                if (buffs.checksum != buffs.Checksum()) {
                    buffs.array.Clear();
                    playerController.stats.buffs.Clear();
                }

                buffController.buffs = buffs.array;
            }

            if (PlayerPrefs.HasKey("teddor_save_" + gamedef.channel + "soulshards")) {
                PlayerShardList shards = JsonUtility.FromJson<PlayerShardList>(PlayerPrefs.GetString("teddor_save_" + gamedef.channel + "soulshards"));

                if (shards.checksum != shards.Checksum()) {
                    shards.array.Clear();
                    playerController.stats.soulShard.level = 0;
                    playerController.stats.soulShard.type = PlayerSoulShardType.NONE;
                }

                shardController.shards = shards.array;
            }

            if (PlayerPrefs.HasKey("teddor_save_" + gamedef.channel + "quests")) {
                PlayerQuestInfo info = JsonUtility.FromJson<PlayerQuestInfo>(PlayerPrefs.GetString("teddor_save_" + gamedef.channel + "quests"));

                if (info.checksum != info.Checksum()) {
                    info.active = "1.01";
                    info.story = "";
                }

                storyController.activeQuestId = info.active;
                storyController.storyQuestId = info.story;
            }

            if (PlayerPrefs.HasKey("teddor_save_" + gamedef.channel + "resources")) {
                PlayerResourceInfo info = JsonUtility.FromJson<PlayerResourceInfo>(PlayerPrefs.GetString("teddor_save_" + gamedef.channel + "resources"));

                if (info.checksum != info.Checksum()) {
                    info.coins = 0;
                    info.bmatter = 0;
                    info.bstars = 0;
                }

                resourceController.coins = info.coins;
                resourceController.bstars = info.bstars;
                resourceController.bmatter = info.bmatter;
            }
        } else {
            Migrate(gamedef);
        }
    }

    public void SaveAll() {
        string gamedefstr = File.ReadAllText(Application.streamingAssetsPath + "/gamedef.json");
        GameDef gamedef = JsonUtility.FromJson<GameDef>(gamedefstr);

        PlayerPrefs.SetString("teddor_save_" + gamedef.channel + "player", JsonUtility.ToJson(playerController.stats.Verify()));
        PlayerPrefs.SetString("teddor_save_" + gamedef.channel + "abilities", JsonUtility.ToJson(new PlayerAbilityList(abilityController.abilities).Verify()));
        PlayerPrefs.SetString("teddor_save_" + gamedef.channel + "buffs", JsonUtility.ToJson(new PlayerBuffList(buffController.buffs).Verify()));
        PlayerPrefs.SetString("teddor_save_" + gamedef.channel + "soulshards", JsonUtility.ToJson(new PlayerShardList(shardController.shards).Verify()));
        PlayerPrefs.SetString("teddor_save_" + gamedef.channel + "quests", JsonUtility.ToJson(new PlayerQuestInfo(storyController).Verify()));
        PlayerPrefs.SetString("teddor_save_" + gamedef.channel + "resources", JsonUtility.ToJson(new PlayerResourceInfo(resourceController).Verify()));

        PlayerPrefs.Save();
    }

    private void Migrate(GameDef gamedef) {
        if (File.Exists(Application.persistentDataPath + "/" + gamedef.channel + "player.dat")) {
            PlayerStats stats = JsonUtility.FromJson<PlayerStats>(File.ReadAllText(Application.persistentDataPath + "/" + gamedef.channel + "player.dat"));
            playerController.stats = stats;
        }

        if (File.Exists(Application.persistentDataPath + "/" + gamedef.channel + "abilities.dat")) {
            PlayerAbilityList abilities = JsonUtility.FromJson<PlayerAbilityList>(File.ReadAllText(Application.persistentDataPath + "/" + gamedef.channel + "abilities.dat"));
            abilityController.abilities = abilities.array;
        }

        if (File.Exists(Application.persistentDataPath + "/" + gamedef.channel + "buffs.dat")) {
            PlayerBuffList buffs = JsonUtility.FromJson<PlayerBuffList>(File.ReadAllText(Application.persistentDataPath + "/" + gamedef.channel + "buffs.dat"));
            buffController.buffs = buffs.array;
        }

        if (File.Exists(Application.persistentDataPath + "/" + gamedef.channel + "soulshards.dat")) {
            PlayerShardList shards = JsonUtility.FromJson<PlayerShardList>(File.ReadAllText(Application.persistentDataPath + "/" + gamedef.channel + "soulshards.dat"));
            shardController.shards = shards.array;
        }

        if (File.Exists(Application.persistentDataPath + "/" + gamedef.channel + "quests.dat")) {
            PlayerQuestInfo info = JsonUtility.FromJson<PlayerQuestInfo>(File.ReadAllText(Application.persistentDataPath + "/" + gamedef.channel + "quests.dat"));
            storyController.activeQuestId = info.active;
            storyController.storyQuestId = info.story;
        }

        if (File.Exists(Application.persistentDataPath + "/" + gamedef.channel + "resources.dat")) {
            PlayerResourceInfo info = JsonUtility.FromJson<PlayerResourceInfo>(File.ReadAllText(Application.persistentDataPath + "/" + gamedef.channel + "resources.dat"));
            resourceController.coins = info.coins;
            resourceController.bstars = info.bstars;
            resourceController.bmatter = info.bmatter;
        }

        PlayerPrefs.SetInt("teddor_migrated", MIGRATION);
        SaveAll();
    }
}

[System.Serializable]
public class PlayerBuffList {
    public List<PlayerBuff> array;
    public int checksum;

    public PlayerBuffList() { }
    public PlayerBuffList(List<PlayerBuff> buffs) {
        this.array = buffs;
    }

    public int Checksum() {
        int cs = array.Count;
        for (int i = 0; i < array.Count; i++) {
            cs += array[i].identifier + (i * 3);
        }
        return cs;
    }

    public PlayerBuffList Verify() {
        checksum = Checksum();
        return this;
    }
}

[System.Serializable]
public class PlayerAbilityList {
    public List<PlayerAbility> array;
    public int checksum;

    public PlayerAbilityList() { }
    public PlayerAbilityList(List<PlayerAbility> abilities) {
        array = abilities;
    }

    public int Checksum() {
        int cs = array.Count;
        for (int i = 0; i < array.Count; i++) {
            cs += ((int) array[i].type + 1) * (array[i].level + 1) + (i * 3);
        }
        return cs;
    }

    public PlayerAbilityList Verify() {
        checksum = Checksum();
        return this;
    }
}

[System.Serializable]
public class PlayerShardList {
    public List<PlayerSoulShard> array;
    public int checksum;

    public PlayerShardList() { }
    public PlayerShardList(List<PlayerSoulShard> shards) {
        array = shards;
    }

    public int Checksum() {
        int cs = array.Count;
        for (int i = 0; i < array.Count; i++) {
            cs += ((int) array[i].type + 1) * (array[i].level + 1) + (i * 3);
        }
        return cs;
    }

    public PlayerShardList Verify() {
        checksum = Checksum();
        return this;
    }
}

[System.Serializable]
public class PlayerQuestInfo {
    public string active;
    public string story;
    public int checksum;

    public PlayerQuestInfo() { }
    public PlayerQuestInfo(StorylineController sc) {
        active = sc.activeQuestId;
        story = sc.storyQuestId;
    }

    public int Checksum() {
        return (active.Length + 1) * (active.GetHashCode() + 1) + (story.Length + 1) * (story.GetHashCode() + 1);
    }

    public PlayerQuestInfo Verify() {
        checksum = Checksum();
        return this;
    }
}

[System.Serializable]
public class PlayerResourceInfo {
    public int coins;
    public int bstars;
    public int bmatter;
    public int checksum;

    public PlayerResourceInfo() { }
    public PlayerResourceInfo(ResourceController rc) {
        coins = rc.coins;
        bstars = rc.bstars;
        bmatter = rc.bmatter;
    }

    public PlayerResourceInfo Verify() {
        checksum = Checksum();
        return this;
    }

    public int Checksum() {
        return (coins * 32823 + 3) * (bstars * 32823 + 5) * (bmatter * 32823 - 7);
    }
}