using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorylineController : MonoBehaviour {
    public static StorylineController instance;
    public Text questTextInfo;
    public ObjectToScreen ots;
    public string activeQuestId = "";
    public string storyQuestId = "";

    void Start() {
        instance = this;
        TranslateKey.Init();
    }

    void Update() {
        if (activeQuestId == "") {
            ots.target = null;
            questTextInfo.text = TranslateKey.Translate("ui.quest.none");
        } else {
            questTextInfo.text = GetQuestInfoFromID(activeQuestId);
        }
    }

    public static string GetQuestInfoFromID(string questID) {
        return TranslateKey.Translate("ui.quest." + questID);
    }

    public void SetGlobalQuest(string questId) {
        activeQuestId = questId;
        storyQuestId = questId;
    }

    public void SetStoryQuest(string questId) {
        storyQuestId = questId;
    }

    public void SetActiveQuest(string questId) {
        activeQuestId = questId;
    }
}
