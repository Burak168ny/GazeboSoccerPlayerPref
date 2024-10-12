using Fusion.Sockets;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    // UI buton referansı
    public Button jumpButton;
    private bool isJumping = false;

    private bool isShooting = false;
    private float shootAngle;

    // Joystick referansını alalım
    public JoystickController joystickController;

    // UI buton referansı (şut için)
    public Button shootButton;

    public int i = 0;
    private NetworkRunner networkRunner;

    // Prefab referanslarını saklamak için bir dizi
    [SerializeField] private NetworkPrefabRef[] networkPrefabRefs;

    private Dictionary<PlayerRef, NetworkObject> spawnCharacter = new Dictionary<PlayerRef, NetworkObject>();

    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(10, 5, -10),
        new Vector3(9.5f, 5, 30),
        new Vector3(27, 5, 20),
        new Vector3(27, 5, 0),
        new Vector3(-7, 5, 20),
        new Vector3(-7.5f, 5, -0.5f)
    };

    private Vector3[] spawnRotations = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, -120, 0),
        new Vector3(0, -60, 0),
        new Vector3(0, 120, 0),
        new Vector3(0, 60, 0)
    };

    // Yeni tanımladığımız zıplama butonu fonksiyonu
    public void OnJumpButtonPressed()
    {
        isJumping = true; // Zıplama işlemi tetikleniyor
    }

    private void Start()
    {
        // Jump butonu
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(OnJumpButtonPressed); // Hata bu satırdaydı, şimdi düzeldi
        }

        // Shoot butonu
        if (shootButton != null)
        {
            shootButton.onClick.AddListener(OnShootButtonPressed);
        }
    }

    // Butona basılınca çalışacak şut çekme fonksiyonu
    public void OnShootButtonPressed()
    {
        isShooting = true;
        CalculateShootDirection(); // Şut yönünü hesapla
    }

    private void CalculateShootDirection()
{
    // Joystick X ekseni değerini al (-1 ile 1 arasında bir değer döner)
    float joystickX = joystickController.GetShootJoystickHorizontalInput();

    // Joystick değerini 0 ile 180 derece arasında bir açıya çevir ve ortalama 90 dereceye göre ayarla
    shootAngle = Mathf.Lerp(190f, -10f, (joystickX + 1) / 2f);  // -1 -> 0 derece, +1 -> 180 derece, 0 -> 90 derece

    Debug.Log("Shoot angle: " + shootAngle);
}


    

    
    async void GameStart(GameMode mode)
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoomNestedMango",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnGUI()
    {
        if (networkRunner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                GameStart(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                GameStart(GameMode.Client);
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(runner.IsServer)
        {
            // PlayerPrefs'ten seçilen karakterin indeksini al
            int characterIndex = PlayerPrefs.GetInt("CharacterSelected", 0);

            if (characterIndex >= 0 && characterIndex < networkPrefabRefs.Length)
            {
                Vector3 spawnPosition = spawnPositions[i % spawnPositions.Length];
                Quaternion spawnRotation = Quaternion.Euler(spawnRotations[i % spawnRotations.Length]);

                // İlgili prefab'ı seç
                NetworkPrefabRef selectedPrefab = networkPrefabRefs[characterIndex];
                
                NetworkObject networkObject = runner.Spawn(selectedPrefab, spawnPosition, spawnRotation, player);
                spawnCharacter.Add(player, networkObject);
            }
            else
            {
                Debug.LogWarning("Invalid character index.");
            }
            i++;
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnCharacter.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnCharacter.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        // Joystick inputlarını al
        Vector3 joystickInput = joystickController.GetMovementInput();
        
        // Inputları doldur
        data.ReadInput(joystickInput);

        if (isJumping)
        {
            data.buttons.Set(NetworkInputData.JUMP, true);
            isJumping = false;  // Zıplama inputunu sıfırla
        }

        // Şut çekme kontrolü (butondan gelen)
        if (isShooting)
        {
            data.shootDirection = shootAngle; // Şut yönünü input'a ekle
            data.buttons.Set(NetworkInputData.SHOOT, true);
            isShooting = false; // Şut inputunu sıfırla
        }

        // Boyut değiştirme kontrolleri
        if (Input.GetKey(KeyCode.Keypad6)) data.buttons.Set(NetworkInputData.SCALE_X_UP, true);
        if (Input.GetKey(KeyCode.Keypad3)) data.buttons.Set(NetworkInputData.SCALE_X_DOWN, true);
        if (Input.GetKey(KeyCode.Keypad4)) data.buttons.Set(NetworkInputData.SCALE_Y_UP, true);
        if (Input.GetKey(KeyCode.Keypad1)) data.buttons.Set(NetworkInputData.SCALE_Y_DOWN, true);
        if (Input.GetKey(KeyCode.Keypad5)) data.buttons.Set(NetworkInputData.SCALE_Z_UP, true);
        if (Input.GetKey(KeyCode.Keypad2)) data.buttons.Set(NetworkInputData.SCALE_Z_DOWN, true);

        

        // Şut çekme kontrolü
        if (Input.GetKey(KeyCode.R))
        {
            data.buttons.Set(NetworkInputData.SHOOT, true);
        }

        input.Set(data);
    }



    // public void OnInput(NetworkRunner runner, NetworkInput input)
    // {
    //     var data = new NetworkInputData();

    //     // Yön girdilerini oku
    //     Vector3 direction = Vector3.zero;
    //     if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
    //     if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
    //     if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
    //     if (Input.GetKey(KeyCode.D)) direction += Vector3.right;

    //     data.direction = direction;

    //     // Zıplama isteğini kontrol et
    //     if (Input.GetKey(KeyCode.Space))
    //     {
    //         data.buttons.Set(NetworkInputData.JUMP, true);
    //     }

    //     // Sunucuya girdileri gönder
    //     input.Set(data);
    // }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // Girdi eksik olduğunda yapılacak işlemler
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        // Oyun kapatıldığında yapılacak işlemler
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        // Sunucuya bağlandığında yapılacak işlemler
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        // Sunucudan bağlantı kesildiğinde yapılacak işlemler
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // Bağlantı isteği alındığında yapılacak işlemler
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        // Bağlantı başarısız olduğunda yapılacak işlemler
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // Simülasyon mesajı alındığında yapılacak işlemler
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // Oturum listesi güncellendiğinde yapılacak işlemler
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // Özel kimlik doğrulama yanıtı alındığında yapılacak işlemler
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        // Sunucu göçü gerçekleştiğinde yapılacak işlemler
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        // Güvenilir veri alındığında yapılacak işlemler
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // Sahne yüklendiğinde yapılacak işlemler
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        // Sahne yüklenmeye başladığında yapılacak işlemler
    }

    // Eksik yöntemlerin eklenmesi
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player)
    {
        // Obje AOI'den çıktığında yapılacak işlemler
        // Bu yöntem `INetworkRunnerCallbacks` arayüzünün bir parçasıdır ve uygulanmalıdır. !
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player)
    {
        // Obje AOI'ye girdiğinde yapılacak işlemler
        // Bu yöntem `INetworkRunnerCallbacks` arayüzünün bir parçasıdır ve uygulanmalıdır. !
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        // Sunucudan bağlantı kesildiğinde yapılacak işlemler
        // Bu yöntem `INetworkRunnerCallbacks` arayüzünün bir parçasıdır ve uygulanmalıdır. !
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        // Güvenilir veri alındığında yapılacak işlemler
        // Bu yöntem `INetworkRunnerCallbacks` arayüzünün bir parçasıdır ve uygulanmalıdır. !
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        // Güvenilir veri ilerlemesi alındığında yapılacak işlemler
        // Bu yöntem `INetworkRunnerCallbacks` arayüzünün bir parçasıdır ve uygulanmalıdır. !
    }
}