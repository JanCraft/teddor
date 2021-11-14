using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorylineController : MonoBehaviour {
    public static StorylineController instance;
    public Text questTextInfo;
    public string activeQuestId = "";

    void Start() {
        instance = this;
        TranslateKey.Init();
    }

    void Update() {
        if (activeQuestId == "") {
            questTextInfo.text = TranslateKey.Translate("ui.quest.none");
        } else {
            questTextInfo.text = GetQuestInfoFromID(activeQuestId);
        }
    }

    public static string GetQuestInfoFromID(string questID) {
        return TranslateKey.Translate("ui.quest." + questID);
    }
}
