using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBaseController : MonoBehaviour {
    public Pet pet;
    public bool active;
    public bool owned;
    public bool buying;

    public GameObject buyMenu;

    void Start() {
        owned = PlayerPrefs.GetInt("teddor.pets." + pet.ToString(), 0) == 1;
    }

    void Update() {
        pet.enabled = active;
        if (!active) {
            pet.transform.position = Vector3.Lerp(pet.transform.position, transform.position + Vector3.up * .75f, Time.deltaTime * 5f);
            pet.transform.Rotate(Vector3.up * 180f * Time.deltaTime);
        }

        if (buying && Input.GetKeyDown(KeyCode.Return)) {
            FindObjectOfType<ResourceController>().umatter -= 50;
            PlayerPrefs.SetInt("teddor.pets." + pet.ToString(), 1);
            owned = true;

            foreach (var x in FindObjectsOfType<PetBaseController>()) x.active = false;
            active = true;
            buying = false;
            buyMenu.SetActive(false);
        }
    }
    
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (owned) {
                bool status = active;
                if (!status) foreach (var x in FindObjectsOfType<PetBaseController>()) x.active = false;
                active = !status;
            }
            else {
                if (FindObjectOfType<ResourceController>().umatter >= 50) {
                    buying = true;
                    buyMenu.SetActive(true);
                }
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            buying = false;
            buyMenu.SetActive(false);
        }
    }
}
