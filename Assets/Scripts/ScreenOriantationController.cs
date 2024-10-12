using UnityEngine;

public class ScreenOrientationController : MonoBehaviour
{
    void Start()
    {
        // Başlangıçta ekran yönlendirmesini Landscape Left olarak ayarlıyoruz.
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // Sadece Landscape Left ve Landscape Right'a izin veriyoruz.
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }
}
