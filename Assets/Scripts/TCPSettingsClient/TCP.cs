using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.InputSystem;


public class TCP : MonoBehaviour
{
    // =====================================================
    // TCP CONNECTION
    // =====================================================

    [Header("TCP Connection")]

    [SerializeField]
    private string HOST = "127.0.0.1";

    [SerializeField]
    private int PORT = 12345;

    private TcpClient client;
    private NetworkStream netStream;

    private bool isRunning = true;
    private bool isConnected = false;


    // =====================================================
    // PLAYER
    // =====================================================

    [Header("Player")]

    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform centerEyeAnchorTransform;


    // =====================================================
    // CONTROLLERS
    // =====================================================

    [Header("Controller Positions")]

    [SerializeField]
    private Transform rightControllerTransform;

    [SerializeField]
    private Transform leftControllerTransform;


    // =====================================================
    // INPUT ACTIONS
    // =====================================================

    [Header("Input Actions")]

    [SerializeField]
    private InputActionReference rightThumbstickAction;

    [SerializeField]
    private InputActionReference rightTriggerAction;

    [SerializeField]
    private InputActionReference rightGrabAction;

    [SerializeField]
    private InputActionReference leftThumbstickAction;

    [SerializeField]
    private InputActionReference leftTriggerAction;


    // =====================================================
    // GAME MANAGER
    // =====================================================

    [Header("Game Manager")]

    [SerializeField]
    private GameManager gameManager;


    // =====================================================
    // SEND SETTINGS
    // =====================================================

    [Header("Send Settings")]

    [Tooltip("Continuous movement data is sent every X seconds.")]
    [SerializeField]
    private float sendInterval = 0.05f;

    private float sendTimer;

    private DateTime startTime;


    // =====================================================
    // SEND LOCK
    // =====================================================

    private readonly SemaphoreSlim sendLock =
        new SemaphoreSlim(1, 1);


    // =====================================================
    // START
    // =====================================================

    private async void Start()
    {
        startTime = DateTime.Now;

        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }

        EnableInputs();

