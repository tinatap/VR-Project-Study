using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;

public class TCPAnalyticsClient : MonoBehaviour
{
    // =====================================================
    // NETWORK
    // =====================================================

    [Header("TCP Connection")]

    [Tooltip("IP address of the laptop/PC running Python receiver")]
    public string pcIPAddress = "192.168.1.100";

    [Tooltip("TCP port")]
    public int port = 12345;

    [Tooltip("How often DATA is sent")]
    public float sendInterval = 0.05f;


    // =====================================================
    // REFERENCES
    // =====================================================

    [Header("Player References")]

    public Transform player;

    [Tooltip("Center Eye Anchor / Main Camera")]
    public Transform centerEyeAnchorTransform;


    [Header("Input Actions")]

    public InputActionReference rightThumbstickAction;

    public InputActionReference rightTriggerAction;

    public InputActionReference leftThumbstickAction;

    public InputActionReference leftTriggerAction;


    [Header("Game Manager")]

    public GameManager gameManager;


    // =====================================================
    // TCP
    // =====================================================

    private TcpClient client;

    private NetworkStream networkStream;

    private CancellationTokenSource cancellationTokenSource;

    private SemaphoreSlim sendLock =
        new SemaphoreSlim(1, 1);

    private bool connected = false;

    private bool shuttingDown = false;

    private float sendTimer = 0f;


    // =====================================================
    // START
    // =====================================================

