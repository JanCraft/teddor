using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [Header("Internal")]
    public PlayerCombat combat;
    public PlayerCombatAbilities abilities;
    public PlayerMovement movement;
    public PlayerUI ui;

    [Header("External")]
    public Banner banner;
    public ForgeryController forgery;
    public DialogController dialog;

    void Update() {
        movement.enabled = abilities.enabled = !PauseMenu.open && !dialog.isOpen && !PlayerCombatAbilities.soulShardAnimationPause && !combat.hasDied && !forgery.enabled && !banner.open;
    }

    private void OnEnable() {
        combat.enabled = true;
        abilities.enabled = true;
        movement.enabled = true;
        ui.enabled = true;
    }

    private void OnDisable() {
        combat.enabled = false;
        abilities.enabled = false;
        movement.enabled = false;
        ui.enabled = false;
    }
}
