using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class TCPSettingsClient : MonoBehaviour
{
    // =====================================================
    // NETWORK
    // =====================================================

    [Header("Settings TCP Connection")]

    [Tooltip("IP address of laptop")]
    public string pcIPAddress = "192.168.1.100";

    [Tooltip("Settings TCP port")]
    public int port = 12346;


    // =====================================================
    // REFERENCES
    // =====================================================

    [Header("Loading Panel")]

    public GameObject settingsLoadingPanel;

    [Header("References")]

    public EnvironmentManager environmentManager;

    public MusicManager musicManager;

    public GameManager gameManager;


    // =====================================================
    // TCP
    // =====================================================

    private TcpClient client;

    private NetworkStream networkStream;

    private bool connected = false;

    private bool shuttingDown = false;


    // =====================================================
    // START
    // =====================================================

    private async void Start()
    {
        // =================================================
        // SHOW LOADING PANEL
        // =================================================

        if (settingsLoadingPanel != null)
        {
            settingsLoadingPanel.SetActive(true);
        }


        // =================================================
        // FIND REFERENCES
        // =================================================

        if (environmentManager == null)
        {
            environmentManager =
                FindFirstObjectByType<EnvironmentManager>();
        }

        if (musicManager == null)
        {
            musicManager =
                FindFirstObjectByType<MusicManager>();
        }

        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }


        Debug.Log(
            "TCPSettingsClient started."
        );


        Debug.Log(
            "Connecting to settings server: " +
            pcIPAddress +
            ":" +
            port
        );


        await ConnectToServer();
    }

    // =====================================================
    // CONNECT
    // =====================================================

    private async Task ConnectToServer()
    {
        try
        {
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
                "===================================="
            );

            Debug.Log(
                "SETTINGS TCP CONNECTED"
            );

            Debug.Log(
                "Waiting for settings from laptop..."
            );

            Debug.Log(
                "===================================="
            );


            await ReceiveSettings();
        }
        catch (Exception ex)
        {
            connected = false;


            if (!shuttingDown)
            {
                Debug.LogError(
                    "Settings TCP connection failed: " +
                    ex.Message
                );
            }
        }
    }


    // =====================================================
    // RECEIVE SETTINGS
    // =====================================================

    private async Task ReceiveSettings()
    {
        try
        {
            while (
                connected &&
                !shuttingDown &&
                networkStream != null
            )
            {
                // -----------------------------------------
                // RECEIVE 4 BYTE LENGTH
                // -----------------------------------------

                byte[] lengthBytes =
                    await ReceiveExact(
                        networkStream,
                        4
                    );


                if (lengthBytes == null)
                {
                    Debug.Log(
                        "Settings server disconnected."
                    );

                    break;
                }


                // -----------------------------------------
                // BIG ENDIAN LENGTH
                // -----------------------------------------

                int messageLength =
                    (lengthBytes[0] << 24) |
                    (lengthBytes[1] << 16) |
                    (lengthBytes[2] << 8) |
                    lengthBytes[3];


                // -----------------------------------------
                // SAFETY CHECK
                // -----------------------------------------

                if (
                    messageLength <= 0 ||
                    messageLength > 100000
                )
                {
                    Debug.LogError(
                        "Invalid settings message length: " +
                        messageLength
                    );

                    break;
                }


                // -----------------------------------------
                // RECEIVE JSON
                // -----------------------------------------

                byte[] jsonBytes =
                    await ReceiveExact(
                        networkStream,
                        messageLength
                    );


                if (jsonBytes == null)
                    break;


                string json =
                    Encoding.UTF8.GetString(
                        jsonBytes
                    );


                Debug.Log(
                    "===================================="
                );

                Debug.Log(
                    "SETTINGS JSON RECEIVED:"
                );

                Debug.Log(json);

                Debug.Log(
                    "===================================="
                );


                // -----------------------------------------
                // DESERIALIZE
                // -----------------------------------------

                SettingsData settings =
                    JsonConvert.DeserializeObject<SettingsData>(
                        json
                    );


                if (settings == null)
                {
                    Debug.LogError(
                        "Could not deserialize settings."
                    );

                    continue;
                }


                // -----------------------------------------
                // APPLY SETTINGS
                // -----------------------------------------

                ApplySettings(settings);
            }
        }
        catch (Exception ex)
        {
            if (!shuttingDown)
            {
                Debug.LogError(
                    "Settings receive error: " +
                    ex.Message
                );
            }
        }
    }


    // =====================================================
    // RECEIVE EXACT BYTES
    // =====================================================

    private async Task<byte[]> ReceiveExact(
        NetworkStream stream,
        int size
    )
    {
        byte[] data =
            new byte[size];

        int totalReceived = 0;


        while (totalReceived < size)
        {
            int received =
                await stream.ReadAsync(
                    data,
                    totalReceived,
                    size - totalReceived
                );


            if (received <= 0)
            {
                return null;
            }


            totalReceived += received;
        }


        return data;
    }


    // =====================================================
    // APPLY SETTINGS
    // =====================================================

    private void ApplySettings(
        SettingsData settings
    )
    {
        Debug.Log(
            "===================================="
        );

        Debug.Log(
            "APPLYING NEW SETTINGS"
        );


        // =================================================
        // ENVIRONMENT
        // =================================================

        if (environmentManager != null)
        {
            try
            {
                EnvironmentManager.EnvironmentType
                    selectedEnvironment;


                bool validEnvironment =
                    Enum.TryParse(
                        settings.environment,
                        true,
                        out selectedEnvironment
                    );


                if (!validEnvironment)
                {
                    Debug.LogError(
                        "Invalid environment received: " +
                        settings.environment
                    );
                }
                else
                {
                    environmentManager.environmentType =
                        selectedEnvironment;


                    environmentManager.ApplyEnvironment();


                    Debug.Log(
                        "Environment applied: " +
                        selectedEnvironment
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    "Environment error: " +
                    ex.Message
                );
            }
        }
        else
        {
            Debug.LogError(
                "EnvironmentManager reference is missing!"
            );
        }


        // =================================================
        // MUSIC
        // =================================================

        if (musicManager != null)
        {
            try
            {
                MusicManager.MusicMode
                    selectedMusic;


                bool validMusic =
                    Enum.TryParse(
                        settings.music,
                        true,
                        out selectedMusic
                    );


                if (!validMusic)
                {
                    Debug.LogError(
                        "Invalid music received: " +
                        settings.music
                    );
                }
                else
                {
                    musicManager.musicMode =
                        selectedMusic;


                    musicManager.ApplyMusicMode();


                    Debug.Log(
                        "Music applied: " +
                        selectedMusic
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    "Music error: " +
                    ex.Message
                );
            }
        }
        else
        {
            Debug.LogError(
                "MusicManager reference is missing!"
            );
        }


        // =================================================
        // GAME MODE
        // =================================================

        if (gameManager != null)
        {
            try
            {
                GameManager.ScoreMode
                    selectedScoreMode;


                bool validGameMode =
                    Enum.TryParse(
                        settings.gameMode,
                        true,
                        out selectedScoreMode
                    );


                if (!validGameMode)
                {
                    Debug.LogError(
                        "Invalid game mode received: " +
                        settings.gameMode
                    );
                }
                else
                {
                    gameManager.scoreMode =
                        selectedScoreMode;


                    Debug.Log(
                        "Game Mode applied: " +
                        selectedScoreMode
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    "Game mode error: " +
                    ex.Message
                );
            }
        }
        else
        {
            Debug.LogError(
                "GameManager reference is missing!"
            );
        }


        Debug.Log(
            "===================================="
        );

        Debug.Log(
            "SETTINGS APPLIED"
        );

        Debug.Log(
            "Environment = " +
            settings.environment
        );

        Debug.Log(
            "Music = " +
            settings.music
        );

        Debug.Log(
            "GameMode = " +
            settings.gameMode
        );

        Debug.Log(
            "===================================="
        );
        // =================================================
        // SETTINGS READY
        // =================================================

        if (settingsLoadingPanel != null)
        {
            settingsLoadingPanel.SetActive(false);
        }

        Debug.Log(
            "Game settings loaded. Starting game."
        );
    }


    // =====================================================
    // CLEANUP
    // =====================================================

    private void OnDestroy()
    {
        shuttingDown = true;

        connected = false;


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
    }
}


// =========================================================
// SETTINGS DATA
// =========================================================

[Serializable]
public class SettingsData
{
    public string environment;

    public string music;

    public string gameMode;
}