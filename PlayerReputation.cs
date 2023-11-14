using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerReputation : MonoBehaviour{

    public event EventHandler<OnReputationValueChangedEventArgs> OnReputationValueChanged;

    public class OnReputationValueChangedEventArgs : EventArgs {
        public float reputationValue;
    }

    private ulong clientID;
    private int colorID;
    private float reputationValue;
    private float maxReputationValue = 1f;

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

    public void SetColorID(int colorID){
        this.colorID = colorID;
    }

    public int GetColorID() {
        return this.colorID;
    }

    public void SetClientID(ulong clientID){
        this.clientID = clientID;
    }

    public ulong GetClientID() {
        return clientID;
    }

}
