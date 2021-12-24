using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour {
    public int coins;
    public int bstars;
    public int bmatter;

    public Text resourceText;

    void Update() {
        resourceText.text = "      " + coins + "\n      " + bstars + "\n      " + bmatter;
    }

    public void GiveCoins(int count) {
        coins += count;
    }

    public void GiveMatter(int count) {
        bmatter += count;
    }

    public void GiveStars(int count) {
        bstars += count;
    }
}
