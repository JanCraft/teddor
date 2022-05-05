using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour {
    public static DialogController instance;

    public GameObject dialogBox;
    public Text speakerTxt;
    public Text contentTxt;

    public AudioSource typesound;

    public bool hasNext;
    public bool next;
    public bool isOpen;

    [HideInInspector]
    public string contentslow;
    private int contentslowidx;

    void Start() {
        instance = this;
    }

    void Update() {
        dialogBox.SetActive(isOpen);
        if (isOpen) {
            if (contentTxt.text.Length < contentslow.Length && !typesound.isPlaying) {
                contentTxt.text += contentslow[contentslowidx];
                contentslowidx++;

                typesound.Play();
                typesound.pitch = Random.Range(.75f, 1f);
            }
            if (Input.GetKey(KeyCode.Return)) typesound.pitch = 3.5f;
            if (Input.GetKeyDown(KeyCode.Return)) {
                if (contentTxt.text.Length >= contentslow.Length) {
                    if (hasNext) {
                        ConsumeCache();
                        next = true;
                    } else {
                        isOpen = false;
                    }
                }
            }
        }
    }

    public void ConsumeCache() {
        contentslowidx = 0;
        contentTxt.text = "";
    }
}
