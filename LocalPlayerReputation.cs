using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;


public class LocalPlayerReputation : MonoBehaviour{
    public static LocalPlayerReputation Instance { get; private set; }

    public event EventHandler<OnReputationValueChangedEventArgs> OnReputationValueChanged;

    public class OnReputationValueChangedEventArgs : EventArgs {
        public float reputationValue;
    }

    private float reputationValue;
    private float maxReputationValue = 1f;
    private bool hasKeyBeenPressed = false;

    private void Awake() {
        Instance = this;
        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
        KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId);
    }

    private void Start() {
        reputationValue = maxReputationValue;

        OnReputationValueChanged?.Invoke(this, new OnReputationValueChangedEventArgs {
            reputationValue = reputationValue
        });
    }

    public void ChangeReputationValue(float reputationValue) {
        this.reputationValue = reputationValue;
        OnReputationValueChanged?.Invoke(this, new OnReputationValueChangedEventArgs {
            reputationValue = reputationValue
        });
    }

    private void Update() {
        if (Input.GetKey(KeyCode.H) && (!hasKeyBeenPressed)) {
            hasKeyBeenPressed = true;
            ChangeReputationValue(0.5f);
        }
    }
}
