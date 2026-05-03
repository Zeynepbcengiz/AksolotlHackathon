using UnityEngine;

public class BuildingStatus : MonoBehaviour
{
    private Building _buildingScript;
    private Light _pointLight;

    [Header("Işık Ayarları")]
    public float baseIntensity = 30f; // Işık şiddetini artırdık
    public float blinkIntensity = 50f;

    private bool _isBlinking = false;
    private Color _currentTargetColor;

    void Awake()
    {
        _buildingScript = GetComponent<Building>();
        _pointLight = GetComponentInChildren<Light>();
    }

   public void UpdateAnalysis()
{
    if (_buildingScript == null || _buildingScript.data == null) return;

    float consumption = _buildingScript.data.tuketim;
    double carbon = _buildingScript.data.karbon;
    bool hasAnomaly = _buildingScript.data.isAnomaly;

    // Hangi değerlerin geldiğini Console'da görmek için bunu ekleyelim
    // Debug.Log($"Bina: {gameObject.name} | Tüketim: {consumption} | Karbon: {carbon}");

    _isBlinking = false;

    if (hasAnomaly)
    {
        _currentTargetColor = Color.red; 
        _isBlinking = true;
    }
    // SARI ŞARTINI ZORLAŞTIRDIK: Hem tüketim hem karbon çok yüksekse sarı yansın
    else if (consumption > 120f && carbon > 110.0) 
    {
        _currentTargetColor = Color.yellow;
    }
    // Geri kalan her durumda yeşil yanacaktır
    else
    {
        _currentTargetColor = Color.green;
    }

    if (_pointLight != null)
    {
        _pointLight.color = _currentTargetColor;
        _pointLight.intensity = baseIntensity;
        _pointLight.enabled = true;
    }
}

    void Update()
    {
        if (_pointLight == null) return;

        if (_isBlinking)
        {
            // Kırmızı alarm için hızlı yanıp sönme
            float blink = (Mathf.Sin(Time.time * 8f) + 1f) / 2f;
            _pointLight.intensity = blink * blinkIntensity;
        }
        else
        {
            // Sabit yanan ışıklar için hafif bir "nefes alma" efekti (Canlılık katar)
            float pulse = (Mathf.Sin(Time.time * 1.5f) + 1f) / 2f; 
            _pointLight.intensity = baseIntensity + (pulse * 5f);
        }
    }
}