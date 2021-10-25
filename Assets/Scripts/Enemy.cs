using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float hp = 100f;

    void Start() {
        
    }

    void Update() {
        
    }

    public void TakeDamage(float amount, Vector3 from) {
        hp -= amount;

        hp = Mathf.Max(hp, 0f);
    }
}
