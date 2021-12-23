using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Quest : MonoBehaviour {
    public string id = "";
    public bool isStoryOnly;

    public UnityEvent onPrepared;
    public UnityEvent onFinished;

    private bool active;
    private bool wasActive;

    void Update() {
        active = StorylineController.instance.activeQuestId == id
            || (StorylineController.instance.storyQuestId == id && isStoryOnly);
        
        if (active && !wasActive) {
            onPrepared.Invoke();
        } else if (!active && wasActive) {
            onFinished.Invoke();
        }

        if (active && !isStoryOnly) {
            StorylineController.instance.ots.target = transform;
        }

        wasActive = active;
    }
}
