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

    void Update() {
        menu.SetActive(open);

        if (Input.GetKeyDown(KeyCode.Escape)) {
            open = true;
        } else if (Input.GetKeyDown(KeyCode.UpArrow) && open) {
            selectedIdx--;
            if (selectedIdx < 0) selectedIdx = 4 + selectedIdx;
        } else if (Input.GetKeyDown(KeyCode.DownArrow) && open) {
            selectedIdx++;
            selectedIdx %= 4;
        } else if (Input.GetKeyDown(KeyCode.Return) && open) {
            if (selectedIdx == 0) open = false;
            else if (selectedIdx == 1) open = false;
            else if (selectedIdx == 2) open = false;
            else if (selectedIdx == 3) {
                open = false;
                SceneManager.LoadSceneAsync(0);
            }
        }

        selected.anchoredPosition = Vector3.Lerp(selected.anchoredPosition, new Vector3(15f, 55f - selectedIdx * 40f, 0f), 15f * Time.deltaTime);
    }
}
