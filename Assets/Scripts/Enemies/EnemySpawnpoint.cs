using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnpoint : MonoBehaviour {
    public string playerTag = "Player";
    public int areaDanger = 0;
    [Range(1, 5)]
    public int amount = 3;
    public GameObject[] enemies;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(playerTag)) {
            for (int i = 0; i < amount; i++) {
                int idx = Random.Range(0, enemies.Length);
                GameObject o = Instantiate(enemies[idx], transform.position + Random.insideUnitSphere, Quaternion.identity);

                o.GetComponent<Enemy>().areadanger = areaDanger;
            }

            Destroy(gameObject);
        }
    }
}