    private async void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }

        EnableInputs();

        cancellationTokenSource =
            new CancellationTokenSource();

        await ConnectToPC();
    }


    // =====================================================
    // ENABLE INPUTS
    // =====================================================

    private void EnableInputs()
    {
        if (rightThumbstickAction != null)
            rightThumbstickAction.action.Enable();

        if (rightTriggerAction != null)
            rightTriggerAction.action.Enable();

        if (leftThumbstickAction != null)
            leftThumbstickAction.action.Enable();

        if (leftTriggerAction != null)
            leftTriggerAction.action.Enable();
    }


    // =====================================================
    // DISABLE INPUTS
    // =====================================================

    private void DisableInputs()
    {
        if (rightThumbstickAction != null)
            rightThumbstickAction.action.Disable();

        if (rightTriggerAction != null)
            rightTriggerAction.action.Disable();

        if (leftThumbstickAction != null)
            leftThumbstickAction.action.Disable();

        if (leftTriggerAction != null)
            leftTriggerAction.action.Disable();
    }


    // =====================================================
    // CONNECT TO PC
    // =====================================================

    private async Task ConnectToPC()
    {
        try
        {
            Debug.Log(
                "Connecting to PC: " +
                pcIPAddress +
                ":" +
                port
            );

            client = new TcpClient();

            await client.ConnectAsync(
                pcIPAddress,
                port
            );

            if (shuttingDown)
                return;

            networkStream =
                client.GetStream();

            connected = true;

            Debug.Log(
                "TCP Analytics connected to PC."
            );
        }
        catch (Exception ex)
        {
            connected = false;

            if (!shuttingDown)
            {
                Debug.LogError(
                    "TCP connection failed: " +
                    ex.Message
                );
            }
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (!connected)
            return;

        if (shuttingDown)
            return;

        if (networkStream == null)
            return;

        if (!networkStream.CanWrite)
            return;

        sendTimer += Time.deltaTime;

        if (sendTimer >= sendInterval)
        {
            sendTimer = 0f;

            SendCurrentData();
        }
    }


    // =====================================================
    // SEND CURRENT DATA
    // =====================================================

    private async void SendCurrentData()
    {
        if (shuttingDown)
            return;

        try
        {
            // =================================================
            // TIME
            // =================================================

            float timestamp =
                Time.time;


            // =================================================
            // HEAD POSITION
            // =================================================

            Vector3 headPosition =
                Vector3.zero;

            Vector3 headRotation =
                Vector3.zero;

            if (centerEyeAnchorTransform != null)
            {
                headPosition =
                    centerEyeAnchorTransform.position;

                headRotation =
                    centerEyeAnchorTransform.eulerAngles;
            }


            // =================================================
            // PLAYER POSITION
            // =================================================

            Vector3 playerPosition =
                Vector3.zero;

            Vector3 playerRotation =
                Vector3.zero;

            if (player != null)
            {
                playerPosition =
                    player.position;

                playerRotation =
                    player.eulerAngles;
            }


            // =================================================
            // RIGHT THUMBSTICK
            // =================================================

            Vector2 rightThumbstick =
                Vector2.zero;

            if (rightThumbstickAction != null)
            {
                rightThumbstick =
                    rightThumbstickAction
                    .action
                    .ReadValue<Vector2>();
            }


            // =================================================
            // RIGHT TRIGGER
            // =================================================

            float rightTrigger = 0f;

            if (rightTriggerAction != null)
            {
                rightTrigger =
                    rightTriggerAction
                    .action
                    .ReadValue<float>();
            }


            // =================================================
            // LEFT THUMBSTICK
            // =================================================

            Vector2 leftThumbstick =
                Vector2.zero;

            if (leftThumbstickAction != null)
            {
                leftThumbstick =
                    leftThumbstickAction
                    .action
                    .ReadValue<Vector2>();
            }


            // =================================================
            // LEFT TRIGGER
            // =================================================

            float leftTrigger = 0f;

            if (leftTriggerAction != null)
            {
                leftTrigger =
                    leftTriggerAction
                    .action
                    .ReadValue<float>();
            }


            // =================================================
            // GAME MANAGER DATA
            // =================================================

            int mazeNumber = 0;

            int attemptNumber = 0;

            int collectedCoins = 0;

            int totalCoins = 0;

            int score = 0;

            float mazeElapsedTime = 0f;

            float totalElapsedTime = 0f;


            if (gameManager != null)
            {
                mazeNumber =
                    gameManager.CurrentMaze;

                attemptNumber =
                    gameManager.CurrentAttempt;

                collectedCoins =
                    gameManager.CollectedCoins;

                totalCoins =
                    gameManager.TotalCoins;

                score =
                    gameManager.TotalScore;

                mazeElapsedTime =
                    gameManager.CurrentMazeElapsedTime;

                totalElapsedTime =
                    gameManager.TotalGameElapsedTime;
            }


            // =================================================
            // CREATE DATA
            // =================================================

            TCPMazeData data =
                new TCPMazeData();


            // =================================================
            // GENERAL
            // =================================================

            data.timestamp =
                timestamp;

            data.eventType =
                "DATA";

            data.mazeNumber =
                mazeNumber;

            data.attemptNumber =
                attemptNumber;

            data.collectedCoins =
                collectedCoins;

            data.totalCoins =
                totalCoins;

            data.totalScore =
                score;

            data.mazeElapsedTime =
                mazeElapsedTime;

            data.totalGameElapsedTime =
                totalElapsedTime;


            // =================================================
            // HEAD
            // =================================================

            data.headPositionX =
                headPosition.x;

            data.headPositionY =
                headPosition.y;

            data.headPositionZ =
                headPosition.z;

            data.headRotationX =
                ConvertRotation(
                    headRotation.x
                );

            data.headRotationY =
                ConvertRotation(
                    headRotation.y
                );

            data.headRotationZ =
                ConvertRotation(
                    headRotation.z
                );


            // =================================================
            // PLAYER
            // =================================================

            data.playerPositionX =
                playerPosition.x;

            data.playerPositionY =
                playerPosition.y;

            data.playerPositionZ =
                playerPosition.z;

            data.playerRotationY =
                ConvertRotation(
                    playerRotation.y
                );


            // =================================================
            // RIGHT CONTROLLER
            // =================================================

            data.rightThumbstickX =
                rightThumbstick.x;

            data.rightThumbstickY =
                rightThumbstick.y;

            data.rightTrigger =
                rightTrigger;


            // =================================================
            // LEFT CONTROLLER
            // =================================================

            data.leftThumbstickX =
                leftThumbstick.x;

            data.leftThumbstickY =
                leftThumbstick.y;

            data.leftTrigger =
                leftTrigger;


            // =================================================
            // SEND
            // =================================================

            await SendData(data);
        }
        catch (Exception ex)
        {
            if (!shuttingDown)
            {
                Debug.LogError(
                    "TCP SendCurrentData Error: " +
                    ex.Message
                );
            }
        }
    }


    // =====================================================
    // SEND EVENT
    // =====================================================

    public async void SendEvent(string eventType)
    {
        if (shuttingDown)
            return;

        if (string.IsNullOrEmpty(eventType))
            return;

        try
        {
            TCPMazeData data =
                new TCPMazeData();


            // =================================================
            // GENERAL
            // =================================================

            data.timestamp =
                Time.time;

            data.eventType =
                eventType;


            // =================================================
            // GAME MANAGER
            // =================================================

            if (gameManager != null)
            {
                data.mazeNumber =
                    gameManager.CurrentMaze;

                data.attemptNumber =
                    gameManager.CurrentAttempt;

                data.collectedCoins =
                    gameManager.CollectedCoins;

                data.totalCoins =
                    gameManager.TotalCoins;

                data.totalScore =
                    gameManager.TotalScore;

                data.mazeElapsedTime =
                    gameManager.CurrentMazeElapsedTime;

                data.totalGameElapsedTime =
                    gameManager.TotalGameElapsedTime;
            }
            else
            {
                data.mazeNumber = 0;

                data.attemptNumber = 0;

                data.collectedCoins = 0;

                data.totalCoins = 0;

                data.totalScore = 0;

                data.mazeElapsedTime = 0f;

                data.totalGameElapsedTime = 0f;
            }


            // =================================================
            // SEND
            // =================================================

            await SendData(data);
        }
        catch (Exception ex)
        {
            if (!shuttingDown)
            {
                Debug.LogError(
                    "TCP SendEvent Error: " +
                    ex.Message
                );
            }
        }
    }


    // =====================================================
    // COMMON SEND FUNCTION
    // =====================================================

    private async Task SendData(
        TCPMazeData data
    )
    {
        if (shuttingDown)
            return;

        if (!connected)
            return;

        if (networkStream == null)
            return;

        if (!networkStream.CanWrite)
            return;


        // =================================================
        // LOCK
        // =================================================

        bool lockTaken = false;

        try
        {
            await sendLock.WaitAsync();

            lockTaken = true;


            // =================================================
            // CHECK AGAIN AFTER WAITING
            // =================================================

            if (shuttingDown)
                return;

            if (!connected)
                return;

            if (networkStream == null)
                return;

            if (!networkStream.CanWrite)
                return;


            // =================================================
            // JSON
            // =================================================

            string json =
                JsonConvert.SerializeObject(
                    data
                );


            byte[] jsonBytes =
                Encoding.UTF8.GetBytes(
                    json
                );


            // =================================================
            // LENGTH PREFIX
            // =================================================

            byte[] lengthPrefix =
                BitConverter.GetBytes(
                    jsonBytes.Length
                );


            // Unity/C# معمولاً Little Endian است.
            // Python ما Big Endian می‌خواند.
            // بنابراین Reverse می‌کنیم.

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(
                    lengthPrefix
                );
            }


            // =================================================
            // SEND LENGTH
            // =================================================

            await networkStream.WriteAsync(
                lengthPrefix,
                0,
                lengthPrefix.Length
            );


            // =================================================
            // SEND JSON
            // =================================================

            await networkStream.WriteAsync(
                jsonBytes,
                0,
                jsonBytes.Length
            );


            // =================================================
            // FLUSH
            // =================================================

            await networkStream.FlushAsync();
        }
        catch (ObjectDisposedException)
        {
            if (!shuttingDown)
            {
                connected = false;

                Debug.LogWarning(
                    "TCP NetworkStream was disposed."
                );
            }
        }
        catch (Exception ex)
        {
            if (!shuttingDown)
            {
                connected = false;

                Debug.LogError(
                    "TCP SendData Error: " +
                    ex.Message
                );
            }
        }
        finally
        {
            if (lockTaken)
            {
                sendLock.Release();
            }
        }
    }


    // =====================================================
    // ROTATION
    // =====================================================

    private float ConvertRotation(
        float value
    )
    {
        return value > 180f
            ? value - 360f
            : value;
    }


    // =====================================================
    // CLEANUP
    // =====================================================

    private void OnDestroy()
    {
        shuttingDown = true;

        connected = false;

        DisableInputs();


        // =================================================
        // CANCEL
        // =================================================

        try
        {
            cancellationTokenSource?.Cancel();
        }
        catch
        {
        }


        // =================================================
        // CLOSE STREAM
        // =================================================

        try
        {
            if (networkStream != null)
            {
                networkStream.Close();
                networkStream = null;
            }
        }
        catch
        {
        }


        // =================================================
        // CLOSE CLIENT
        // =================================================

        try
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }
        catch
        {
        }


        // =================================================
        // DISPOSE
        // =================================================

        try
        {
            cancellationTokenSource?.Dispose();
        }
        catch
        {
        }
    }
}


// =========================================================
// TCP DATA CLASS
// =========================================================

[Serializable]
public class TCPMazeData
{
    // =====================================================
    // GENERAL
    // =====================================================

    public float timestamp;

    public string eventType;


    // =====================================================
    // GAME
    // =====================================================

    public int mazeNumber;

    public int attemptNumber;

    public int collectedCoins;

    public int totalCoins;

    public int totalScore;

    public float mazeElapsedTime;

    public float totalGameElapsedTime;


    // =====================================================
    // HEAD
    // =====================================================

    public float headPositionX;

    public float headPositionY;

    public float headPositionZ;

    public float headRotationX;

    public float headRotationY;

    public float headRotationZ;


    // =====================================================
    // PLAYER
    // =====================================================

    public float playerPositionX;

    public float playerPositionY;

    public float playerPositionZ;

    public float playerRotationY;


    // =====================================================
    // RIGHT CONTROLLER
    // =====================================================

    public float rightThumbstickX;

    public float rightThumbstickY;

    public float rightTrigger;


    // =====================================================
    // LEFT CONTROLLER
    // =====================================================

    public float leftThumbstickX;

    public float leftThumbstickY;

    public float leftTrigger;
}