        await ConnectToServer();
    }


    // =====================================================
    // ENABLE INPUTS
    // =====================================================

    private void EnableInputs()
    {
        EnableAction(rightThumbstickAction);
        EnableAction(rightTriggerAction);
        EnableAction(rightGrabAction);

        EnableAction(leftThumbstickAction);
        EnableAction(leftTriggerAction);
    }


    // =====================================================
    // DISABLE INPUTS
    // =====================================================

    private void DisableInputs()
    {
        DisableAction(rightThumbstickAction);
        DisableAction(rightTriggerAction);
        DisableAction(rightGrabAction);

        DisableAction(leftThumbstickAction);
        DisableAction(leftTriggerAction);
    }


    // =====================================================
    // INPUT HELPERS
    // =====================================================

    private void EnableAction(
        InputActionReference actionReference
    )
    {
        if (actionReference != null &&
            actionReference.action != null)
        {
            actionReference.action.Enable();
        }
    }


    private void DisableAction(
        InputActionReference actionReference
    )
    {
        if (actionReference != null &&
            actionReference.action != null)
        {
            actionReference.action.Disable();
        }
    }


    // =====================================================
    // CONNECT TO SERVER
    // =====================================================

    private async Task ConnectToServer()
    {
        try
        {
            Debug.Log("====================================");
            Debug.Log("Connecting to Python TCP Server...");
            Debug.Log("HOST: " + HOST);
            Debug.Log("PORT: " + PORT);

            client = new TcpClient();

            await client.ConnectAsync(
                HOST,
                PORT
            );

            if (!isRunning)
            {
                client.Close();
                return;
            }

            netStream =
                client.GetStream();

            isConnected = true;

            Debug.Log("====================================");
            Debug.Log("TCP CONNECTED SUCCESSFULLY");
            Debug.Log(
                "Python Server: " +
                HOST +
                ":" +
                PORT
            );
            Debug.Log("====================================");
        }
        catch (Exception ex)
        {
            isConnected = false;

            Debug.LogError(
                "TCP CONNECTION FAILED:\n" +
                ex.Message
            );
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (!isRunning ||
            !isConnected ||
            netStream == null ||
            !netStream.CanWrite)
        {
            return;
        }

        sendTimer += Time.deltaTime;

        if (sendTimer < sendInterval)
            return;

        sendTimer -= sendInterval;

        _ = SendCurrentData();
    }


    // =====================================================
    // TIMESTAMP
    // =====================================================

    private float GetTimestamp()
    {
        return (float)Math.Round(
            (DateTime.Now - startTime).TotalSeconds,
            3
        );
    }


    // =====================================================
    // SEND CONTINUOUS DATA
    // =====================================================

    private async Task SendCurrentData()
    {
        if (!CanSend())
            return;

        try
        {
            // =================================================
            // HEAD
            // =================================================

            Vector3 headPosition =
                GetPosition(centerEyeAnchorTransform);

            Vector3 headRotation =
                GetRotation(centerEyeAnchorTransform);


            // =================================================
            // PLAYER
            // =================================================

            Vector3 playerPosition =
                GetPosition(player);

            Vector3 playerRotation =
                GetRotation(player);


            // =================================================
            // RIGHT CONTROLLER
            // =================================================

            Vector3 rightControllerPosition =
                GetPosition(rightControllerTransform);

            Vector3 rightControllerRotation =
                GetRotation(rightControllerTransform);


            // =================================================
            // LEFT CONTROLLER
            // =================================================

            Vector3 leftControllerPosition =
                GetPosition(leftControllerTransform);

            Vector3 leftControllerRotation =
                GetRotation(leftControllerTransform);


            // =================================================
            // INPUT
            // =================================================

            Vector2 rightThumbstick =
                ReadVector2(rightThumbstickAction);

            float rightTrigger =
                ReadFloat(rightTriggerAction);

            float rightGrab =
                ReadFloat(rightGrabAction);

            Vector2 leftThumbstick =
                ReadVector2(leftThumbstickAction);

            float leftTrigger =
                ReadFloat(leftTriggerAction);


            // =================================================
            // GAME MANAGER DATA
            // =================================================

            int currentMaze = 0;
            int currentAttempt = 0;

            int collectedCoins = 0;
            int totalCoins = 0;

            int totalScore = 0;

            float mazeElapsedTime = 0f;
            float totalGameElapsedTime = 0f;


            if (gameManager != null)
            {
                currentMaze =
                    gameManager.CurrentMaze;

                currentAttempt =
                    gameManager.CurrentAttempt;

                collectedCoins =
                    gameManager.CollectedCoins;

                totalCoins =
                    gameManager.TotalCoins;

                totalScore =
                    gameManager.TotalScore;

                mazeElapsedTime =
                    gameManager.CurrentMazeElapsedTime;

                totalGameElapsedTime =
                    gameManager.TotalGameElapsedTime;
            }


            // =================================================
            // CREATE CONTINUOUS DATA
            // =================================================

            SentData dataToSend =
                new SentData
                {
                    recordType = "DATA",

                    timestamp =
                        GetTimestamp(),

                    // HEAD POSITION

                    headPositionX =
                        headPosition.x,

                    headPositionY =
                        headPosition.y,

                    headPositionZ =
                        headPosition.z,

                    // HEAD ROTATION

                    headRotationX =
                        ConvertRotation(headRotation.x),

                    headRotationY =
                        ConvertRotation(headRotation.y),

                    headRotationZ =
                        ConvertRotation(headRotation.z),

                    // PLAYER POSITION

                    playerPositionX =
                        playerPosition.x,

                    playerPositionY =
                        playerPosition.y,

                    playerPositionZ =
                        playerPosition.z,

                    // PLAYER ROTATION

                    playerRotationX =
                        ConvertRotation(playerRotation.x),

                    playerRotationY =
                        ConvertRotation(playerRotation.y),

                    playerRotationZ =
                        ConvertRotation(playerRotation.z),

                    // RIGHT CONTROLLER POSITION

                    rightControllerPositionX =
                        rightControllerPosition.x,

                    rightControllerPositionY =
                        rightControllerPosition.y,

                    rightControllerPositionZ =
                        rightControllerPosition.z,

                    // RIGHT CONTROLLER ROTATION

                    rightControllerRotationX =
                        ConvertRotation(
                            rightControllerRotation.x
                        ),

                    rightControllerRotationY =
                        ConvertRotation(
                            rightControllerRotation.y
                        ),

                    rightControllerRotationZ =
                        ConvertRotation(
                            rightControllerRotation.z
                        ),

                    // LEFT CONTROLLER POSITION

                    leftControllerPositionX =
                        leftControllerPosition.x,

                    leftControllerPositionY =
                        leftControllerPosition.y,

                    leftControllerPositionZ =
                        leftControllerPosition.z,

                    // LEFT CONTROLLER ROTATION

                    leftControllerRotationX =
                        ConvertRotation(
                            leftControllerRotation.x
                        ),

                    leftControllerRotationY =
                        ConvertRotation(
                            leftControllerRotation.y
                        ),

                    leftControllerRotationZ =
                        ConvertRotation(
                            leftControllerRotation.z
                        ),

                    // RIGHT INPUT

                    rightThumbstickX =
                        rightThumbstick.x,

                    rightThumbstickY =
                        rightThumbstick.y,

                    rightTrigger =
                        rightTrigger,

                    rightGrab =
                        rightGrab,

                    // LEFT INPUT

                    leftThumbstickX =
                        leftThumbstick.x,

                    leftThumbstickY =
                        leftThumbstick.y,

                    leftTrigger =
                        leftTrigger,

                    // GAME DATA

                    mazeNumber =
                        currentMaze,

                    attemptNumber =
                        currentAttempt,

                    collectedCoins =
                        collectedCoins,

                    totalCoins =
                        totalCoins,

                    totalScore =
                        totalScore,

                    mazeElapsedTime =
                        mazeElapsedTime,

                    totalGameElapsedTime =
                        totalGameElapsedTime
                };


            string jsonData =
                JsonConvert.SerializeObject(
                    dataToSend
                );


            await SendJson(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "TCP SEND ERROR:\n" +
                ex.Message
            );

            isConnected = false;
        }
    }


    // =====================================================
    // LOG EVENT
    // =====================================================

    public void LogEvent(
        string eventType,
        string eventName,
        string eventMessage = ""
    )
    {
        if (!isRunning)
            return;

        int mazeNumber = 0;
        int attemptNumber = 0;

        float mazeTime = 0f;
        float totalTime = 0f;


        if (gameManager != null)
        {
            mazeNumber =
                gameManager.CurrentMaze;

            attemptNumber =
                gameManager.CurrentAttempt;

            mazeTime =
                gameManager.CurrentMazeElapsedTime;

            totalTime =
                gameManager.TotalGameElapsedTime;
        }


        EventData eventData =
            new EventData
            {
                recordType = "EVENT",

                timestamp =
                    GetTimestamp(),

                eventType =
                    eventType,

                eventName =
                    eventName,

                eventMessage =
                    eventMessage,

                mazeNumber =
                    mazeNumber,

                attemptNumber =
                    attemptNumber,

                mazeElapsedTime =
                    mazeTime,

                totalGameElapsedTime =
                    totalTime
            };


        string jsonData =
            JsonConvert.SerializeObject(
                eventData
            );


        _ = SendEventJson(jsonData);


        Debug.Log(
            "========== GAME EVENT ==========\n" +
            "Type: " +
            eventType +
            "\nName: " +
            eventName +
            "\nMessage: " +
            eventMessage +
            "\nMaze: " +
            mazeNumber +
            "\nAttempt: " +
            attemptNumber +
            "\nMaze Time: " +
            mazeTime.ToString("F3") +
            "\nTotal Time: " +
            totalTime.ToString("F3")
        );
    }


    // =====================================================
    // SEND EVENT JSON
    // =====================================================

    private async Task SendEventJson(
        string jsonData
    )
    {
        if (!CanSend())
            return;

        try
        {
            await SendJson(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "TCP EVENT SEND ERROR:\n" +
                ex.Message
            );

            isConnected = false;
        }
    }


    // =====================================================
    // SEND EXPERIMENT SUMMARY
    // =====================================================

    public async void SendExperimentSummary(
        string finalResult,
        float totalGameTime,
        int finalScore,
        int highestCompletedMaze,
        float startRoomDuration,
        List<MazeVisitRecord> mazeVisits
    )
    {
        if (!isRunning)
            return;

        if (!isConnected)
        {
            Debug.LogError(
                "TCP: Cannot send experiment summary. " +
                "TCP is not connected."
            );

            return;
        }

        if (mazeVisits == null)
        {
            mazeVisits =
                new List<MazeVisitRecord>();
        }


        // Make a copy so the original list
        // cannot change while serializing.

        List<MazeVisitRecord> visitCopy =
            new List<MazeVisitRecord>(
                mazeVisits
            );


        ExperimentSummaryData summary =
            new ExperimentSummaryData
            {
                messageType =
                    "EXPERIMENT_SUMMARY",

                recordType =
                    "EXPERIMENT_SUMMARY",

                timestamp =
                    GetTimestamp(),

                finalResult =
                    finalResult,

                totalGameTime =
                    totalGameTime,

                finalScore =
                    finalScore,

                highestCompletedMaze =
                    highestCompletedMaze,

                startRoomDuration =
                    startRoomDuration,

                mazeVisits =
                    visitCopy
            };


        try
        {
            string jsonData =
                JsonConvert.SerializeObject(
                    summary
                );


            Debug.Log(
                "============================================\n" +
                "SENDING EXPERIMENT SUMMARY\n" +
                "============================================\n" +
                "Final Result: " +
                finalResult +
                "\nTotal Game Time: " +
                totalGameTime.ToString("F3") +
                "\nFinal Score: " +
                finalScore +
                "\nHighest Completed Maze: " +
                highestCompletedMaze +
                "\nMaze Visits: " +
                visitCopy.Count +
                "\n============================================"
            );


            await SendJson(jsonData);


            Debug.Log(
                "EXPERIMENT SUMMARY SENT SUCCESSFULLY."
            );
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "EXPERIMENT SUMMARY SEND ERROR:\n" +
                ex.Message
            );
        }
    }


    // =====================================================
    // SEND JSON
    // =====================================================

    private async Task SendJson(
        string jsonData
    )
    {
        if (!CanSend())
            return;


        await sendLock.WaitAsync();

        try
        {
            byte[] jsonBytes =
                Encoding.UTF8.GetBytes(
                    jsonData
                );


            // =================================================
            // 4-BYTE BIG-ENDIAN LENGTH PREFIX
            // Compatible with Python:
            // struct.unpack("!I", ...)
            // =================================================

            byte[] lengthPrefix =
                BitConverter.GetBytes(
                    IPAddress.HostToNetworkOrder(
                        jsonBytes.Length
                    )
                );


            await netStream.WriteAsync(
                lengthPrefix,
                0,
                lengthPrefix.Length
            );


            await netStream.WriteAsync(
                jsonBytes,
                0,
                jsonBytes.Length
            );


            await netStream.FlushAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "TCP WRITE ERROR:\n" +
                ex.Message
            );

            isConnected = false;
        }
        finally
        {
            sendLock.Release();
        }
    }


    // =====================================================
    // DATA HELPERS
    // =====================================================

    private bool CanSend()
    {
        return isRunning &&
               isConnected &&
               netStream != null &&
               netStream.CanWrite;
    }


    private Vector3 GetPosition(
        Transform target
    )
    {
        return target != null
            ? target.position
            : Vector3.zero;
    }


    private Vector3 GetRotation(
        Transform target
    )
    {
        return target != null
            ? target.eulerAngles
            : Vector3.zero;
    }


    private Vector2 ReadVector2(
        InputActionReference actionReference
    )
    {
        if (actionReference == null ||
            actionReference.action == null)
        {
            return Vector2.zero;
        }

        return actionReference.action
            .ReadValue<Vector2>();
    }


    private float ReadFloat(
        InputActionReference actionReference
    )
    {
        if (actionReference == null ||
            actionReference.action == null)
        {
            return 0f;
        }

        return actionReference.action
            .ReadValue<float>();
    }


    // =====================================================
    // CONVERT ROTATION
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
    // CLOSE CONNECTION
    // =====================================================

    private void CloseConnection()
    {
        isRunning = false;
        isConnected = false;


        try
        {
            netStream?.Close();
            netStream = null;

            client?.Close();
            client = null;
        }
        catch
        {
            // Ignore errors during shutdown.
        }


        Debug.Log(
            "TCP connection closed."
        );
    }


    // =====================================================
    // DESTROY
    // =====================================================

    private void OnDestroy()
    {
        DisableInputs();

        CloseConnection();


        try
        {
            sendLock.Dispose();
        }
        catch
        {
            // Ignore disposal errors.
        }
    }


    // =====================================================
    // APPLICATION QUIT
    // =====================================================

    private void OnApplicationQuit()
    {
        CloseConnection();
    }
}


