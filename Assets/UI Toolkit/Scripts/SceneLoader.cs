using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için gerekli kütüphane

public class SceneLoader : MonoBehaviour
{
    public void StartDemoScene()
    {
        // "Demo" tırnak içindeki isim, sahne dosyanın ismiyle birebir aynı olmalı
        SceneManager.LoadSceneAsync("Demo");

    }
}