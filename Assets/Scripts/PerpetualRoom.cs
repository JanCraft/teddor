using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PerpetualRoom : MonoBehaviour {
    public int count = 5;
    private int c = 0;
    public UnityEvent onTrigger;

    public void Trigger() {
        c++;
        if (c >= count) {
            c = 0;
            onTrigger.Invoke();
        }
    }
}
