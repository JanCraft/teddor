using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibrations : MonoBehaviour {
    public float range = .1f;
    private Vector3 offset;

    void Start() {
        offset = transform.localPosition;
    }

    void Update() {
        transform.localPosition = offset + Random.insideUnitSphere * range;
    }
}
