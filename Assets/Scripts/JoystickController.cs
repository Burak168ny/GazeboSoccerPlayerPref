using UnityEngine;

public class JoystickController : MonoBehaviour
{
    // Hareket için joystick referansı
    public Joystick movementJoystick;
    
    // Şut yönü için joystick referansı
    public Joystick shootJoystick;

    // Hareket joystick'inden gelen X ve Y ekseni girdilerini almak için
    public Vector3 GetMovementInput()
    {
        float horizontal = movementJoystick.Horizontal;
        float vertical = movementJoystick.Vertical;
        return new Vector3(horizontal, 0, vertical);
    }

    // Şut joystick'inden sadece yatay eksen girdisini almak için
    public float GetShootJoystickHorizontalInput()
    {
        return shootJoystick.Horizontal;  // Şut joystick'inin yatay ekseni (-1 ile 1 arasında)
    }

    // Şut joystick'inden hem X hem de Y ekseni girdilerini almak için (isterseniz)
    public Vector3 GetShootJoystickInput()
    {
        float horizontal = shootJoystick.Horizontal;
        float vertical = shootJoystick.Vertical;
        return new Vector3(horizontal, 0, vertical);
    }
}
