using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    public static bool open = false;
    private bool switching;
    public GameObject menu;
    public RectTransform selected;
    private int selectedIdx = 0;
    private bool buffsOpen;
    private bool abilitiesOpen;
    private bool shardsOpen;

    public GameObject buffsMenu;
    public GameObject abilitiesMenu;
    public GameObject shardsMenu;

    public AudioSource slideSfx;
    public AudioSource clickSfx;

    private void Start() {
        open = false;
        AudioListener.volume = PlayerPrefs.GetFloat("teddor.volume", 1f);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("teddor.quality", 5));
    }

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

        if (Input.GetKeyDown(KeyCode.Escape) && !Banner.playing) {
            open = !open;
            selectedIdx = 0;
            SaveController.instance.SaveAll();
        } else if (Input.GetKeyDown(KeyCode.UpArrow) && open && !buffsOpen && !abilitiesOpen && !shardsOpen) {
            selectedIdx--;
            slideSfx.Play();
            if (selectedIdx < 0) selectedIdx = 5 + selectedIdx;
        } else if (Input.GetKeyDown(KeyCode.DownArrow) && open && !buffsOpen && !abilitiesOpen && !shardsOpen) {
            selectedIdx++;
            slideSfx.Play();
            selectedIdx %= 5;
        } else if (Input.GetKeyDown(KeyCode.Return) && open && !buffsOpen && !abilitiesOpen && !shardsOpen) {
            clickSfx.Play();
            if (selectedIdx == 0) open = false;
            else if (selectedIdx == 1) buffsOpen = true;
            else if (selectedIdx == 2) abilitiesOpen = true;
            else if (selectedIdx == 3) shardsOpen = true;
            else if (selectedIdx == 4 && !switching) {
                switching = true;
                LoadingScreen.SwitchScene(0);
            }
        }

        selected.anchoredPosition = Vector3.Lerp(selected.anchoredPosition, new Vector3(15f, 55f - selectedIdx * 35f, 0f), 15f * Time.deltaTime);
    }
}
