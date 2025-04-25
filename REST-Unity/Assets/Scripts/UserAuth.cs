using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserAuth : MonoBehaviour
{
    public InputField emailInputField;
    public InputField passwordInputField;
    public Text responseText;
    public GameObject signupPanel;
    public GameObject adminPanel;
    public GameObject categoryPanel;

    private string apiUrlSignup = "http://localhost:3000/user/signup";
    private string apiUrlLogin = "http://localhost:3000/user/login";
    
    public static string currentRole;

    public void OnSignupButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        StartCoroutine(SignUp(email, password));
    }

    IEnumerator SignUp(string email, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("role", "admin"); 

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrlSignup, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                responseText.text = "Signup successful!";
                Debug.Log("Signup successful!");
            }
            else
            {
                responseText.text = "Signup failed: " + request.error;
                Debug.LogError("Signup failed: " + request.error);
            }
        }
    }

    public void OnLoginButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        StartCoroutine(Login(email, password));
    }

    IEnumerator Login(string email, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrlLogin, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;

                if (response.Contains("Auth successful"))
                {
                    string token = ExtractTokenFromResponse(response);
                    PlayerPrefs.SetString("AuthToken", token);
                    
                    currentRole = ExtractRoleFromResponse(response);

                    if (currentRole == "admin")
                    {
                        responseText.text = "Login successful!";
                        Debug.Log("Login successful!");

                        signupPanel.SetActive(false);
                        adminPanel.SetActive(true); 
                    }
                    else
                    {
                        signupPanel.SetActive(false);
                        //categoryPanel.SetActive(true); 
                    }
                }
                else
                {
                    responseText.text = "Login failed: " + response;
                    Debug.LogError("Login failed: " + response);
                }
            }
            else
            {
                responseText.text = "Login failed: " + request.error;
                Debug.LogError("Login failed: " + request.error);
            }
        }
    }

    private string ExtractTokenFromResponse(string response)
    {
        var tokenData = JsonUtility.FromJson<TokenResponse>(response);
        return tokenData.token;
    }

    private string ExtractRoleFromResponse(string response)
    {
        var roleData = JsonUtility.FromJson<RoleResponse>(response);
        return roleData.role;
    }

    [System.Serializable]
    public class TokenResponse
    {
        public string token;
    }

    [System.Serializable]
    public class RoleResponse
    {
        public string role;
    }
}


