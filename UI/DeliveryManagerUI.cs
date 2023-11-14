using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour {


    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;


    public class RecipeUIPair {
        public RecipeSO Recipe { get; set; }
        public Transform UIElement { get; set; }
    }

    private List<RecipeUIPair> recipeUIList = new List<RecipeUIPair>();

    private void Awake() {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        List<RecipeSO> waitingRecipes = DeliveryManager.Instance.GetWaitingRecipeSOList();

        int i = 0;
        while (i < recipeUIList.Count) {
            if (i >= waitingRecipes.Count || recipeUIList[i].Recipe != waitingRecipes[i]) {
                int removeIndex = i;
                while (removeIndex > 0 && recipeUIList[removeIndex].Recipe == recipeUIList[removeIndex - 1].Recipe) {
                    removeIndex--;
                }
                Transform removedUI = recipeUIList[removeIndex].UIElement;
                recipeUIList.RemoveAt(removeIndex);
                Destroy(removedUI.gameObject);
                i = recipeUIList.Count;
            }
            else {
                i++;
            }
        }

        for (i = 0; i < waitingRecipes.Count; i++) {
            if (i < recipeUIList.Count) {
                recipeUIList[i].UIElement.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(waitingRecipes[i]);
            }
            else {
                Transform recipeTransform = Instantiate(recipeTemplate, container);
                recipeTransform.gameObject.SetActive(true);
                recipeUIList.Add(new RecipeUIPair { Recipe = waitingRecipes[i], UIElement = recipeTransform });
                recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(waitingRecipes[i]);
            }
        }
    }
}