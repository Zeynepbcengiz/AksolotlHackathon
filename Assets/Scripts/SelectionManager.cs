using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UIElements;
using TMPro;
using System.Collections; 
using Image = UnityEngine.UI.Image;


public class SelectionManager : MonoBehaviour
{

   // Image kullanacağımız için bu gerekli

[Header("Dinamik Barlar")]
public Image tuketimBarFill;
public Image karbonBarFill;



// 0'dan hedef sayıya hızla sayan fonksiyon
IEnumerator CountUpTo(TMP_Text textComponent, float targetValue, string suffix, string prefix)
{
    float duration = 1.0f; // 1 saniyede tamamlansın
    float currentValue = 0f;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        currentValue = Mathf.Lerp(0, targetValue, elapsedTime / duration);
        
        // Ekrana anlık yaz
        textComponent.text = prefix + currentValue.ToString("F1") + suffix;
        yield return null;
    }
    
    // Tam sayıya ulaştığında emin olmak için son kez yaz
    textComponent.text = prefix + targetValue.ToString("F1") + suffix;
}

// UpdateAnalysisUI fonksiyonu içine şunu ekle:

    [Header("Kameralar")]
    public CinemachineCamera birdEyeCam;
    public CinemachineCamera focusCam;

    [Header("Ayarlar")]
    public LayerMask buildingLayer;
    public CamMovement movementScript;
    public GameObject followLight;
    public float maxVeriDegeri = 200f; // Bar doluluğu için referans değer

    [Header("UI Toolkit")]
    public UIDocument uiDoc;
    private Button backButton;
    private Label binaAdiLabel;
    private Label energyLabel;
    private Label karbonLabel;
    private Label skorLabel;
    private VisualElement analizPaneli; // Sol panelin ana kapsayıcısı

    private bool isFocused = false;

   
public Canvas worldCanvas; // Sahnede oluşturduğun Canvas'ı buraya sürükle
public TMP_Text adText;     // Canvas içindeki metinleri sürükle
public TMP_Text enerjiText;
public TMP_Text karbonText;

