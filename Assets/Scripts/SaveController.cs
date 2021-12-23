using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour {
    public static SaveController instance;

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

        if (File.Exists(Application.persistentDataPath + "/" + gamedef.channel + "player.dat")) {
            PlayerStats stats = JsonUtility.FromJson<PlayerStats>(File.ReadAllText(Application.persistentDataPath + "/" + gamedef.channel + "player.dat"));

            if (stats.checksum != stats.Checksum()) {
                stats.level = 1;
                stats.xp = 0;
            }

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

            if (info.checksum != info.Checksum()) {
                info.coins = 0;
                info.bmatter = 0;
                info.bstars = 0;
            }

            resourceController.coins = info.coins;
            resourceController.bstars = info.bstars;
            resourceController.bmatter = info.bmatter;
        }
    }

    public void SaveAll() {
        string gamedefstr = File.ReadAllText(Application.streamingAssetsPath + "/gamedef.json");
        GameDef gamedef = JsonUtility.FromJson<GameDef>(gamedefstr);

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "player.dat", JsonUtility.ToJson(playerController.stats.Verify()));

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "abilities.dat", JsonUtility.ToJson(new PlayerAbilityList(abilityController.abilities)));

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "buffs.dat", JsonUtility.ToJson(new PlayerBuffList(buffController.buffs)));

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "soulshards.dat", JsonUtility.ToJson(new PlayerShardList(shardController.shards)));

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "quests.dat", JsonUtility.ToJson(new PlayerQuestInfo(storyController)));

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "resources.dat", JsonUtility.ToJson(new PlayerResourceInfo(resourceController).Verify()));
    }
}

[System.Serializable]
public class PlayerBuffList {
    public List<PlayerBuff> array;

    public PlayerBuffList() { }
    public PlayerBuffList(List<PlayerBuff> buffs) {
        this.array = buffs;
    }
}

[System.Serializable]
public class PlayerAbilityList {
    public List<PlayerAbility> array;

    public PlayerAbilityList() { }
    public PlayerAbilityList(List<PlayerAbility> abilities) {
        array = abilities;
    }
}

[System.Serializable]
public class PlayerShardList {
    public List<PlayerSoulShard> array;

    public PlayerShardList() { }
    public PlayerShardList(List<PlayerSoulShard> shards) {
        array = shards;
    }
}

[System.Serializable]
public class PlayerQuestInfo {
    public string active;
    public string story;

    public PlayerQuestInfo() { }
    public PlayerQuestInfo(StorylineController sc) {
        active = sc.activeQuestId;
        story = sc.storyQuestId;
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