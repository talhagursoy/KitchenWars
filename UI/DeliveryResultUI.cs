using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour {


    private const string POPUP = "Popup";
    private const string VIBRATE = "Vibrate";


    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image deliveryResultImage;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;
    [SerializeField] private DeliveryCounter deliveryCounter;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;

        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, DeliveryManager.OnrecipeFailedEventArgs e) {
        if (e.clientId == deliveryCounter.GetClientId()){
            deliveryResultImage.sprite = failedSprite;
            gameObject.SetActive(true);
            animator.SetTrigger(VIBRATE);
        }
        
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, DeliveryManager.OnrecipeSuccessEventArgs e) { 
        if(e.clientId == deliveryCounter.GetClientId()){
            deliveryResultImage.sprite = successSprite;
            gameObject.SetActive(true);
            animator.SetTrigger(POPUP);
        }
        
    }

}