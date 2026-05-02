using UnityEngine;

// Sadece BİR KERE yazdığından emin ol
[CreateAssetMenu(fileName = "NewBuildingData", menuName = "EnergyProject/BuildingData")]
public class BuildingData : ScriptableObject
{
    public string id;
    public string ad;
    public float tuketim;
    public float yenilenebilir;
    public double karbon;
    public bool isAnomaly;
}