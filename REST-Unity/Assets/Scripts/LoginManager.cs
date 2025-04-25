/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    private string loginUrl = "http://localhost:3000/user/login"; // Login URL

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Giriş işlemi
    public void Login(string email, string password, System.Action<string, bool> callback)
    {
        StartCoroutine(LoginCoroutine(email, password, callback));
    }

    private IEnumerator LoginCoroutine(string email, string password, System.Action<string, bool> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(loginUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;

                if (response.Contains("Auth successful"))
                {
                    // Token'ı API'den al
                    string token = ExtractTokenFromResponse(response);
                    PlayerPrefs.SetString("AuthToken", token); // Token'ı PlayerPrefs'e kaydet

                    callback("Giriş Başarılı!", true);
                }
                else
                {
                    callback("Email ya da şifre yanlış", false);
                }
            }
            else
            {
                callback("Sunucu Hatası", false);
            }
        }
    }

    private string ExtractTokenFromResponse(string response)
    {
        // API'den gelen cevaptan token'ı çıkartın
        var tokenData = JsonUtility.FromJson<TokenResponse>(response);
        return tokenData.token;
    }

    [System.Serializable]
    public class TokenResponse
    {
        public string token;
    }
}*/