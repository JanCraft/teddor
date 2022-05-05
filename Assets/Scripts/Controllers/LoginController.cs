using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using jDevES.FirebaseIDToken;
using jDevES.Auth;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class LoginController : MonoBehaviour {
    private System.Threading.CancellationTokenSource cancel;
    public GameObject loginObject;
    public Text errorCode;
    public Button confirmBtn;
    public Button cancelBtn;

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
    }

    public async void Confirm() {
        confirmBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(true);

        cancel = new System.Threading.CancellationTokenSource();
        IDToken tok = await jDevAuth.OpenPopup(cancel.Token);
        if (tok.IsExpired) {
            errorCode.text = "ERROR: The token has expired";
            cancelBtn.gameObject.SetActive(false);
            confirmBtn.gameObject.SetActive(true);
        }
        if (await tok.ValidateIDToken()) {
            UnityWebRequest www = UnityWebRequest.Get("https://game.jdev.com.es/teddor/auth?authmode=1&jwt=" + tok.idToken);
            await www.SendWebRequest();

            if (www.responseCode == 400) {
                errorCode.text = "Account not found";
            } else if (www.responseCode == 401) {
                errorCode.text = "Invalid credentials";
            } else if (www.responseCode == 200) {
                PlayerPrefs.SetString("teddor.token", www.downloadHandler.text);
                PlayerPrefs.SetString("teddor.token.uid", tok.UserID);
                PlayerPrefs.SetString("teddor.token.email", tok.jwt.Payload["email"].ToString());
                PlayerPrefs.SetString("teddor.user", tok.jwt.Payload["name"].ToString().Trim());
                open = false;
            } else {
                errorCode.text = www.error ?? "";
            }

            confirmBtn.gameObject.SetActive(true);
            cancelBtn.gameObject.SetActive(false);
        } else {
            errorCode.text = "ERROR: The token is invalid";
            confirmBtn.gameObject.SetActive(true);
            cancelBtn.gameObject.SetActive(false);
        }
    }

    public void Cancel() {
        cancel.Cancel();
        confirmBtn.gameObject.SetActive(true);
        cancelBtn.gameObject.SetActive(false);
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

public static class UnityWebRequestExtension {
    public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp) {
        TaskCompletionSource<UnityWebRequest.Result> tsc = new TaskCompletionSource<UnityWebRequest.Result>();
        reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

        if (reqOp.isDone) tsc.TrySetResult(reqOp.webRequest.result);

        return tsc.Task.GetAwaiter();
    }
}