// =========================================================
// CONTINUOUS DATA
// =========================================================

[Serializable]
public class SentData
{
    public string recordType;

    public float timestamp;


    // HEAD

    public float headPositionX;
    public float headPositionY;
    public float headPositionZ;

    public float headRotationX;
    public float headRotationY;
    public float headRotationZ;


    // PLAYER

    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    public float playerRotationX;
    public float playerRotationY;
    public float playerRotationZ;


    // RIGHT CONTROLLER

    public float rightControllerPositionX;
    public float rightControllerPositionY;
    public float rightControllerPositionZ;

    public float rightControllerRotationX;
    public float rightControllerRotationY;
    public float rightControllerRotationZ;


    // LEFT CONTROLLER

    public float leftControllerPositionX;
    public float leftControllerPositionY;
    public float leftControllerPositionZ;

    public float leftControllerRotationX;
    public float leftControllerRotationY;
    public float leftControllerRotationZ;


    // RIGHT INPUT

    public float rightThumbstickX;
    public float rightThumbstickY;

    public float rightTrigger;
    public float rightGrab;


    // LEFT INPUT

    public float leftThumbstickX;
    public float leftThumbstickY;

    public float leftTrigger;


    // GAME DATA

    public int mazeNumber;
    public int attemptNumber;

    public int collectedCoins;
    public int totalCoins;

    public int totalScore;

    public float mazeElapsedTime;
    public float totalGameElapsedTime;
}


// =========================================================
// EVENT DATA
// =========================================================

[Serializable]
public class EventData
{
    public string recordType;

    public float timestamp;

    public string eventType;
    public string eventName;
    public string eventMessage;

    public int mazeNumber;
    public int attemptNumber;

    public float mazeElapsedTime;
    public float totalGameElapsedTime;
}


// =========================================================
// EXPERIMENT SUMMARY
// =========================================================

[Serializable]
public class ExperimentSummaryData
{
    public string messageType;

    public string recordType;

    public float timestamp;


    // FINAL RESULT

    public string finalResult;

    public float totalGameTime;

    public int finalScore;

    public int highestCompletedMaze;


    // START ROOM

    public float startRoomDuration;


    // ALL MAZE VISITS

    public List<MazeVisitRecord> mazeVisits;
}