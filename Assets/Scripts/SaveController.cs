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
        if (File.Exists(Application.persistentDataPath + "/player.dat")) {
            PlayerStats stats = JsonUtility.FromJson<PlayerStats>(File.ReadAllText(Application.persistentDataPath + "/player.dat"));
            playerController.stats = stats;
        }

        if (File.Exists(Application.persistentDataPath + "/abilities.dat")) {
            PlayerAbilityList abilities = JsonUtility.FromJson<PlayerAbilityList>(File.ReadAllText(Application.persistentDataPath + "/abilities.dat"));
            abilityController.abilities = abilities.array;
        }

        if (File.Exists(Application.persistentDataPath + "/buffs.dat")) {
            PlayerBuffList buffs = JsonUtility.FromJson<PlayerBuffList>(File.ReadAllText(Application.persistentDataPath + "/buffs.dat"));
            buffController.buffs = buffs.array;
        }

        if (File.Exists(Application.persistentDataPath + "/soulshards.dat")) {
            PlayerShardList shards = JsonUtility.FromJson<PlayerShardList>(File.ReadAllText(Application.persistentDataPath + "/soulshards.dat"));
            shardController.shards = shards.array;
        }
    }

    public void SaveAll() {
        File.WriteAllText(Application.persistentDataPath + "/player.dat", JsonUtility.ToJson(playerController.stats));

        File.WriteAllText(Application.persistentDataPath + "/abilities.dat", JsonUtility.ToJson(new PlayerAbilityList(abilityController.abilities)));

        File.WriteAllText(Application.persistentDataPath + "/buffs.dat", JsonUtility.ToJson(new PlayerBuffList(buffController.buffs)));

        File.WriteAllText(Application.persistentDataPath + "/soulshards.dat", JsonUtility.ToJson(new PlayerShardList(shardController.shards)));
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