void Start()
{
    // Oyun başladığında analiz panelini "Sessiz Mod"a al
    if (worldCanvas != null)
    {
        worldCanvas.gameObject.SetActive(false);
    }
}

    void OnEnable()
    {
        var root = uiDoc.rootVisualElement;
        
        // Elemanları Bağla
        backButton = root.Q<Button>("geri-buton"); 
        binaAdiLabel = root.Q<Label>("bina-adi");
        energyLabel = root.Q<Label>("tuketim-degeri");
        karbonLabel = root.Q<Label>("karbon-degeri");
        skorLabel = root.Q<Label>("sustainability-score"); // Yeni: Skor etiketi
        analizPaneli = root.Q<VisualElement>("LeftAnalysisPanel"); // Yeni: Panel referansı

        if (backButton != null)
        {
            backButton.clicked += ReturnToBirdEye;
            backButton.style.display = DisplayStyle.None;
        }
        
        if (analizPaneli != null) analizPaneli.style.display = DisplayStyle.None;
    }

    void Update()
    {
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
        focusCam.Follow = building.focusPoint;
        focusCam.LookAt = building.focusPoint;
        birdEyeCam.Priority = 10;
        focusCam.Priority = 20;

        movementScript.canMove = false;
        isFocused = true;

        if (followLight != null) followLight.SetActive(false);

        // UI Güncelleme ve Gösterme
        if (building.data != null)
        {
            UpdateAnalysisUI(building.data);
            if (analizPaneli != null) analizPaneli.style.display = DisplayStyle.Flex;
            if (backButton != null) backButton.style.display = DisplayStyle.Flex;
        }

        if (worldCanvas != null && building.data != null)
    {
        // 1. Paneli binanın önüne yerleştir
        worldCanvas.transform.position = building.transform.position + new Vector3(8f, 12f, -5f);
        worldCanvas.gameObject.SetActive(true);

        // 2. Verileri güncelle
        adText.text = building.data.ad;
        enerjiText.text = $"ENRJ: {building.data.tuketim:F1} kW";
        karbonText.text = $"CO2: {building.data.karbon:F1} kg";
    }
    // ... Kamera öncelik kodların ...
    focusCam.Follow = building.focusPoint;
    focusCam.LookAt = building.focusPoint;
    birdEyeCam.Priority = 10;
    focusCam.Priority = 20;

    movementScript.canMove = false;
    isFocused = true;

    if (worldCanvas != null && building.data != null)
    {
        // ARTIK IŞINLAMA YOK: Sadece aktif ediyoruz, zaten ekrana yapışık duruyor.
        worldCanvas.gameObject.SetActive(true);

        // Verileri güncelle (Senin yazdığın UpdateAnalysisUI fonksiyonunu çağırıyoruz)
        UpdateAnalysisUI(building.data);
    }
    }

    void UpdateAnalysisUI(BuildingData data)
{

    float dolulukOrani = data.tuketim / 200f;
    Debug.Log($"Bina: {data.ad}, Tüketim: {data.tuketim}, Oran: {dolulukOrani}");

    if (tuketimBarFill != null)
        tuketimBarFill.fillAmount = dolulukOrani;
    if (data == null) return;
     // ... Metin atamaların aynen kalıyor ...
    if (enerjiText != null) StartCoroutine(CountUpTo(enerjiText, data.tuketim, " kW", "ENRJ: "));

    // Değeri 0 ile 1 arasına sıkıştırıp barı dolduruyoruz (Örn: Max kapasite 200 olsun)
    if (tuketimBarFill != null)
        tuketimBarFill.fillAmount = Mathf.Clamp01(data.tuketim / 200f);

    if (karbonBarFill != null)
        karbonBarFill.fillAmount = Mathf.Clamp01((float)data.karbon / 200f);


    // 1. UI Toolkit (Sol Panel) Güncelleme - Sadece objeler varsa çalışır
    if (binaAdiLabel != null) binaAdiLabel.text = data.ad;
    if (energyLabel != null) energyLabel.text = data.tuketim.ToString("F1") + " kW";
    if (karbonLabel != null) karbonLabel.text = data.karbon.ToString("F1") + " kg";

    // 2. Bar Grafikleri Güncelleme
    UpdateBar("tuketim-bar", data.tuketim);
    UpdateBar("karbon-bar", (float)data.karbon);

    // 3. Skor Hesaplama
    float score = 100 - (data.tuketim / maxVeriDegeri * 50) + (data.yenilenebilir / maxVeriDegeri * 50);
    score = Mathf.Clamp(score, 0, 100);
    
    if (skorLabel != null) {
        skorLabel.text = "%" + score.ToString("F0");
        skorLabel.style.color = score > 70 ? Color.green : (score > 40 ? Color.yellow : Color.red);
    }

    // 4. Anomali İkonu
    var statusIcon = uiDoc.rootVisualElement.Q<VisualElement>("status-indicator");
    if (statusIcon != null)
        statusIcon.style.backgroundColor = data.isAnomaly ? Color.red : Color.green;

    // 5. Hologram (World Canvas) Güncelleme - Güvenlik kontrolü
    if (adText != null && enerjiText != null && karbonText != null)
    {
        adText.text = data.ad;
        enerjiText.text = $"ENRJ: {data.tuketim:F1} kW";
        karbonText.text = $"CO2: {data.karbon:F1} kg";
    }
    else
    {
        Debug.LogWarning("DİKKAT: Hologram metinleri (adText vb.) Inspector'dan sürüklenmemiş!");
    }
    
}
    

    void UpdateBar(string barName, float value)
    {
        var bar = uiDoc.rootVisualElement.Q<VisualElement>(barName);
        if (bar != null)
        {
            float ratio = Mathf.Clamp01(value / maxVeriDegeri);
            bar.style.width = Length.Percent(ratio * 100);
            
            // Değer çok yüksekse barı kırmızı yap
            bar.style.backgroundColor = ratio > 0.8f ? Color.red : new Color(0, 0.8f, 1f); 
        }
    }

    public void ReturnToBirdEye()
    {
        focusCam.Priority = 5; 
        birdEyeCam.Priority = 30;
        movementScript.canMove = true;
        isFocused = false;

        if (followLight != null) followLight.SetActive(true);
        if (backButton != null) backButton.style.display = DisplayStyle.None;
        if (analizPaneli != null) analizPaneli.style.display = DisplayStyle.None;
        focusCam.Priority = 5; 
    birdEyeCam.Priority = 30;
    movementScript.canMove = true;
    isFocused = false;

    // PANELİ KAPAT: Butona basıldığında ekranı kaplayan analiz gitsin
    if (worldCanvas != null)
    {
        worldCanvas.gameObject.SetActive(false);
    }
    }

}