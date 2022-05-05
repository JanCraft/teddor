using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CodeTrigger : MonoBehaviour {
    public int lvlRequired = 1;
    public int lvlMaximum = int.MaxValue;
    public bool requiresEmptiness = false;

    public UnityEvent onTriggered;

    private bool hasTriggered = false;
    private PlayerCombat pc;

    void Start() {
        pc = FindObjectOfType<PlayerCombat>();
    }

    void Update() {
        if (!hasTriggered && pc.stats.level >= lvlRequired && pc.stats.level < lvlMaximum) {
            if (requiresEmptiness) {
                if (transform.childCount == 0) {
                    hasTriggered = true;
                    onTriggered.Invoke();
                }
            } else {
                hasTriggered = true;
                onTriggered.Invoke();
            }
        }
    }
}