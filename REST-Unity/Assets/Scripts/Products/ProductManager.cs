using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ProductManager : MonoBehaviour
{
    public static ProductManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public void CreateProduct(string name, float price, string description, int quantity, string categoryId, string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogError("Resim seçilmedi!");
            return;
        }

        StartCoroutine(ProductRequestHelper.CreateProductRequest(name, price, description, quantity, categoryId, imagePath));
    }

    public void UpdateProduct(string productId, string name, float price, string description, int quantity, string categoryId, string imagePath)
    {
        StartCoroutine(ProductRequestHelper.UpdateProductRequest(productId, name, price, description, quantity, categoryId, imagePath));
    }



    public void DeleteProduct(string productId)
    {
        StartCoroutine(ProductRequestHelper.DeleteProductRequest(productId));
    }
}