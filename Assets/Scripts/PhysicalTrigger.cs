using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicalTrigger : MonoBehaviour {
    public string triggerTag = "Player";
    public UnityEvent onEnter;
    public UnityEvent onLeave;

    void OnTriggerEnter(Collider coll) {
        if (coll.CompareTag(triggerTag)) onEnter.Invoke();
    }

    void OnTriggerExit(Collider coll) {
        if (coll.CompareTag(triggerTag)) onLeave.Invoke();
    }
}
