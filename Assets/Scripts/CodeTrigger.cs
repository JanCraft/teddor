using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CodeTrigger : MonoBehaviour {
    public int lvlRequired = 1;
    public bool requiresEmptiness = false;

    public UnityEvent onTriggered;

    private bool hasTriggered = false;
    private PlayerController pc;

    void Start() {
        pc = FindObjectOfType<PlayerController>();
    }

    void Update() {
        if (!hasTriggered && pc.stats.level >= lvlRequired) {
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