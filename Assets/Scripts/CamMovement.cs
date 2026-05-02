using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public float speed = 25f;
    public bool canMove = true;

    void Update()
    {
        // Eğer bir binaya odaklanmışsak hareket etme
        if (!canMove) return;

        // Klasik Input Manager (Eski Sistem)
        float h = Input.GetAxis("Horizontal"); // A-D veya Sol-Sağ Ok
        float v = Input.GetAxis("Vertical");   // W-S veya Yukarı-Aşağı Ok

        Vector3 moveDirection = new Vector3(h, 0, v);

        if (moveDirection.magnitude > 0.1f)
        {
            transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
        }
    }
}