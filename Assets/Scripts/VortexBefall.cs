using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VortexBefall : MonoBehaviour {
    public int round = 0;

    public Transform current;
    public Transform next;
    public Transform start;
    public Transform water;

    private float waterheight = -1;

    void Start() {
        waterheight = current.position.y - .5f;
    }

    void Update() {
        Vector3 w = water.position;
        w.y = Mathf.Lerp(w.y, waterheight, 7.5f * Time.deltaTime);
        water.position = w;

        next.localScale = Vector3.Lerp(next.localScale, Vector3.one, 7.5f * Time.deltaTime);
    }

    public void SwapStart() {
        waterheight = current.position.y - .5f;
        current.position = next.position;
        round++;
        PrepareNext();
    }

    void PrepareNext() {
        const float radius = 3f;
        int code = round % 4;
        if (code == 0) next.position = start.position + Vector3.up * (1.5f * round) + Vector3.left * radius + Vector3.forward * radius;
        else if (code == 1) next.position = start.position + Vector3.up * (1.5f * round) + Vector3.right * radius + Vector3.forward * radius;
        else if (code == 2) next.position = start.position + Vector3.up * (1.5f * round) + Vector3.right * radius + Vector3.back * radius;
        else if (code == 3) next.position = start.position + Vector3.up * (1.5f * round) + Vector3.left * radius + Vector3.back * radius;
        next.localScale = Vector3.zero;
    }

    public void Init() {
        current.position = start.position;
        water.localPosition = Vector3.down * 95f;
        waterheight = water.position.y;
        round = 1;
        PrepareNext();
    }
}
