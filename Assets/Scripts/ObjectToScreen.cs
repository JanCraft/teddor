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
    private Color origColor;

    private float pollTime = 0f;
    
    void Start() {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        cam = Camera.main;

        origColor = img.color;
    }

    void Update() {
        img.color = target != null ? origColor : Color.clear;
        if (target != null) PointToTarget();

        if (followEnemies) {
            if (pollTime <= 0f) {
                EnemySpawnpoint[] esp = FindObjectsOfType<EnemySpawnpoint>();

                if (esp == null || esp.Length == 0) target = null;
                else {
                    EnemySpawnpoint e = null;
                    float dst = 99999f;
                    foreach (EnemySpawnpoint x in esp) {
                        float d = Vector3.Distance(x.transform.position, cam.transform.position);
                        if (d < dst) {
                            e = x;
                            dst = d;
                        }
                    }
                    target = e.transform;
                }
                pollTime = 5f;
            } else {
                pollTime -= Time.deltaTime;
            }
        }
    }

    private void PointToTarget() {
        Vector3 toPos = target.position;
        Vector3 fromPos = cam.transform.position;
        Vector3 dir = (toPos - fromPos).normalized;

        rt.forward = dir;
        Vector3 v = rt.localEulerAngles;
        v.z -= v.y;
        v.x = v.y = 0f;
        rt.localEulerAngles = v;

        Vector3 targetPosScreenPt = cam.WorldToScreenPoint(toPos);
        bool isOnScreen = !(targetPosScreenPt.x <= 0f || targetPosScreenPt.x >= Screen.width || targetPosScreenPt.y <= 0f || targetPosScreenPt.y >= Screen.height) && targetPosScreenPt.z >= 0f;

        if (isOnScreen) {
            Vector3 translatedTargetPos = targetPosScreenPt;
            translatedTargetPos.x /= Screen.width;
            translatedTargetPos.x *= canvas.sizeDelta.x;
            translatedTargetPos.y /= Screen.height;
            translatedTargetPos.y *= canvas.sizeDelta.y;
            translatedTargetPos.z = 0f;

            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, translatedTargetPos, 15f * Time.deltaTime);
        } else {
            const float margin = 125f;
            Vector3 cappedTargetPos = targetPosScreenPt;
            cappedTargetPos.x = Mathf.Clamp(cappedTargetPos.x, margin, Screen.width - margin);
            cappedTargetPos.y = Mathf.Clamp(cappedTargetPos.y, margin, Screen.height - margin);

            if (targetPosScreenPt.z < 0f) cappedTargetPos.y = Screen.height - cappedTargetPos.y;

            Vector3 translatedTargetPos = cappedTargetPos;
            translatedTargetPos.x /= Screen.width;
            translatedTargetPos.x *= canvas.sizeDelta.x;
            translatedTargetPos.y /= Screen.height;
            translatedTargetPos.y *= canvas.sizeDelta.y;
            translatedTargetPos.z = 0f;

            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, translatedTargetPos, 15f * Time.deltaTime);
        }
    }
}