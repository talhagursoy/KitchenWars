using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour {

    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private MeshRenderer bottomBodyMeshRenderer;


    private Material material;

    private void Awake() {
        material = new Material(bodyMeshRenderer.material);
        bodyMeshRenderer.material = material;
        bottomBodyMeshRenderer.material = material;
    }

    public void SetPlayerColor(Color color) {
        material.color = color;
    }
}