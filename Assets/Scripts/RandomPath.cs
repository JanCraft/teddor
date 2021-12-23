using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPath : MonoBehaviour {
    public LayerMask mask;
    public float speed = 3.5f;

    private float delay = 1f;
    private bool walking;
    private Ray ray;
    private float length;
    private Vector3 startLoc;

    void Start() {
        startLoc = transform.position;
    }

    void Update() {
        if (DialogController.instance.isOpen) return;

        if (delay > 0f) {
            delay -= Time.deltaTime;
        } else if (!walking) {
            FindPath();
            walking = true;
        } else {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, ray.origin + ray.direction * length) < 1f) {
                walking = false;
                delay = Random.Range(1f, 2f);
            }
        }

        if (Vector3.Distance(transform.position, startLoc) > 15f) {
            transform.LookAt(startLoc);
            ray.origin = transform.position;
            ray.direction = transform.forward;
            length = Vector3.Distance(transform.position, startLoc);
        }
    }

    void FindPath() {
        float angle = Random.Range(0f, 360f);

        transform.localEulerAngles = Vector3.up * angle;
        ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f, mask)) {
            length = hit.distance;
        } else {
            length = 5f;
        }
    }
}
