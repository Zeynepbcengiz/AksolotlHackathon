using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject splashScreen;

    void Start()
    {
        splashScreen.SetActive(true);
    }
}