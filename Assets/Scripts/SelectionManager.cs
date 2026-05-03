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
    public GameObject followLight; // Bizimle gezen o ışığı buraya sürükle

    [Header("UI Toolkit")]
    public UIDocument uiDoc;
    private Button backButton;
    private Label binaAdiLabel;
    private Label energyLabel;
    private Label karbonLabel;

    private bool isFocused = false;

    void OnEnable()
    {
        var root = uiDoc.rootVisualElement;
        
        // UI Elemanlarını Bağla
        backButton = root.Q<Button>("geri-buton"); 
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
        // Eğer odaklanmadıysak ve sol tık yapıldıysa
        if (Input.GetMouseButtonDown(0) && !isFocused)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, buildingLayer))
            {
                Building clickedBuilding = hit.collider.GetComponentInParent<Building>();
                if (clickedBuilding != null) FocusBuilding(clickedBuilding);
            }
        }
    }

    void FocusBuilding(Building building)
    {
        // 1. Kamera Geçişi
        focusCam.Follow = building.focusPoint;
        focusCam.LookAt = building.focusPoint;
        
        // Öncelikleri netleştirelim
        birdEyeCam.Priority = 10;
        focusCam.Priority = 20; // Focus kamerası üste çıkar

        movementScript.canMove = false;
        isFocused = true;

        // 2. Işığı Kapat (Bina ışıkları daha iyi görünsün diye istersen)
        if (followLight != null) followLight.SetActive(false);

        // 3. Verileri Yazdır
        if (building.data != null)
        {
            if (binaAdiLabel != null) binaAdiLabel.text = building.data.ad;
            if (energyLabel != null) energyLabel.text = building.data.tuketim.ToString("F2") + " kW";
            if (karbonLabel != null) karbonLabel.text = building.data.karbon.ToString("F2") + " kg";
        }

        if (backButton != null) backButton.style.display = DisplayStyle.Flex;
    }

    public void ReturnToBirdEye()
    {
        // 1. Kamera Önceliklerini Sıfırla
        focusCam.Priority = 5; 
        birdEyeCam.Priority = 30; // Kuşbakışı en üste çıksın

        movementScript.canMove = true;
        isFocused = false;

        // 2. Takip Işığını Geri Getir
        if (followLight != null) followLight.SetActive(true);

        // 3. UI Kapat
        if (backButton != null) backButton.style.display = DisplayStyle.None;
        
        // Metinleri temizle
        if (binaAdiLabel != null) binaAdiLabel.text = "Bina Seçilmedi";
    }
}