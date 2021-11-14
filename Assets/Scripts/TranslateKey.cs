using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslateKey : MonoBehaviour {
    public static string lang = "en";
    public string key = "";
    private Text text;

    public static void Init() {
        lang = PlayerPrefs.GetString("teddor.lang", "en");
    }

    void Start() {
        Init();
        text = GetComponent<Text>();
    }

    void Update() {
        text.text = Translate(key);
    }

    public static string Translate(string key) {
        switch (key) {
            case "ui.wod.floor":
                if (lang == "en") return "Floor";
                if (lang == "es") return "Piso";
                break;

            case "ui.pause.items":
                if (lang == "en") return "Resume\nBuffs\nAbilities\nSouls\nQuit";
                if (lang == "es") return "Volver\nMejoras\nAbilidades\nAlmas\nSalir";
                break;
            case "ui.pause.noshards":
                if (lang == "en") return "no souls available";
                if (lang == "es") return "sin almas disponibles";
                break;
            case "ui.pause.nobuffs":
                if (lang == "en") return "no buffs available";
                if (lang == "es") return "sin mejoras dispoibles";
                break;

            case "ui.menu.death":
                if (lang == "en") return "You have fallen...\nbut somehow...\n\n<color=#f88>You wake up again...</color>";
                if (lang == "es") return "Has caído...\npero de alguna forma...\n\n<color=#f88>Te despiertas de nuevo...</color>";
                break;
            case "ui.menu.spc_play":
                if (lang == "en") return "'Spacebar' to play";
                if (lang == "es") return "'Espacio' para jugar";
                break;
            case "ui.menu.clt_update":
                if (lang == "en") return "Client not up to date";
                if (lang == "es") return "Cliente no actualizado";
                break;
            case "ui.menu.chn_mismatch":
                if (lang == "en") return "Channel mismatch";
                if (lang == "es") return "Canal incompatible";
                break;
            case "ui.menu.req_error":
                if (lang == "en") return "Server request failed";
                if (lang == "es") return "Petición al servidor fallida";
                break;
            case "ui.menu.settings":
                if (lang == "en") return "Settings";
                if (lang == "es") return "Ajustes";
                break;
            case "ui.menu.lang_active":
                if (lang == "en") return "English";
                if (lang == "es") return "Español";
                break;
            case "ui.menu.volume":
                if (lang == "en") return "Volume";
                if (lang == "es") return "Volumen";
                break;
            case "ui.menu.quality":
                if (lang == "en") return "Quality";
                if (lang == "es") return "Calidad";
                break;

            case "ui.quest.none":
                if (lang == "en") return "<b>Open World</b>\n no active quest";
                if (lang == "es") return "<b>Mundo Abierto</b>\n sin misiones";
                break;
        }
        return "<???>";
    }
}
