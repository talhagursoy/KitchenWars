using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReputationUI : MonoBehaviour{

    [SerializeField] private PlayerReputation playerReputation;

    [SerializeField] private Image reputationBarImage;

    [SerializeField] private Image playerImage;

    [SerializeField] private PlayerImageListSO playerImageListSO;

    private Color color;


    private void Start() {
        playerReputation.OnReputationValueChanged += PlayerReputation_OnReputationValueChanged;
        color = KitchenGameMultiplayer.Instance.GetPlayerColor(playerReputation.GetColorID());
        reputationBarImage.color = color;
        playerImage.sprite = playerImageListSO.playerImageList[playerReputation.GetColorID()];
    }

    private void PlayerReputation_OnReputationValueChanged(object sender, PlayerReputation.OnReputationValueChangedEventArgs e) {
        reputationBarImage.fillAmount += e.reputationValue;
    }

}
