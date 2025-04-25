using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;

public static class ProductRequestHelper
{
    private static string productsApiUrl = "http://localhost:3000/products";
    public static string baseImageUrl = "http://localhost:3000/";


    public static IEnumerator LoadProductImage(RawImage imageComponent, string imageUrl, GameObject spinner)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            imageComponent.texture = texture;
        }
        else
        {
            Debug.LogError("Görsel yüklenemedi: " + request.error);
        }

        if (spinner != null)
            spinner.SetActive(false);
    }



    public static IEnumerator FetchProductsRequest(string categoryId, System.Action<string> onSuccess)
    {
        string url = $"{productsApiUrl}?categoryId={categoryId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            string authToken = PlayerPrefs.GetString("AuthToken");
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Ürünler alınırken hata: " + request.error);
            }
        }
    }

    public static IEnumerator CreateProductRequest(string name, float price, string description, int quantity, string categoryId, string imagePath)
    {
        string url = productsApiUrl;
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("price", price.ToString());
        form.AddField("description", description);
        form.AddField("quantity", quantity.ToString());
        form.AddField("categoryId", categoryId);

        byte[] imageBytes = File.ReadAllBytes(imagePath);
        form.AddBinaryData("productImage", imageBytes, Path.GetFileName(imagePath), "image/jpeg");

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        string authToken = PlayerPrefs.GetString("AuthToken");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Ürün başarıyla oluşturuldu.");
        }
        else
        {
            Debug.LogError("Ürün oluşturulurken hata oluştu: " + request.error);
        }
    }


    public static IEnumerator UpdateProductRequest(string productId, string name, float price, string description, int quantity, string categoryId, string imagePath)
    {
        string url = $"{productsApiUrl}/{productId}";
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("price", price.ToString());
        form.AddField("description", description);
        form.AddField("quantity", quantity.ToString());
        form.AddField("categoryId", categoryId);

        if (!string.IsNullOrEmpty(imagePath))
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            form.AddBinaryData("productImage", imageBytes, Path.GetFileName(imagePath), "image/jpeg");
        }

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        string authToken = PlayerPrefs.GetString("AuthToken");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        request.method = "PATCH";
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Ürün başarıyla güncellendi.");
        }
        else
        {
            Debug.LogError("Ürün güncellenirken hata oluştu: " + request.error);
        }
    }


    public static IEnumerator DeleteProductRequest(string productId)
    {
        string url = $"{productsApiUrl}/{productId}";
        UnityWebRequest request = new UnityWebRequest(url, "DELETE");
        string authToken = PlayerPrefs.GetString("AuthToken");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Ürün başarıyla silindi.");
        }
        else
        {
            Debug.LogError("Ürün silinirken hata oluştu: " + request.error);
        }
    }
}