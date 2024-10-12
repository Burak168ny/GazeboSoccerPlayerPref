using UnityEngine;
using Fusion;

public class BallController : NetworkBehaviour
{
    private Rigidbody rb;
    private bool isControlled = false;
    private NetworkObject controllingPlayer;

    public float throwForce = 500f; // Fırlatma kuvveti

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (isControlled && controllingPlayer != null)
        {
            // Topu kontrol eden oyuncu varsa, top oyuncunun konumunu takip et
            transform.position = controllingPlayer.transform.position + Vector3.up; // Top oyuncunun üstünde olsun

            // Eğer 'R' tuşuna basıldıysa (topu fırlat)
            if (GetInput(out NetworkInputData data) && data.buttons.IsSet(NetworkInputData.SHOOT))
            {
                ThrowBall();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Eğer topa bir oyuncu çarptıysa kontrolü al
        if (!isControlled && other.CompareTag("Player") && other.TryGetComponent<NetworkObject>(out var player))
        {
            isControlled = true;
            controllingPlayer = player;
            rb.isKinematic = true; // Topu hareketsiz yap
            rb.velocity = Vector3.zero;
        }
    }

    private void ThrowBall()
    {
        // Topu fırlat
        isControlled = false;
        rb.isKinematic = false; // Topu tekrar fizik kurallarına dahil et

        // İleri doğru bir kuvvet uygula
        rb.AddForce(controllingPlayer.transform.forward * throwForce);

        // Kontrolü bırak
        controllingPlayer = null;
    }
}
