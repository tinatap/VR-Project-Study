using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TCP : MonoBehaviour
{
    // ===================================================== 
    // TCP CONNECTION 
    // ===================================================== 

    [Header("TCP Connection")]

    [SerializeField] private string HOST = "127.0.0.1";

    [SerializeField] private int PORT = 12345;

    private TcpClient client;

    private NetworkStream netStream;

    private bool isRunning = true;

    private bool isConnected = false;


    // ===================================================== 
    // PLAYER 
    // ===================================================== 

    [Header("Player")]

    [SerializeField] private Transform player;

    [SerializeField] private Transform centerEyeAnchorTransform;


    // ===================================================== 
    // CONTROLLER TRANSFORMS 
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

    [Tooltip("Movement data is sent every X seconds.")]
    [SerializeField]
    private float sendInterval = 0.05f;

    private float sendTimer = 0f;

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
        if (rightThumbstickAction != null)
            rightThumbstickAction.action.Enable();

        if (rightTriggerAction != null)
            rightTriggerAction.action.Enable();

        if (rightGrabAction != null)
            rightGrabAction.action.Enable();

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

        if (rightGrabAction != null)
            rightGrabAction.action.Disable();

        if (leftThumbstickAction != null)
            leftThumbstickAction.action.Disable();

        if (leftTriggerAction != null)
            leftTriggerAction.action.Disable();
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
                return;

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
        if (!isRunning)
            return;

        if (!isConnected)
            return;

        if (netStream == null)
            return;

        if (!netStream.CanWrite)
            return;

        sendTimer += Time.deltaTime;

        if (sendTimer >= sendInterval)
        {
            sendTimer = 0f;

            _ = SendCurrentData();
        }
    }


    // ===================================================== 
    // GET TCP TIMESTAMP 
    // ===================================================== 

    private float GetTimestamp()
    {
        return (float)Math.Round(
            (DateTime.Now - startTime).TotalSeconds,
            3
        );
    }


    // ===================================================== 
    // SEND CURRENT DATA 
    // ===================================================== 

    private async Task SendCurrentData()
    {
        if (!isConnected)
            return;

        if (netStream == null)
            return;

        if (!netStream.CanWrite)
            return;

        try
        {
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
            // CONTROLLERS 
            // ================================================= 

            Vector3 rightControllerPosition =
                Vector3.zero;

            Vector3 leftControllerPosition =
                Vector3.zero;

            if (rightControllerTransform != null)
            {
                rightControllerPosition =
                    rightControllerTransform.position;
            }

            if (leftControllerTransform != null)
            {
                leftControllerPosition =
                    leftControllerTransform.position;
            }


            // ================================================= 
            // INPUT 
            // ================================================= 

            Vector2 rightThumbstick =
                Vector2.zero;

            float rightTrigger = 0f;

            float rightGrab = 0f;

            Vector2 leftThumbstick =
                Vector2.zero;

            float leftTrigger = 0f;


            if (rightThumbstickAction != null)
            {
                rightThumbstick =
                    rightThumbstickAction.action
                    .ReadValue<Vector2>();
            }


            if (rightTriggerAction != null)
            {
                rightTrigger =
                    rightTriggerAction.action
                    .ReadValue<float>();
            }


            if (rightGrabAction != null)
            {
                rightGrab =
                    rightGrabAction.action
                    .ReadValue<float>();
            }


            if (leftThumbstickAction != null)
            {
                leftThumbstick =
                    leftThumbstickAction.action
                    .ReadValue<Vector2>();
            }


            if (leftTriggerAction != null)
            {
                leftTrigger =
                    leftTriggerAction.action
                    .ReadValue<float>();
            }


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
            // CREATE DATA 
            // ================================================= 

            SentData dataToSend =
                new SentData
                {
                    recordType = "DATA",

                    timestamp =
                        GetTimestamp(),

                    headPositionX =
                        headPosition.x,

                    headPositionY =
                        headPosition.y,

                    headPositionZ =
                        headPosition.z,


                    headRotationX =
                        ConvertRotation(
                            headRotation.x
                        ),

                    headRotationY =
                        ConvertRotation(
                            headRotation.y
                        ),

                    headRotationZ =
                        ConvertRotation(
                            headRotation.z
                        ),


                    playerPositionX =
                        playerPosition.x,

                    playerPositionY =
                        playerPosition.y,

                    playerPositionZ =
                        playerPosition.z,


                    playerRotationX =
                        ConvertRotation(
                            playerRotation.x
                        ),

                    playerRotationY =
                        ConvertRotation(
                            playerRotation.y
                        ),

                    playerRotationZ =
                        ConvertRotation(
                            playerRotation.z
                        ),


                    rightControllerPositionX =
                        rightControllerPosition.x,

                    rightControllerPositionY =
                        rightControllerPosition.y,

                    rightControllerPositionZ =
                        rightControllerPosition.z,


                    leftControllerPositionX =
                        leftControllerPosition.x,

                    leftControllerPositionY =
                        leftControllerPosition.y,

                    leftControllerPositionZ =
                        leftControllerPosition.z,


                    rightThumbstickX =
                        rightThumbstick.x,

                    rightThumbstickY =
                        rightThumbstick.y,

                    rightTrigger =
                        rightTrigger,

                    rightGrab =
                        rightGrab,


                    leftThumbstickX =
                        leftThumbstick.x,

                    leftThumbstickY =
                        leftThumbstick.y,

                    leftTrigger =
                        leftTrigger,


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


        // Event is sent immediately. 
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
        if (!isConnected)
            return;

        if (netStream == null)
            return;

        if (!netStream.CanWrite)
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
    // SEND JSON 
    // ===================================================== 

    private async Task SendJson(
        string jsonData
    )
    {
        if (!isConnected)
            return;

        if (netStream == null)
            return;

        if (!netStream.CanWrite)
            return;


        await sendLock.WaitAsync();

        try
        {
            byte[] jsonBytes =
                Encoding.UTF8.GetBytes(
                    jsonData
                );


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
        finally
        {
            sendLock.Release();
        }
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

        sendLock.Dispose();
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
// SENT DATA 
// ========================================================= 

[Serializable]
public class SentData
{
    // ===================================================== 
    // RECORD TYPE 
    // ===================================================== 

    public string recordType;


    // ===================================================== 
    // TIME 
    // ===================================================== 

    public float timestamp;


    // ===================================================== 
    // HEAD POSITION 
    // ===================================================== 

    public float headPositionX;
    public float headPositionY;
    public float headPositionZ;


    // ===================================================== 
    // HEAD ROTATION 
    // ===================================================== 

    public float headRotationX;
    public float headRotationY;
    public float headRotationZ;


    // ===================================================== 
    // PLAYER POSITION 
    // ===================================================== 

    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;


    // ===================================================== 
    // PLAYER ROTATION 
    // ===================================================== 

    public float playerRotationX;
    public float playerRotationY;
    public float playerRotationZ;


    // ===================================================== 
    // RIGHT CONTROLLER 
    // ===================================================== 

    public float rightControllerPositionX;
    public float rightControllerPositionY;
    public float rightControllerPositionZ;


    // ===================================================== 
    // LEFT CONTROLLER 
    // ===================================================== 

    public float leftControllerPositionX;
    public float leftControllerPositionY;
    public float leftControllerPositionZ;


    // ===================================================== 
    // RIGHT INPUT 
    // ===================================================== 

    public float rightThumbstickX;
    public float rightThumbstickY;

    public float rightTrigger;

    public float rightGrab;


    // ===================================================== 
    // LEFT INPUT 
    // ===================================================== 

    public float leftThumbstickX;
    public float leftThumbstickY;

    public float leftTrigger;


    // ===================================================== 
    // GAME DATA 
    // ===================================================== 

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
    // ===================================================== 
    // RECORD TYPE 
    // ===================================================== 

    public string recordType;


    // ===================================================== 
    // TIME 
    // ===================================================== 

    public float timestamp;


    // ===================================================== 
    // EVENT 
    // ===================================================== 

    public string eventType;

    public string eventName;

    public string eventMessage;


    // ===================================================== 
    // GAME DATA 
    // ===================================================== 

    public int mazeNumber;

    public int attemptNumber;

    public float mazeElapsedTime;

    public float totalGameElapsedTime;
}
