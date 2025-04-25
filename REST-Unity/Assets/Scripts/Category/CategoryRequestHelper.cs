using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;

public class CategoryRequestHelper : MonoBehaviour
{
    private static string categoryApiUrl = "http://localhost:3000/category";
    private static string productsApiUrl = "http://localhost:3000/products";

    public static IEnumerator GetCategoriesRequest(System.Action<string> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(categoryApiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    public static IEnumerator PatchCategoryRequest(string categoryId, string newCategoryName, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"http://localhost:3000/category/{categoryId}";

        var updateData = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "propName", "categoryName" }, { "value", newCategoryName } }
        };

        string json = JsonConvert.SerializeObject(updateData);
        byte[] jsonData = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            string authToken = PlayerPrefs.GetString("AuthToken");
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }


    public static IEnumerator PostCategoryRequest(string categoryName, System.Action onSuccess, System.Action<string> onError)
    {
        CategoryData newCategory = new CategoryData { Name = categoryName };
        string json = JsonConvert.SerializeObject(newCategory);
        byte[] jsonData = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(categoryApiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            string authToken = PlayerPrefs.GetString("AuthToken");
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }

    public static IEnumerator DeleteCategoryRequest(string categoryId, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"http://localhost:3000/category/{categoryId}";
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            string authToken = PlayerPrefs.GetString("AuthToken");
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }

    public static IEnumerator FetchProductsByCategoryRequest(string categoryId, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"{productsApiUrl}?categoryId={categoryId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}
