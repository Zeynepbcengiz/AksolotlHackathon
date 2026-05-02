using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UIElements; // Buton kontrolü için kalmalı

public class SelectionManager : MonoBehaviour
{
    [Header("Kameralar")]
    public CinemachineCamera birdEyeCam;
    public CinemachineCamera focusCam;

    [Header("Ayarlar")]
    public LayerMask buildingLayer;
    public CamMovement movementScript;

    [Header("UI Toolkit")]
    public UIDocument uiDoc;
    private Button backButton;

    private bool isFocused = false;

    void OnEnable()
    {
        // UI Toolkit butonunu bul ve bağla
        var root = uiDoc.rootVisualElement;
        backButton = root.Q<Button>("geri-buton"); 

        if (backButton != null)
        {
            backButton.clicked += ReturnToBirdEye;
            backButton.style.display = DisplayStyle.None; // Başlangıçta gizli
        }
    }

    void Update()
    {
        // Klasik Sol Tık (Eski Sistem)
        if (Input.GetMouseButtonDown(0) && !isFocused)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, buildingLayer))
            {
                Building clickedBuilding = hit.collider.GetComponent<Building>();
                if (clickedBuilding != null) FocusBuilding(clickedBuilding);
            }
        }
        
    }

    void FocusBuilding(Building building)
    {
        focusCam.Follow = building.focusPoint;
        focusCam.LookAt = building.focusPoint;
        focusCam.Priority = 20;

        movementScript.canMove = false;
        isFocused = true;

        if (backButton != null) backButton.style.display = DisplayStyle.Flex;
    }

    public void ReturnToBirdEye()
    {
        focusCam.Priority = 5;
        movementScript.canMove = true;
        isFocused = false;

        if (backButton != null) backButton.style.display = DisplayStyle.None;
    }
}