using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptedDeath : MonoBehaviour {
    public UnityEvent onTrigger;
    private PlayerCombat player;

    void Start() {
        player = FindObjectOfType<PlayerCombat>();
        player.canDie = false;
    }

    void Update() {
        if (player.hasDied) {
            player.stats.hp = player.stats.maxhp;
            player.canDie = true;
            player.hasDied = false;
            onTrigger.Invoke();
        }
    }
}
