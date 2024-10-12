using UnityEngine;
using Fusion;

public class BounceManagerNet : NetworkBehaviour
{
    public Transform targetObject;
    public PhysicMaterial targetMaterial;
    public float baseSpeed = 5f;
    public float moveLimit = 6f; // X ekseninde hareket sınırı
    private float moveSpeed;
    private Vector3 initialPosition;
    private float xyProduct;
    private float yzProduct;

    private void Start()
    {
        // Başlangıç konumunu sakla
        initialPosition = targetObject.position;
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer && GetInput(out NetworkInputData data))
        {
            HandleBounce(data);
            HandleMovement(data);
            RpcUpdatePosition(targetObject.position);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcUpdatePosition(Vector3 newPosition)
    {
        targetObject.position = newPosition;
    }

    private void HandleBounce(NetworkInputData data)
    {
        xyProduct = targetObject.localScale.x * targetObject.localScale.y;

        // XY çarpımına göre bounciness değeri
        if (xyProduct >= 20f && xyProduct <= 40f)
        {
            targetMaterial.bounciness = 0.2f;
        }
        else if (xyProduct >= 10f && xyProduct < 20f)
        {
            targetMaterial.bounciness = 0.5f;
        }
        else if (xyProduct >= 4f && xyProduct < 10f)
        {
            targetMaterial.bounciness = 1f;
        }
        else
        {
            targetMaterial.bounciness = 0f;
        }

        moveSpeed = baseSpeed * targetMaterial.bounciness;
    }

    private void HandleMovement(NetworkInputData data)
    {
        Vector3 moveDirection = targetObject.parent.right * data.direction.x;
        Vector3 newPosition = targetObject.position + moveDirection * moveSpeed * Runner.DeltaTime;

        xyProduct = targetObject.localScale.x * targetObject.localScale.y;
        yzProduct = targetObject.localScale.y * targetObject.localScale.z;

        Vector3 localPosition = targetObject.parent.InverseTransformPoint(newPosition);

        // X ekseninde yerel sınırları kontrol et
        if (xyProduct > 10 && yzProduct > 10)
        {
            localPosition.x = Mathf.Clamp(localPosition.x, -8.5f, 8.5f);
        }
        else if (xyProduct > 10 && yzProduct < 4)
        {
            localPosition.x = Mathf.Clamp(localPosition.x, -5.5f, 5.5f);
        }
        else
        {
            localPosition.x = Mathf.Clamp(localPosition.x, -5f, 5f);
        }

        targetObject.position = targetObject.parent.TransformPoint(localPosition);
    }
}
