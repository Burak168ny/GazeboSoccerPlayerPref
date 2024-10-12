using UnityEngine;
using Fusion;

public class Ball : NetworkBehaviour
{
    public static Ball Instance; // Singleton örneği
    public Rigidbody rb;

    private void Awake()
    {
        // Singleton Pattern ile Ball Instance'ı ayarla
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        rb = GetComponent<Rigidbody>();
    }

    // Topun pozisyonunu güncelleyen metod
    public void UpdateBallPosition(Vector3 newPosition)
    {
        if (Object.HasStateAuthority)
        {
            transform.position = newPosition;
        }
    }

    // Topu fırlatmak için kullanılan metod
    public void ThrowBall(Vector3 force)
    {
        if (Object.HasStateAuthority)
        {
            rb.isKinematic = false; // Fizik motorunu yeniden etkinleştir
            rb.AddForce(force, ForceMode.Impulse); // Topu kuvvetle fırlat
        }
    }
}
