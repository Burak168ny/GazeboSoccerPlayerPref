using UnityEngine;
using Fusion;

public class CubeControllerNet : NetworkBehaviour
{
    public Transform targetObject;
    public float scaleSpeed = 0.5f;
    public float maxVolume = 24f;
    public float minScale = 0.6f;
    public float maxScale = 30f;
    private Rigidbody _rb;

    float xyProduct;
    float yzProduct;
    public Vector3 minPosition = new Vector3(-10, 0, -10);
    public Vector3 maxPosition = new Vector3(10, 10, 10);

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer && GetInput(out NetworkInputData data))
        {
            HandleScale(data);
            // Sunucu scale işlemi yaptığında tüm istemcilere pozisyonu aktar
            RpcUpdateScale(targetObject.localScale);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcUpdateScale(Vector3 newScale)
    {
        targetObject.localScale = newScale;
    }

    private void HandleScale(NetworkInputData data)
    {
        Vector3 newScale = targetObject.localScale;

        // Yüksekliği değiştirme
        if (data.buttons.IsSet(NetworkInputData.SCALE_Y_UP))
        {
            newScale.y += scaleSpeed;
        }
        if (data.buttons.IsSet(NetworkInputData.SCALE_Y_DOWN))
        {
            newScale.y -= scaleSpeed;
        }

        // Genişliği değiştirme
        if (data.buttons.IsSet(NetworkInputData.SCALE_X_UP))
        {
            newScale.x += scaleSpeed;
        }
        if (data.buttons.IsSet(NetworkInputData.SCALE_X_DOWN))
        {
            newScale.x -= scaleSpeed;
        }

        // Derinliği değiştirme
        if (data.buttons.IsSet(NetworkInputData.SCALE_Z_UP))
        {
            newScale.z += scaleSpeed;
        }
        if (data.buttons.IsSet(NetworkInputData.SCALE_Z_DOWN))
        {
            newScale.z -= scaleSpeed;
        }

        // Boyutları sınırlamak için minimum ve maksimum değerleri belirle
        newScale.x = Mathf.Clamp(newScale.x, minScale, 8);
        newScale.y = Mathf.Clamp(newScale.y, minScale, 8);
        newScale.z = Mathf.Clamp(newScale.z, minScale, 4);

        // Hacmi kontrol et
        float currentVolume = newScale.x * newScale.y * newScale.z;
        if (currentVolume > maxVolume)
        {
            float scaleFactor = Mathf.Pow(maxVolume / currentVolume, 1f / 3f);
            newScale.x *= scaleFactor;
            newScale.y *= scaleFactor;
            newScale.z *= scaleFactor;
        }

        targetObject.localScale = newScale;
    }
}
