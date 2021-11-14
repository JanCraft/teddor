using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRotate : MonoBehaviour {
    public Vector3 axis;
    public float speed = 25f;

    void Update() {
        transform.Rotate(axis * speed * Time.deltaTime);
    }
}
