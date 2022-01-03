using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour {
    public Transform target;
    public float speed = 6f;

    public PetType type;

    void Start() {
        
    }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
        transform.forward = Vector3.Lerp(transform.forward, target.forward, speed * Time.deltaTime);
    }
}

public enum PetType {
    NONE, BOT
}