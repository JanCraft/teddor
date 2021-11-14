using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Quest : MonoBehaviour {
    public string id = "";

    public UnityEvent onPrepared;
    public UnityEvent onFinished;

    private bool active;
    private bool wasActive;

    void Update() {
        active = StorylineController.instance.activeQuestId == id;
        
        if (active && !wasActive) {
            onPrepared.Invoke();
        } else if (!active && wasActive) {
            onFinished.Invoke();
        }

        wasActive = active;
    }
}
