using UnityEngine;
using UnityEngine.UIElements;

public class SelectionManager : MonoBehaviour
{
    // Public yaparak Inspector'dan sürüklememize olanak sağlıyoruz
    public UIDocument uiDoc; 
    
    private Label binaAdiLabel;
    private Label energyLabel;
    private Label karbonLabel;

    void OnEnable()
    {
        // Eğer Inspector'dan atanmadıysa bileşeni üzerinde ara
        if (uiDoc == null) uiDoc = GetComponent<UIDocument>();

        if (uiDoc != null)
        {
            var root = uiDoc.rootVisualElement;
            // UI Builder'daki "Name" alanına yazdığın isimlerle birebir aynı olmalı!
            binaAdiLabel = root.Q<Label>("bina-adi"); 
            energyLabel = root.Q<Label>("tuketim-degeri"); 
            karbonLabel = root.Q<Label>("karbon-degeri");
        }
        else
        {
            Debug.LogError("HATA: SelectionManager objesinde UIDocument bulunamadı!");
        }
    }

    public void FocusBuilding(Building building)
    {
        if (building != null && building.data != null)
        {
            if (binaAdiLabel != null) binaAdiLabel.text = building.data.ad;
            if (energyLabel != null) energyLabel.text = building.data.tuketim.ToString("F2") + " kW";
            if (karbonLabel != null) karbonLabel.text = building.data.karbon.ToString("F2") + " kg";
            
            Debug.Log($"UI Güncellendi: {building.data.id} - {building.data.tuketim} kW");
        }
    }
}