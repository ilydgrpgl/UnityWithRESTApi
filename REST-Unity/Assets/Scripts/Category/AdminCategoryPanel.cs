using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

public class AdminCategoryPanel : MonoBehaviour
{
    public static AdminCategoryPanel Instance { get; private set; }

    public TMP_Dropdown categoryDropdown;
    public TMP_Dropdown secondCategoryDropdown;

    public Transform productsContentPanel;
    public GameObject productButtonPrefab;
    public Button exit;

    public TMP_InputField categoryInputField;
    public TMP_InputField categoryIdInputField;

    public Button addCategoryButton;
    public Button updateCategoryButton;
    public Button deleteCategoryButton;

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

    void Start()
    {
        RequestCategories(); 
        secondCategoryDropdown.onValueChanged.AddListener(OnCategoryChanged);
        addCategoryButton.onClick.AddListener(AddNewCategory);
        categoryIdInputField.interactable = false;
        deleteCategoryButton.onClick.AddListener(DeleteCategory);
        updateCategoryButton.onClick.AddListener(UpdateCategory);
    }

    private void RequestCategories()
    {
        StartCoroutine(CategoryRequestHelper.GetCategoriesRequest(OnCategoriesReceived, ErrorHandler));
    }

    private void OnCategoriesReceived(string json)
    {
        CategoryRepository.Instance.SetCategories(json);
        PopulateDropdown();
    }

    private void ErrorHandler(string error)
    {
        Debug.LogError($"Request failed: {error}"); 
    }
    private void UpdateCategory()
    {
        string categoryId = categoryIdInputField.text.Trim();
        string categoryName = categoryInputField.text.Trim();

        if (!string.IsNullOrEmpty(categoryId) && !string.IsNullOrEmpty(categoryName))
        {
            StartCoroutine(CategoryRequestHelper.PatchCategoryRequest(categoryId, categoryName, OnCategoryUpdated, ErrorHandler));
        }
        else
        {
            Debug.LogError("Kategori ID veya adı boş olamaz!");
        }
    }

    private void OnCategoryUpdated()
    {
        Debug.Log("Kategori başarıyla güncellendi!");
        RequestCategories(); 
    }

    private void DeleteCategory()
    {
        string categoryId = categoryIdInputField.text.Trim();
        if (!string.IsNullOrEmpty(categoryId))
        {
            StartCoroutine(CategoryRequestHelper.DeleteCategoryRequest(categoryId, OnCategoryDeleted, ErrorHandler));
        }
        else
        {
            Debug.LogError("Kategori ID boş olamaz!");
        }
    }

    private void OnCategoryDeleted()
    {
        Debug.Log("Kategori ve bağlı ürünler başarıyla silindi!");
        RequestCategories();
    }

    private void AddNewCategory()
    {
        string categoryName = categoryInputField.text.Trim();
        if (!string.IsNullOrEmpty(categoryName))
        {
            StartCoroutine(CategoryRequestHelper.PostCategoryRequest(categoryName, OnCategoryAdded, ErrorHandler));
        }
        else
        {
            Debug.LogError("Kategori adı boş olamaz!");
        }
    }

    private void OnCategoryAdded()
    {
        Debug.Log("Kategori başarıyla eklendi!");
        RequestCategories(); 
    }

    void PopulateDropdown()
    {
        List<string> categoryNames = CategoryRepository.Instance.GetCategoryNames();
        

        categoryDropdown.ClearOptions();
        categoryDropdown.AddOptions(categoryNames);

        secondCategoryDropdown.ClearOptions();
        secondCategoryDropdown.AddOptions(categoryNames);
    }
    

    private void OnCategoryChanged(int index)
    {
        string selectedCategoryId = CategoryRepository.Instance.GetCategoryId(index);
        string selectedCategoryName = CategoryRepository.Instance.GetCategoryNames()[index];

        categoryInputField.text = selectedCategoryName;
        categoryIdInputField.text = selectedCategoryId;

        if (!string.IsNullOrEmpty(selectedCategoryId))
        {
            StartCoroutine(CategoryRequestHelper.FetchProductsByCategoryRequest(selectedCategoryId, DisplayProducts, ErrorHandler));
        }
    }

    private List<GameObject> instantiatedButtons = new List<GameObject>();

    private void DisplayProducts(string json)
    {
        var products = JsonConvert.DeserializeObject<ProductList>(json);

        foreach (GameObject button in instantiatedButtons)
        {
            Destroy(button);
        }

        instantiatedButtons.Clear();

        foreach (var product in products.products)
        {
            GameObject button = Instantiate(productButtonPrefab, productsContentPanel);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = product.name;
            }

            instantiatedButtons.Add(button);
            button.SetActive(true);
        }
    }

    [System.Serializable]
    public class ProductList
    {
        public Product[] products;
    }

    [System.Serializable]
    public class Product
    {
        public string _id;
        public string name;
        public string category;
    }
}
