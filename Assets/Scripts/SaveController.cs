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
    }

    public void SaveAll() {
        string gamedefstr = File.ReadAllText(Application.streamingAssetsPath + "/gamedef.json");
        GameDef gamedef = JsonUtility.FromJson<GameDef>(gamedefstr);

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "player.dat", JsonUtility.ToJson(playerController.stats));

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "abilities.dat", JsonUtility.ToJson(new PlayerAbilityList(abilityController.abilities)));

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "buffs.dat", JsonUtility.ToJson(new PlayerBuffList(buffController.buffs)));

        File.WriteAllText(Application.persistentDataPath + "/" + gamedef.channel + "soulshards.dat", JsonUtility.ToJson(new PlayerShardList(shardController.shards)));
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
