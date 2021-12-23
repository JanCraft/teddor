using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectToScreen : MonoBehaviour {
    public Transform target;
    public RectTransform canvas;
    public bool followEnemies;
    private RectTransform rt;
    private Image img;
    private Camera cam;

    private float pollTime = 0f;
    
    void Start() {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        cam = Camera.main;
    }

    void Update() {
        img.color = target != null ? Color.white : Color.clear;
        if (target != null)
            WorldToCanvasPoint(target.transform.position);

        if (followEnemies) {
            if (pollTime <= 0f) {
                EnemySpawnpoint esp = FindObjectOfType<EnemySpawnpoint>();

                if (esp == null) target = null;
                else target = esp.transform;
                pollTime = 5f;
            } else {
                pollTime -= Time.deltaTime;
            }
        }
    }

    private void WorldToCanvasPoint(Vector3 worldPoint) {
        Vector3 screenPos = cam.WorldToScreenPoint(worldPoint);
        const float borderSize = 100f;

        Vector3 cappedTargetPos = screenPos;
        cappedTargetPos.x = Mathf.Clamp(cappedTargetPos.x, borderSize, Screen.width - borderSize);
        cappedTargetPos.y = Mathf.Clamp(cappedTargetPos.y, borderSize, Screen.height - borderSize);

        if (screenPos.z < 0f) cappedTargetPos.y = Screen.height - cappedTargetPos.y;

        Vector3 translatedTargetPos = cappedTargetPos;
        translatedTargetPos.x /= Screen.width;
        translatedTargetPos.x *= 800f;
        translatedTargetPos.y /= Screen.height;
        translatedTargetPos.y *= 400f;
        translatedTargetPos.z = 0f;

        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, translatedTargetPos, 10f * Time.deltaTime);
    }
}