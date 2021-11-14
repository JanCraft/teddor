using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LimitLifetime : MonoBehaviour {
    public float lifetime = 1f;
    private float time;

    void Update() {
        time += Time.deltaTime;
        if (time >= lifetime) Destroy(gameObject);
    }
}