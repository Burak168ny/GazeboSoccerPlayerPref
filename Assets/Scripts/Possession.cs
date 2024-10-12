using UnityEngine;
using Fusion;

public class Possession : NetworkBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    // Top kontrolünü kazanma metodu
    private void GainPossession()
    {
        if (Ball.Instance != null)
        {
            Ball.Instance.rb.isKinematic = true; // Fizik motorunu devre dışı bırak
            _player.hasPossession = true;

            // Topu karakterin tam önüne yerleştir
            UpdateBallPosition();
        }
    }

    // Topu karakterin önüne yerleştiren metod
    private void UpdateBallPosition()
    {
        // Karakterin forward yönü baz alındığında:
        Vector3 forwardOffset = _player.transform.forward * + 1.5f; // Z ekseninde ileri
        Vector3 rightOffset = _player.transform.right * -0.68f; // X ekseninde sola yarım birim
        Vector3 upOffset = _player.transform.up * -2f; // Y ekseninde bir birim yukarı

        // İstenen konumu hesapla
        Vector3 newPosition = _player.transform.position + forwardOffset + rightOffset + upOffset;

        // Topu yeni konuma ve karakterle aynı rotasyona hizala
        Ball.Instance.transform.position = newPosition;
        Ball.Instance.transform.rotation = _player.transform.rotation;
    }

    public override void FixedUpdateNetwork()
    {
        if (_player.hasPossession && Object.HasStateAuthority)
        {
            // Topu her karede karakterin önünde tut
            UpdateBallPosition();
        }

        var input = GetInput(out NetworkInputData data);
        if (input && data.buttons.IsSet(NetworkInputData.SHOOT) && _player.hasPossession)
        {
            // Joystick'ten alınan açıya göre kuvvet hesapla ve topu fırlat
            Vector3 force = CalculateShootForce(data.shootDirection);
            ThrowBall(force);
        }
    }

    // Topu fırlatma metodu
    public void ThrowBall(Vector3 force)
    {
        if (_player.hasPossession && Object.HasStateAuthority)
        {
            Ball.Instance.rb.isKinematic = false; // Fizik motorunu yeniden aktif et
            Ball.Instance.ThrowBall(force);
            _player.hasPossession = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.GetComponent<Ball>() && Object.HasStateAuthority)
        {
            GainPossession(); // Çarpışma olduğunda topu al
        }
    }

    // Joystick ile açıyı hesapla ve fırlatma kuvvetini belirle
    private Vector3 CalculateShootForce(float shootAngle)
    {
        // Açıya göre yön hesapla
        float radians = shootAngle * Mathf.Deg2Rad; // Açıyı radyana çevir
        Vector3 direction = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)); // Yatay düzlemdeki yön

        // Fırlatma kuvvetini belirle
        float forceMagnitude = 50f; // Sabit bir kuvvet değeri (isteğe göre değiştirilebilir)

        // Karakterin yönüne göre topu fırlatma kuvvetini hesapla
        Vector3 force = _player.transform.TransformDirection(direction) * forceMagnitude;

        return force;
    }
}
