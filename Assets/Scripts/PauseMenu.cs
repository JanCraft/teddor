using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public static bool open = false;
    public GameObject menu;
    public RectTransform selected;
    private int selectedIdx = 0;
    private bool buffsOpen;
    private bool abilitiesOpen;
    private bool shardsOpen;

    public GameObject buffsMenu;
    public GameObject abilitiesMenu;
    public GameObject shardsMenu;

    void Update() {
        menu.SetActive(open);
        buffsMenu.SetActive(buffsOpen);
        abilitiesMenu.SetActive(abilitiesOpen);
        shardsMenu.SetActive(shardsOpen);

        if (!open && (buffsOpen || abilitiesOpen || shardsOpen)) {
            buffsOpen = false;
            abilitiesOpen = false;
            shardsOpen = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            open = !open;
            selectedIdx = 0;
        } else if (Input.GetKeyDown(KeyCode.UpArrow) && open && !buffsOpen && !abilitiesOpen && !shardsOpen) {
            selectedIdx--;
            if (selectedIdx < 0) selectedIdx = 5 + selectedIdx;
        } else if (Input.GetKeyDown(KeyCode.DownArrow) && open && !buffsOpen && !abilitiesOpen && !shardsOpen) {
            selectedIdx++;
            selectedIdx %= 5;
        } else if (Input.GetKeyDown(KeyCode.Return) && open && !buffsOpen && !abilitiesOpen && !shardsOpen) {
            if (selectedIdx == 0) open = false;
            else if (selectedIdx == 1) buffsOpen = true;
            else if (selectedIdx == 2) abilitiesOpen = true;
            else if (selectedIdx == 3) shardsOpen = true;
            else if (selectedIdx == 4) {
                open = false;
                SceneManager.LoadSceneAsync(0);
            }
        }

        selected.anchoredPosition = Vector3.Lerp(selected.anchoredPosition, new Vector3(15f, 55f - selectedIdx * 35f, 0f), 15f * Time.deltaTime);
    }
}
