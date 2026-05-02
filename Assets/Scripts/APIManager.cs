using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class APIManager : MonoBehaviour
{
    private string url = "http://localhost:5218/api/binalar/enerji-cek";

    void Start() => InvokeRepeating(nameof(IstekAt), 0f, 5f);

    void IstekAt() => StartCoroutine(VeriCek());

    IEnumerator VeriCek() {
        var binalar = Object.FindObjectsByType<Building>(FindObjectsSortMode.None);
        // ScriptableObject (data) içindeki Id'leri (C-01, C-02 vb.) topluyoruz
        List<string> idListesi = binalar.Where(b => b.data != null).Select(b => b.data.id).ToList();

        if (idListesi.Count == 0) yield break;

        string json = "[\"" + string.Join("\",\"", idListesi) + "\"]";
        
        using (UnityWebRequest req = new UnityWebRequest(url, "POST")) {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success) {
                // Backend'den gelen listeyi çöz
                BuildingDataDTO[] gelenler = JsonHelper.FromJson<BuildingDataDTO>(req.downloadHandler.text);
                
                foreach (var v in gelenler) {
                    // Sahnede asset ID'si eşleşen binayı bul ve verisini yaz
                    var hedef = binalar.FirstOrDefault(b => b.data != null && b.data.id == v.id);
                    if (hedef != null) {
                        hedef.data.tuketim = v.tuketim;
                        hedef.data.karbon = (float)v.karbon;
                        Debug.Log($"<color=green>{v.id} Güncellendi: {v.tuketim}</color>");
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class BuildingDataDTO {
        public string id;      // Backend'dekiyle aynı isimde olmalı
        public float tuketim;
        public double karbon;
    }
}