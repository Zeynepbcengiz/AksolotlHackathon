using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityManager : MonoBehaviour
{
    public List<BuildingStatus> allBuildings = new List<BuildingStatus>();
    public float refreshRate = 2f; // Kaç saniyede bir kontrol edilsin?

    void Start()
    {
        allBuildings.AddRange(FindObjectsByType<BuildingStatus>(FindObjectsSortMode.None));
        
        // Sürekli kontrol döngüsünü başlat
        StartCoroutine(ContinuousAnalysisRoutine());
    }

    IEnumerator ContinuousAnalysisRoutine()
    {
        while (true) // Oyun açık olduğu sürece
        {
            UpdateAllCityVisuals();
            yield return new WaitForSeconds(refreshRate); // Belirlenen saniye kadar bekle
        }
    }

    public void UpdateAllCityVisuals()
    {
        foreach (var building in allBuildings)
        {
            if (building != null)
            {
                building.UpdateAnalysis();
            }
        }
    }
}