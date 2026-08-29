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

    [Header("Analytics TCP Connection")]

    [Tooltip("IP address of the laptop/PC running the Python server.")]
    public string pcIPAddress = "192.168.1.100";

    [Tooltip("TCP port used by the Python analytics server.")]
    public int port = 5000;

    [Tooltip("Automatically connect when the game starts.")]
    public bool connectOnStart = true;

    [Tooltip("How often continuous VR data is sent.")]
    public float sendInterval = 0.05f;


    // =====================================================
    // PLAYER REFERENCES
    // =====================================================

    [Header("Player References")]

    public Transform player;

    public Transform centerEyeAnchorTransform;


    // =====================================================
    // INPUT ACTIONS
    // =====================================================

    [Header("Input Actions")]

    public InputActionReference rightThumbstickAction;

    public InputActionReference rightTriggerAction;

    public InputActionReference leftThumbstickAction;

    public InputActionReference leftTriggerAction;


    // =====================================================
    // GAME MANAGER
    // =====================================================

    [Header("Game Manager")]

    public GameManager gameManager;


    // =====================================================
    // TCP CONNECTION
    // =====================================================

    private TcpClient client;

    private NetworkStream stream;

    private bool isConnecting = false;

    private bool shuttingDown = false;


    // =====================================================
    // SEND CONTROL
    // =====================================================

    private SemaphoreSlim sendLock =
        new SemaphoreSlim(1, 1);

    private float sendTimer = 0f;


    // =====================================================
    // START
    // =====================================================

    private async void Start()
    {
        // -------------------------------------------------
        // Find GameManager automatically
        // -------------------------------------------------

        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }


        // -------------------------------------------------
        // Enable input actions
        // -------------------------------------------------

        EnableInputs();


        // -------------------------------------------------
        // Connect
        // -------------------------------------------------

        if (connectOnStart)
        {
            await ConnectToServer();
        }
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
    // CONNECT
    // =====================================================

    public async Task ConnectToServer()
    {
        if (isConnecting)
            return;

        if (IsConnected())
            return;

        if (shuttingDown)
            return;

        isConnecting = true;

        try
        {
            Debug.Log(
                "=================================================="
            );

            Debug.Log(
                "TCPAnalyticsClient: Connecting..."
            );

            Debug.Log(
                "IP: " + pcIPAddress
            );

            Debug.Log(
                "Port: " + port
            );


            client = new TcpClient();


            await client.ConnectAsync(
                pcIPAddress,
                port
            );


            if (shuttingDown)
                return;


            stream = client.GetStream();


            Debug.Log(
                "TCPAnalyticsClient: Connected successfully."
            );

            Debug.Log(
                "=================================================="
            );
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "TCPAnalyticsClient: Connection failed.\n" +
                ex.Message
            );

            CloseConnection();
        }
        finally
        {
            isConnecting = false;
        }
    }


    // =====================================================
    // CONNECTION STATUS
    // =====================================================

    public bool IsConnected()
    {
        return
            client != null &&
            client.Connected &&
            stream != null &&
            stream.CanWrite;
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (shuttingDown)
            return;

        if (!IsConnected())
            return;


        sendTimer += Time.deltaTime;


        if (sendTimer >= sendInterval)
        {
            sendTimer = 0f;

            SendCurrentData();
        }
    }


    // =====================================================
    // CONTINUOUS DATA
    // =====================================================

    private async void SendCurrentData()
    {
        if (shuttingDown)
            return;

        try
        {
            // =================================================
            // TIMESTAMP
            // =================================================

            string timestamp =
                DateTime.Now.ToString(
                    "yyyy-MM-dd HH:mm:ss.fff"
                );


            // =================================================
            // HEAD
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
            // PLAYER
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
            // GAME MANAGER
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
            // CREATE CONTINUOUS MESSAGE
            // =================================================

            ContinuousAnalyticsTCPMessage message =
                new ContinuousAnalyticsTCPMessage();


            message.messageType =
                "CONTINUOUS_DATA";

            message.timestamp =
                timestamp;


            // =================================================
            // GAME
            // =================================================

            message.mazeNumber =
                mazeNumber;

            message.attemptNumber =
                attemptNumber;

            message.collectedCoins =
                collectedCoins;

            message.totalCoins =
                totalCoins;

            message.totalScore =
                score;

            message.mazeElapsedTime =
                mazeElapsedTime;

            message.totalGameElapsedTime =
                totalElapsedTime;


            // =================================================
            // HEAD POSITION
            // =================================================

            message.headPositionX =
                headPosition.x;

            message.headPositionY =
                headPosition.y;

            message.headPositionZ =
                headPosition.z;


            // =================================================
            // HEAD ROTATION
            // =================================================

            message.headRotationX =
                ConvertRotation(
                    headRotation.x
                );

            message.headRotationY =
                ConvertRotation(
                    headRotation.y
                );

            message.headRotationZ =
                ConvertRotation(
                    headRotation.z
                );


            // =================================================
            // PLAYER POSITION
            // =================================================

            message.playerPositionX =
                playerPosition.x;

            message.playerPositionY =
                playerPosition.y;

            message.playerPositionZ =
                playerPosition.z;


            // =================================================
            // PLAYER ROTATION
            // =================================================

            message.playerRotationY =
                ConvertRotation(
                    playerRotation.y
                );


            // =================================================
            // RIGHT CONTROLLER
            // =================================================

            message.rightThumbstickX =
                rightThumbstick.x;

            message.rightThumbstickY =
                rightThumbstick.y;

            message.rightTrigger =
                rightTrigger;


            // =================================================
            // LEFT CONTROLLER
            // =================================================

            message.leftThumbstickX =
                leftThumbstick.x;

            message.leftThumbstickY =
                leftThumbstick.y;

            message.leftTrigger =
                leftTrigger;


            // =================================================
            // SEND
            // =================================================

            await SendMessage(message);
        }
        catch (Exception ex)
        {
            if (!shuttingDown)
            {
                Debug.LogError(
                    "TCPAnalyticsClient: " +
                    "Continuous data error.\n" +
                    ex.Message
                );
            }
        }
    }


    // =====================================================
    // EVENT
    // =====================================================

    public async void SendEvent(string eventType)
    {
        if (shuttingDown)
            return;

        if (string.IsNullOrEmpty(eventType))
            return;


        AnalyticsTCPMessage message =
            new AnalyticsTCPMessage();


        message.messageType =
            "EVENT";

        message.eventType =
            eventType;

        message.timestamp =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff"
            );


        await SendMessage(message);
    }


    // =====================================================
    // MAZE VISIT
    // =====================================================

    public async void SendMazeVisitSummary(
        MazeVisitRecord record
    )
    {
        if (record == null)
        {
            Debug.LogWarning(
                "TCPAnalyticsClient: " +
                "MazeVisitRecord is null."
            );

            return;
        }


        MazeVisitTCPMessage message =
            new MazeVisitTCPMessage();


        message.messageType =
            "MAZE_VISIT";


        message.visitNumber =
            record.visitNumber;

        message.mazeNumber =
            record.mazeNumber;

        message.attemptNumber =
            record.attemptNumber;


        message.durationSeconds =
            record.durationSeconds;

        message.totalGameElapsedTime =
            record.totalGameElapsedTime;


        message.collectedCoins =
            record.collectedCoins;

        message.totalCoins =
            record.totalCoins;


        message.result =
            record.result;


        message.startRoomDuration =
            record.startRoomDuration;

        message.startQuestionPanelDuration =
            record.startQuestionPanelDuration;


        message.timestamp =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff"
            );


        await SendMessage(message);
    }


    // =====================================================
    // EXIT CONFIRM
    // =====================================================

    public async void SendExitConfirmSummary(
        ExitConfirmRecord record
    )
    {
        if (record == null)
        {
            Debug.LogWarning(
                "TCPAnalyticsClient: " +
                "ExitConfirmRecord is null."
            );

            return;
        }


        ExitConfirmTCPMessage message =
            new ExitConfirmTCPMessage();


        message.messageType =
            "EXIT_CONFIRM";


        message.interactionNumber =
            record.interactionNumber;

        message.mazeNumber =
            record.mazeNumber;

        message.attemptNumber =
            record.attemptNumber;


        message.result =
            record.result;


        message.durationSeconds =
            record.durationSeconds;


        message.totalGameElapsedTime =
            record.totalGameElapsedTime;


        message.timestamp =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff"
            );


        await SendMessage(message);
    }


    // =====================================================
    // START ROOM
    // =====================================================

    public async void SendStartRoomSummary(
        float startRoomDuration,
        float startQuestionPanelDuration
    )
    {
        StartRoomTCPMessage message =
            new StartRoomTCPMessage();


        message.messageType =
            "START_ROOM";


        message.startRoomDuration =
            startRoomDuration;

        message.startQuestionPanelDuration =
            startQuestionPanelDuration;


        message.timestamp =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff"
            );


        await SendMessage(message);
    }


    // =====================================================
    // FINAL RESULT
    // =====================================================

    public async void SendFinalResult(
        string result,
        float totalGameTime,
        int finalScore
    )
    {
        FinalResultTCPMessage message =
            new FinalResultTCPMessage();


        message.messageType =
            "FINAL_RESULT";


        message.result =
            result;

        message.totalGameTime =
            totalGameTime;

        message.finalScore =
            finalScore;


        message.timestamp =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff"
            );


        await SendMessage(message);
    }


    // =====================================================
    // GENERIC SEND
    // =====================================================

    private async Task SendMessage(
        object messageObject
    )
    {
        if (shuttingDown)
            return;


        bool lockTaken = false;


        try
        {
            // -------------------------------------------------
            // Connection
            // -------------------------------------------------

            if (!IsConnected())
            {
                Debug.LogWarning(
                    "TCPAnalyticsClient: Not connected. " +
                    "Trying to reconnect..."
                );


                await ConnectToServer();


                if (!IsConnected())
                {
                    Debug.LogError(
                        "TCPAnalyticsClient: " +
                        "Unable to send message."
                    );

                    return;
                }
            }


            // -------------------------------------------------
            // Lock
            // -------------------------------------------------

            await sendLock.WaitAsync();

            lockTaken = true;


            if (shuttingDown)
                return;


            if (!IsConnected())
                return;


            // -------------------------------------------------
            // JSON
            // -------------------------------------------------

            string json =
                JsonConvert.SerializeObject(
                    messageObject
                );


            // -------------------------------------------------
            // Newline delimiter
            // -------------------------------------------------

            string data =
                json + "\n";


            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    data
                );


            // -------------------------------------------------
            // Send
            // -------------------------------------------------

            await stream.WriteAsync(
                bytes,
                0,
                bytes.Length
            );


            await stream.FlushAsync();


            Debug.Log(
                "TCP ANALYTICS SENT:\n" +
                json
            );
        }
        catch (ObjectDisposedException)
        {
            if (!shuttingDown)
            {
                Debug.LogWarning(
                    "TCPAnalyticsClient: " +
                    "Network stream disposed."
                );

                CloseConnection();
            }
        }
        catch (Exception ex)
        {
            if (!shuttingDown)
            {
                Debug.LogError(
                    "TCPAnalyticsClient: " +
                    "Send failed.\n" +
                    ex.Message
                );

                CloseConnection();
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
    // CLOSE
    // =====================================================

    public void CloseConnection()
    {
        try
        {
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }


            if (client != null)
            {
                client.Close();
                client = null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(
                "TCPAnalyticsClient: " +
                "Error closing connection.\n" +
                ex.Message
            );
        }
    }


    // =====================================================
    // APPLICATION QUIT
    // =====================================================

    private void OnApplicationQuit()
    {
        shuttingDown = true;

        DisableInputs();

        CloseConnection();


        try
        {
            sendLock?.Dispose();
        }
        catch
        {
        }
    }


    // =====================================================
    // DESTROY
    // =====================================================

    private void OnDestroy()
    {
        shuttingDown = true;

        DisableInputs();

        CloseConnection();
    }
}


// =========================================================
// CONTINUOUS ANALYTICS MESSAGE
// =========================================================

[Serializable]
public class ContinuousAnalyticsTCPMessage
{
    public string messageType;
    public string timestamp;

    // GAME

    public int mazeNumber;
    public int attemptNumber;

    public int collectedCoins;
    public int totalCoins;
    public int totalScore;

    public float mazeElapsedTime;
    public float totalGameElapsedTime;

    // HEAD POSITION

    public float headPositionX;
    public float headPositionY;
    public float headPositionZ;

    // HEAD ROTATION

    public float headRotationX;
    public float headRotationY;
    public float headRotationZ;

    // PLAYER POSITION

    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    // PLAYER ROTATION

    public float playerRotationY;

    // RIGHT CONTROLLER

    public float rightThumbstickX;
    public float rightThumbstickY;
    public float rightTrigger;

    // LEFT CONTROLLER

    public float leftThumbstickX;
    public float leftThumbstickY;
    public float leftTrigger;
}


// =========================================================
// EVENT MESSAGE
// =========================================================

[Serializable]
public class AnalyticsTCPMessage
{
    public string messageType;
    public string eventType;
    public string timestamp;
}


// =========================================================
// MAZE VISIT MESSAGE
// =========================================================

[Serializable]
public class MazeVisitTCPMessage
{
    public string messageType;

    public int visitNumber;

    public int mazeNumber;
    public int attemptNumber;

    public float durationSeconds;
    public float totalGameElapsedTime;

    public int collectedCoins;
    public int totalCoins;

    public string result;

    public float startRoomDuration;
    public float startQuestionPanelDuration;

    public string timestamp;
}


// =========================================================
// EXIT CONFIRM MESSAGE
// =========================================================

[Serializable]
public class ExitConfirmTCPMessage
{
    public string messageType;

    public int interactionNumber;

    public int mazeNumber;
    public int attemptNumber;

    public string result;

    public float durationSeconds;

    public float totalGameElapsedTime;

    public string timestamp;
}


// =========================================================
// START ROOM MESSAGE
// =========================================================

[Serializable]
public class StartRoomTCPMessage
{
    public string messageType;

    public float startRoomDuration;

    public float startQuestionPanelDuration;

    public string timestamp;
}


// =========================================================
// FINAL RESULT MESSAGE
// =========================================================

[Serializable]
public class FinalResultTCPMessage
{
    public string messageType;

    public string result;

    public float totalGameTime;

    public int finalScore;

    public string timestamp;
}