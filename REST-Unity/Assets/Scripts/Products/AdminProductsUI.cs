using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class AdminProductsUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform contentPanel;
    public TMP_InputField nameInputField;
    public TMP_InputField priceInputField;
    public TMP_InputField descriptionInputField;
    public TMP_InputField quantityInputField;
    public TMP_InputField productIdInputField;
    public TMP_Dropdown categoryDropdown;
    public GameObject loadingSpinnerPrefab; 


    public Button createButton, clearButton, updateButton, deleteButton, categoryButton, uploadImageButton;
    private int currentPage = 0;
    private int itemsPerPage = 6;
    public Transform paginationPanel;
    public GameObject pageButtonPrefab;


    public GameObject categoryPanel, productPanel;
    private string selectedImagePath;
    private List<ProductData> currentProducts = new List<ProductData>();

    private void Start()
    {
        uploadImageButton.onClick.AddListener(OpenFilePicker);
        createButton.onClick.AddListener(CreateProduct);
        clearButton.onClick.AddListener(ClearFields);
        updateButton.onClick.AddListener(UpdateProduct);
        deleteButton.onClick.AddListener(DeleteProduct);
        categoryButton.onClick.AddListener(ToggleCategoryPanel);

        productIdInputField.interactable = false;
        PopulateCategoryDropdown();
        RequestProducts();
    }

    private void PopulateCategoryDropdown()
    {
        if (CategoryRepository.Instance == null) return;
        categoryDropdown.ClearOptions();
        categoryDropdown.AddOptions(CategoryRepository.Instance.GetCategoryNames());
        RequestProducts();
    }

    private void OpenFilePicker()
    {
        selectedImagePath = UnityEditor.EditorUtility.OpenFilePanel("Resim Seç", "", "jpg,png");
        Debug.Log(string.IsNullOrEmpty(selectedImagePath) ? "Resim seçilmedi." : "Seçilen resim yolu: " + selectedImagePath);
    }

    private void ToggleCategoryPanel()
    {
        productPanel.SetActive(!productPanel.activeSelf);
        categoryPanel.SetActive(!categoryPanel.activeSelf);
    }

    private void RequestProducts()
    {
        StartCoroutine(ProductRequestHelper.FetchProductsRequest(null, OnProductsReceived));
    }

    private void OnProductsReceived(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        ProductRepository.Instance.SetProducts(json);
        currentProducts = ProductRepository.Instance.GetAllProducts();
        PopulateProductButtons();
    }

    private void PopulateProductButtons()
    {
        ClearProductButtons();
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, currentProducts.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            var product = currentProducts[i];


            GameObject button = Instantiate(buttonPrefab, contentPanel);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            RawImage rawImage = button.GetComponentInChildren<RawImage>();

            if (buttonText != null)
                buttonText.text = product.Name;

            if (rawImage != null && !string.IsNullOrEmpty(product.ProductImage))
            {
                string fullImageUrl = ProductRequestHelper.baseImageUrl + product.ProductImage.Replace("\\", "/");
                Transform spinnerTransform = rawImage.transform.parent.Find("LoadingSpinner");
                if (spinnerTransform != null) spinnerTransform.gameObject.SetActive(true);

                GameObject spinner = button.transform.Find("LoadingSpinner")?.gameObject;
                if (spinner != null) spinner.SetActive(true); 

                StartCoroutine(ProductRequestHelper.LoadProductImage(rawImage, fullImageUrl, spinner));
            }

            button.GetComponent<Button>().onClick.AddListener(() => OnProductClicked(product));
        }

        CreatePaginationButtons();
    }


    private void CreatePaginationButtons()
    {
        foreach (Transform child in paginationPanel)
        {
            Destroy(child.gameObject);
        }

        int totalPages = Mathf.CeilToInt((float)currentProducts.Count / itemsPerPage);

        for (int i = 0; i < totalPages; i++)
        {
            int pageIndex = i;
            GameObject pageButton = Instantiate(pageButtonPrefab, paginationPanel);
            TMP_Text buttonText = pageButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = (pageIndex + 1).ToString();
            }

            pageButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentPage = pageIndex;
                PopulateProductButtons();
            });
        }
    }


    private void ClearProductButtons()
    {
        foreach (Transform child in contentPanel)
        {
            child.gameObject.SetActive(false);
        }
    }


    private void OnProductClicked(ProductData product)
    {
        nameInputField.text = product.Name;
        priceInputField.text = product.Price.ToString("F2");
        descriptionInputField.text = product.Description;
        quantityInputField.text = product.Quantity.ToString();
        productIdInputField.text = product.Id;

        string categoryName = CategoryRepository.Instance.GetCategoryNameById(product.CategoryId);

        int dropdownIndex = categoryDropdown.options.FindIndex(option => option.text == categoryName);

        if (dropdownIndex >= 0)
        {
            categoryDropdown.value = dropdownIndex;
            categoryDropdown.RefreshShownValue();
        }
        else
        {
            Debug.LogWarning("Kategori adı dropdown'da bulunamadı: " + categoryName);
        }


        RequestProducts();
    }


    public void ClearFields()
    {
        nameInputField.text = "";
        priceInputField.text = "";
        descriptionInputField.text = "";
        quantityInputField.text = "";
        productIdInputField.text = "";
        categoryDropdown.value = 0;
    }


    public void UpdateProduct()
    {
        string selectedCategoryId;
        selectedCategoryId = CategoryRepository.Instance.GetCategoryId(categoryDropdown.value);
        ProductManager.Instance.UpdateProduct(
            productIdInputField.text,
            nameInputField.text,
            float.Parse(priceInputField.text),
            descriptionInputField.text,
            int.Parse(quantityInputField.text),
            selectedCategoryId,
            selectedImagePath
        );

        RequestProducts();
    }


    public void DeleteProduct()
    {
        ProductManager.Instance.DeleteProduct(productIdInputField.text);
        RequestProducts();
    }

    public void CreateProduct()
    {
        string selectedCategoryId = CategoryRepository.Instance.GetCategoryId(categoryDropdown.value);
        string imagePath = selectedImagePath;

        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogError("Resim seçilmedi!");
            return;
        }


        ProductManager.Instance.CreateProduct(
            nameInputField.text,
            float.Parse(priceInputField.text),
            descriptionInputField.text,
            int.Parse(quantityInputField.text),
            selectedCategoryId,
            imagePath
        );
        RequestProducts();
    }
}


[System.Serializable]
public class ProductResponse
{
    public int count;
    public List<Product> products;
}

[System.Serializable]
public class Product
{
    public string _id;
    public string name;
    public float price;
    public string description;
    public int quantity;
    public string productImage;
    public string categoryId;
}