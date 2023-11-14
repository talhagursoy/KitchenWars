using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentRecipeTimer;
    private bool timerInitialized = false;
    private bool recipeTimeOutCalled = false;

    private RecipeSO localRecipeSO;

    private void Awake() {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(RecipeSO recipeSO) {
        recipeTimeOutCalled = false;
        localRecipeSO = recipeSO;
        if (!timerInitialized) {
            currentRecipeTimer = recipeSO.recipeTimer;
            timerInitialized = true;
        }
        recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer) {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }
 
        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList) {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
        UpdateTimer();
    }

    private void Update() {
        if (currentRecipeTimer != 0) {
            currentRecipeTimer -= Time.deltaTime;

            if (currentRecipeTimer <= 0 && localRecipeSO != null) {
                if (!recipeTimeOutCalled) {
                    DeliveryManager.Instance.RecipeTimeOut(localRecipeSO);// here if delivery happens at same time it might cause double call fix later with a check
                    recipeTimeOutCalled = true;
                }    
            }

            UpdateTimer();
        }
    }
    private void UpdateTimer() {
        if (currentRecipeTimer != 0) {
            int seconds = Mathf.CeilToInt(currentRecipeTimer);
            timerText.text = seconds.ToString();
        }
    }

}