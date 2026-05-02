using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "CityPulse/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingID; // API'de kullanılacak benzersiz anahtar
    public string buildingName;
    public float cameraDistance = 15f; // Binaya ne kadar yaklaşılacak?
    public float cameraHeight = 10f;   // Kameranın yüksekliği
}