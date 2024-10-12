using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    public bool hasPossession = false; // Top kontrolü bayrağı

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            // Girdi bilgilerini al
            var input = GetInput(out NetworkInputData data);
            if (input)
            {
                // Yön bilgisi
                transform.position += data.direction * Runner.DeltaTime * 5;

                // Şut kontrolü
                if (data.buttons.IsSet(NetworkInputData.SHOOT) && hasPossession)
                {
                    Vector3 force = transform.forward * 50; // Topu ileri fırlat
                    GetComponent<Possession>().ThrowBall(force); // Topu fırlat
                }
            }
        }
    }
}
