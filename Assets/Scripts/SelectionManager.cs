using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UIElements;

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
    // Verileri yazacağımız etiketler:
    private Label binaAdiLabel;
    private Label energyLabel;
    private Label karbonLabel;

    private bool isFocused = false;

    void OnEnable()
    {
        var root = uiDoc.rootVisualElement;
        
        // Butonu bul ve bağla
        backButton = root.Q<Button>("geri-buton"); 
        
        // Veri etiketlerini bul (UI Builder'daki isimlerle aynı olmalı)
        binaAdiLabel = root.Q<Label>("bina-adi");
        energyLabel = root.Q<Label>("tuketim-degeri");
        karbonLabel = root.Q<Label>("karbon-degeri");

        if (backButton != null)
        {
            backButton.clicked += ReturnToBirdEye;
            backButton.style.display = DisplayStyle.None;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFocused)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, buildingLayer))
            {
                // Collider çocuk objede olabilir, o yüzden InParent kullanmak daha güvenlidir
                Building clickedBuilding = hit.collider.GetComponentInParent<Building>();
                if (clickedBuilding != null) FocusBuilding(clickedBuilding);
            }
        }
    }

    void FocusBuilding(Building building)
    {
        // 1. Kamera Kurgusu (Senin sistemin)
        focusCam.Follow = building.focusPoint;
        focusCam.LookAt = building.focusPoint;
        focusCam.Priority = 20;

        movementScript.canMove = false;
        isFocused = true;

        // 2. Veri Entegrasyonu (Backend'den gelenleri yazdırıyoruz)
        if (building.data != null)
        {
            if (binaAdiLabel != null) binaAdiLabel.text = building.data.ad;
            if (energyLabel != null) energyLabel.text = building.data.tuketim.ToString("F2") + " kW";
            if (karbonLabel != null) karbonLabel.text = building.data.karbon.ToString("F2") + " kg";
            
            Debug.Log($"<color=cyan>Odaklanıldı: {building.data.id} - Veri: {building.data.tuketim}</color>");
        }

        if (backButton != null) backButton.style.display = DisplayStyle.Flex;
    }

    public void ReturnToBirdEye()
    {
        focusCam.Priority = 5;
        movementScript.canMove = true;
        isFocused = false;

        if (backButton != null) backButton.style.display = DisplayStyle.None;
        
        // İstersen geri dönünce yazıları temizleyebilirsin:
        // if (binaAdiLabel != null) binaAdiLabel.text = "Bina Seçilmedi";
    }
}