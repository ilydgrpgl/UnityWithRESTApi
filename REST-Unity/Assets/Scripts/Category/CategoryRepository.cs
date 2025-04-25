using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CategoryRepository : MonoBehaviour
{
    public static CategoryRepository Instance { get; private set; }
    private Dictionary<string, CategoryData> categories = new Dictionary<string, CategoryData>();
    

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

    public void SetCategories(string json)
    {
        categories.Clear();
        JObject response = JObject.Parse(json);
        JArray categoryArray = (JArray)response["category"];

        foreach (JObject category in categoryArray)
        {
            if (category["categoryName"] != null && category["_id"] != null)
            {
                string id = category["_id"].ToString();
                string name = category["categoryName"].ToString();


                categories[id] = new CategoryData { Id = id, Name = name };
            }
        }

        Debug.Log($"Toplam kategori sayısı: {categories.Count}");
        foreach (var category in categories.Values)
        {
            Debug.Log($"Kategori ID: {category.Id}, Adı: {category.Name}");
        }
    }

    public string GetCategoryId(int index)
    {
        List<CategoryData> categoryList = new List<CategoryData>(categories.Values);

        if (index >= 0 && index < categoryList.Count)
        {
            Debug.Log("indexi: " + index);
            return categoryList[index].Id;
            
        }
        Debug.LogWarning("Geçersiz index: " + index);
        return null;
    }
    

    public string GetCategoryNameById(string categoryId)
    {
        if (categories.TryGetValue(categoryId, out var category))
        {  Debug.Log("Kategori adı: " + category.Name);
            return category.Name;
           
        }
        return null;
    }

     

    public List<string> GetCategoryNames()
    {
        List<string> names = new List<string>();
        foreach (var category in categories.Values)
        {
            names.Add(category.Name);
        }

        Debug.Log($"Kategori isimleri çekildi. Toplam kategori sayısı: {categories.Count}");
        return names;
    }
}

public class CategoryData
{
    public string Id { get; set; }
    [JsonProperty("categoryName")] public string Name { get; set; }
}