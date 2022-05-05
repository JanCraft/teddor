using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitLineDistance : MonoBehaviour {
    public Transform target;
    public bool autoToCamera;
    public bool autoToPlayer;
    public float mult = 2f;

    private LineRenderer lr;

    private void Start() {
        if (autoToCamera) target = Camera.main.transform;
        if (autoToPlayer) target = FindObjectOfType<PlayerController>().transform;

        lr = GetComponent<LineRenderer>();
    }

    void Update() {
        float dst = Vector3.Distance(transform.position, target.position);
        lr.SetPosition(1, Vector3.forward * dst * mult);
    }
}
