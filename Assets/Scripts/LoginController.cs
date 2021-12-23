using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginController : MonoBehaviour {
    public GameObject loginObject;
    public InputField username;
    public InputField password;
    public InputField email;
    public Text errorCode;
    public Toggle toggleNew;
    public Button confirmBtn;

    public bool open = false;

    void Start() {
        open = !PlayerPrefs.HasKey("teddor.token");
        Debug.Log("Token: " + PlayerPrefs.GetString("teddor.token"));
        if (!open)
            StartCoroutine(CheckToken());

        if (PlayerPrefs.HasKey("teddor.weekly")) {
            long week = long.Parse(PlayerPrefs.GetString("teddor.weekly"), System.Globalization.CultureInfo.InvariantCulture);
            long weekn = new System.DateTimeOffset(StartOfWeek(System.DateTime.Now, System.DayOfWeek.Monday)).ToUnixTimeMilliseconds();

            if (weekn > week) {
                Debug.Log("=== WEEKLY RESET ===");
                PlayerPrefs.SetInt("teddor.wod.score", 0);
                PlayerPrefs.SetString("teddor.weekly", new System.DateTimeOffset(StartOfWeek(System.DateTime.Now, System.DayOfWeek.Monday)).ToUnixTimeMilliseconds().ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
        } else {
            PlayerPrefs.SetString("teddor.weekly", new System.DateTimeOffset(StartOfWeek(System.DateTime.Now, System.DayOfWeek.Monday)).ToUnixTimeMilliseconds().ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }

    void Update() {
        loginObject.SetActive(open);
        email.gameObject.SetActive(toggleNew.isOn);
    }

    public void Confirm() {
        StartCoroutine(LoginProcess());
    }

    private IEnumerator LoginProcess() {
        errorCode.text = "Missing credentials";
        if (username.text.Trim() == "") yield break;
        if (password.text.Trim() == "") yield break;
        if (toggleNew.isOn && email.text.Trim() == "") yield break;
        errorCode.text = "";

        string sUser = System.Uri.EscapeDataString(username.text);
        string sPasswd = System.Uri.EscapeDataString(password.text);
        string sEmail = System.Uri.EscapeDataString(email.text);
        username.interactable = false;
        password.interactable = false;
        email.interactable = false;
        toggleNew.interactable = false;
        confirmBtn.interactable = false;

        UnityWebRequest www = UnityWebRequest.Get("https://game.jdev.com.es/teddor/auth?user=" + sUser + "&passwd=" + sPasswd + "&email=" + sEmail);
        yield return www.SendWebRequest();

        if (www.responseCode == 400) {
            errorCode.text = "Account not found";
        } else if (www.responseCode == 401) {
            errorCode.text = "Invalid credentials";
        } else if (www.responseCode == 200) {
            PlayerPrefs.SetString("teddor.token", www.downloadHandler.text);
            PlayerPrefs.SetString("teddor.user", username.text.Trim());
            open = false;
        } else {
            errorCode.text = www.error ?? "";
        }

        username.interactable = true;
        password.interactable = true;
        email.interactable = true;
        toggleNew.interactable = true;
        confirmBtn.interactable = true;
    }

    private IEnumerator CheckToken() {
        UnityWebRequest www = UnityWebRequest.Get("https://game.jdev.com.es/teddor/topscores?token=" + PlayerPrefs.GetString("teddor.token"));
        yield return www.SendWebRequest();

        if (www.responseCode != 200) {
            PlayerPrefs.DeleteKey("teddor.token");
            PlayerPrefs.DeleteKey("teddor.user");
            open = true;
        }
    }

    public static System.DateTime StartOfWeek(System.DateTime dt, System.DayOfWeek startOfWeek) {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}
