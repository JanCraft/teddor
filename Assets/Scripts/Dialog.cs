using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Dialog : MonoBehaviour {
    public string[] speakers;
    public string[] contents;

    public DialogEvents events;

    private int current = 0;
    public bool trigger;
    public bool physicalTrigger;

    private void Start() {
        TranslateKey.Init();
    }

    void Update() {
        if (trigger) {
            trigger = false;
            current = 0;
            events.onTrigger.Invoke();

            Refresh();
        }
        if (DialogController.instance.next) {
            current++;
            Refresh();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (physicalTrigger && other.CompareTag("Player")) {
            trigger = true;
        }
    }

    public void Trigger() {
        trigger = true;
    }

    public void DeTrigger() {
        trigger = false;
    }

    void Refresh() {
        DialogController.instance.next = false;
        DialogController.instance.isOpen = true;
        DialogController.instance.hasNext = current < contents.Length - 1;
        if (speakers[current].Trim() == "") {
            DialogController.instance.speakerTxt.text = "";
        } else {
            DialogController.instance.speakerTxt.text = TranslateKey.Translate(speakers[current]);
        }
        DialogController.instance.contentslow = TranslateKey.Translate(contents[current]);
        DialogController.instance.ConsumeCache();

        if (!DialogController.instance.hasNext) events.onFinish.Invoke();
    }
}

[System.Serializable]
public class DialogEvents {
    public UnityEvent onTrigger;
    public UnityEvent onFinish;
}