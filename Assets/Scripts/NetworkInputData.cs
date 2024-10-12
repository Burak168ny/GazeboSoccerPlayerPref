using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public const byte JUMP = 4;
    public const byte SCALE_X_UP = 5;
    public const byte SCALE_X_DOWN = 6;
    public const byte SCALE_Y_UP = 7;
    public const byte SCALE_Y_DOWN = 8;
    public const byte SCALE_Z_UP = 9;
    public const byte SCALE_Z_DOWN = 10;
    public const byte SHOOT = 11; // Yeni bayrak ekleyin

    public NetworkButtons buttons;

    
    public Vector3 direction;

    // Şut yönü için yeni bir alan ekleyin
    public float shootDirection;

    public void ReadInput(Vector3 joystickDirection)
    {
        // Joystick inputlarını direction ile ilişkilendir
        direction = joystickDirection;

        // // Zıplama kontrolü
        // if (Input.GetKey(KeyCode.Space))
        // {
        //     buttons.Set(JUMP, true);
        // }

        // Boyut değiştirme kontrolleri
        if (Input.GetKey(KeyCode.Keypad6)) buttons.Set(SCALE_X_UP, true);
        if (Input.GetKey(KeyCode.Keypad3)) buttons.Set(SCALE_X_DOWN, true);
        if (Input.GetKey(KeyCode.Keypad4)) buttons.Set(SCALE_Y_UP, true);
        if (Input.GetKey(KeyCode.Keypad1)) buttons.Set(SCALE_Y_DOWN, true);
        if (Input.GetKey(KeyCode.Keypad5)) buttons.Set(SCALE_Z_UP, true);
        if (Input.GetKey(KeyCode.Keypad2)) buttons.Set(SCALE_Z_DOWN, true);

    }
}
