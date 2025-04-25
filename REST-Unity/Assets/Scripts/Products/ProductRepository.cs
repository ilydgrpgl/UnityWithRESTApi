using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ProductRepository : MonoBehaviour
{
    public static ProductRepository Instance { get; private set; }
    private Dictionary<string, ProductData> products = new Dictionary<string, ProductData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetProducts(string json, string categoryId = null)
    {
        products.Clear();
        JObject response = JObject.Parse(json);
        JArray productArray = (JArray)response["products"];

        foreach (JObject product in productArray)
        {
            if (product["_id"] != null)
            {
                string id = product["_id"].ToString();
                string name = product["name"]?.ToString();
                float price = product["price"] != null ? float.Parse(product["price"].ToString()) : 0;
                int quantity = product["quantity"] != null ? int.Parse(product["quantity"].ToString()) : 0;
                string description = product["description"]?.ToString();
                string productImage = product["productImage"]?.ToString();
                string category = product["categoryId"]?.ToString();
                
                if (string.IsNullOrEmpty(categoryId) || category == categoryId)
                {
                    products[id] = new ProductData
                    {
                        Id = id,
                        Name = name,
                        Price = price,
                        Quantity = quantity,
                        Description = description,
                        ProductImage = productImage,
                        CategoryId = category
                    };
                }
            }
        }

        Debug.Log($"Toplam ürün sayısı: {products.Count}");
        foreach (var product in products.Values)
        {
            Debug.Log($"Ürün ID: {product.Id}, Adı: {product.Name}, Kategorisi: {product.CategoryId}");
        }
    }
    
    public List<ProductData> GetAllProducts()
    {
        return new List<ProductData>(products.Values);
    }
}


public class ProductData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public string ProductImage { get; set; }
    public string CategoryId { get; set; }
    
}
