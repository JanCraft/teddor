using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerBuff {
    public PlayerBuffType type;
    public float value;
    public bool reforged;
    public int identifier = new System.Random().Next();

    public bool Equality(PlayerBuff obj) {
        return obj.value == value && obj.type == type && obj.reforged == reforged && obj.identifier == identifier;
    }
}