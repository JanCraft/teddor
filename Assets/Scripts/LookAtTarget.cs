using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour {
    public Transform target;
    public bool autoToCamera;
    public bool autoToPlayer;

    private void Start() {
        if (autoToCamera) target = Camera.main.transform;
        if (autoToPlayer) target = FindObjectOfType<PlayerController>().transform;
    }

    void Update() {
        transform.LookAt(target);
    }
}
