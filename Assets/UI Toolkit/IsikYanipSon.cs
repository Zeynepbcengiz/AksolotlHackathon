using UnityEngine;
using UnityEngine.UI;

public class IsikYanipSon : MonoBehaviour
{
    public float minAlpha = 0.3f;
    public float maxAlpha = 1.0f;
    public float speed = 1.5f;

    private Image image;
    private float offset;

    void Start()
    {
        image = GetComponent<Image>();
        offset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, 
            (Mathf.Sin(Time.time * speed + offset) + 1f) / 2f);
        
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}