using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour {
    public Transform target;
    public float speed = 6f;
    public PetType type;

    private float skillCD;

    void Update() {
        transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
        transform.forward = Vector3.Lerp(transform.forward, target.forward, speed * Time.deltaTime);

        if (type == PetType.BOT) {
            if (skillCD <= 0f) {
                PlayerController player = FindObjectOfType<PlayerController>();
                player.healHP += player.stats.maxhp * .15f; // heals 15% every 10s
                foreach (Enemy e in FindObjectsOfType<Enemy>()) {
                    if (Vector3.Distance(e.transform.position, transform.position) < 10f) {
                        e.paralyze = 5f; // paralyzes for 5s every 10s
                    }
                }
                skillCD = 10f;
            } else skillCD -= Time.deltaTime;
        } else if (type == PetType.TURTLE) {
            if (skillCD <= 0f) {
                foreach (Enemy e in FindObjectsOfType<Enemy>()) {
                    if (Vector3.Distance(e.transform.position, transform.position) < 10f) {
                        e.effSpeed = .5f; // slows down enemies to half speed every 10s
                        e.bleed = 10f; // bleeds every 10s
                    }
                }
                skillCD = 10f;
            } else skillCD -= Time.deltaTime;
        }
    }
}

public enum PetType {
    NONE, BOT, TURTLE
}