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

    [Header("References")]

    public EnvironmentManager environmentManager;
    public MusicManager musicManager;
    public GameManager gameManager;
    public AvatarManager avatarManager;


    // =====================================================
    // TCP
    // =====================================================

    private TcpClient client;
    private NetworkStream networkStream;

    private bool connected;
    private bool shuttingDown;

    private const int LengthHeaderSize = 4;
    private const int MaxMessageLength = 100000;


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

            networkStream = client.GetStream();
            connected = true;

            Debug.Log(
                "====================================\n" +
                "SETTINGS TCP CONNECTED\n" +
                "Waiting for settings from laptop...\n" +
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
    // START
    // =====================================================

    private async void Start()
    {
        FindReferences();

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
    // FIND REFERENCES
    // =====================================================

    private void FindReferences()
    {
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

        if (avatarManager == null)
        {
            avatarManager =
                FindFirstObjectByType<AvatarManager>();
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
                        LengthHeaderSize
                    );

                if (lengthBytes == null)
                {
                    HandleServerDisconnect();
                    break;
                }


                // -----------------------------------------
                // BIG ENDIAN LENGTH
                // -----------------------------------------

                int messageLength =
                    ReadBigEndianInt32(lengthBytes);


                // -----------------------------------------
                // SAFETY CHECK
                // -----------------------------------------

                if (
                    messageLength <= 0 ||
                    messageLength > MaxMessageLength
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


                // -----------------------------------------
                // DEBUG LOG
                // -----------------------------------------

                Debug.Log(
                    "====================================\n" +
                    "SETTINGS JSON RECEIVED:\n" +
                    json +
                    "\n===================================="
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
        finally
        {
            connected = false;
        }
    }


    // =====================================================
    // READ BIG ENDIAN INT32
    // =====================================================

    private int ReadBigEndianInt32(byte[] bytes)
    {
        return
            (bytes[0] << 24) |
            (bytes[1] << 16) |
            (bytes[2] << 8) |
            bytes[3];
    }


    // =====================================================
    // RECEIVE EXACT BYTES
    // =====================================================

    private async Task<byte[]> ReceiveExact(
        NetworkStream stream,
        int size
    )
    {
        if (stream == null || size <= 0)
            return null;

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
                return null;

            totalReceived += received;
        }

        return data;
    }


    // =====================================================
    // APPLY SETTINGS
    // =====================================================

    private void ApplySettings(SettingsData settings)
    {
        if (settings == null)
        {
            Debug.LogError(
                "Cannot apply null settings."
            );

            return;
        }

        Debug.Log(
            "====================================\n" +
            "APPLYING NEW SETTINGS"
        );


        // =================================================
        // ENVIRONMENT
        // =================================================

        ApplyEnvironment(
            settings.environment
        );


        // =================================================
        // MUSIC
        // =================================================

        ApplyMusic(
            settings.music
        );


        // =================================================
        // GAME MODE
        // =================================================

        ApplyGameMode(
            settings.gameMode
        );


        // =================================================
        // AVATAR
        // =================================================

        ApplyAvatar(
            settings.avatar
        );


        // =================================================
        // SETTINGS LOG
        // =================================================

        Debug.Log(
            "====================================\n" +
            "SETTINGS APPLIED\n" +
            "Environment = " + settings.environment + "\n" +
            "Music = " + settings.music + "\n" +
            "GameMode = " + settings.gameMode + "\n" +
            "Avatar = " + settings.avatar + "\n" +
            "===================================="
        );
    }


    // =====================================================
    // APPLY ENVIRONMENT
    // =====================================================

    private void ApplyEnvironment(int environmentNumber)
    {
        if (environmentManager == null)
        {
            Debug.LogError(
                "EnvironmentManager reference is missing!"
            );

            return;
        }

        try
        {
            if (!TryGetEnvironment(
                    environmentNumber,
                    out EnvironmentManager.EnvironmentType environmentType))
            {
                Debug.LogError(
                    "Invalid environment number received: " +
                    environmentNumber
                );

                return;
            }

            environmentManager.environmentType =
                environmentType;

            environmentManager.ApplyEnvironment();

            Debug.Log(
                "Environment applied: " +
                environmentType
            );
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "Environment error: " +
                ex.Message
            );
        }
    }


    // =====================================================
    // ENVIRONMENT MAPPING
    // =====================================================

    private bool TryGetEnvironment(
        int number,
        out EnvironmentManager.EnvironmentType environmentType
    )
    {
        switch (number)
        {
            case 1:
                environmentType =
                    EnvironmentManager.EnvironmentType.Galaxy;
                return true;

            case 2:
                environmentType =
                    EnvironmentManager.EnvironmentType.Desert;
                return true;

            case 3:
                environmentType =
                    EnvironmentManager.EnvironmentType.Neutral;
                return true;

            case 4:
                environmentType =
                    EnvironmentManager.EnvironmentType.Park;
                return true;

            default:
                environmentType =
                    default;
                return false;
        }
    }


    // =====================================================
    // APPLY MUSIC
    // =====================================================

    private void ApplyMusic(int musicNumber)
    {
        if (musicManager == null)
        {
            Debug.LogError(
                "MusicManager reference is missing!"
            );

            return;
        }

        try
        {
            if (!TryGetMusicMode(
                    musicNumber,
                    out MusicManager.MusicMode musicMode))
            {
                Debug.LogError(
                    "Invalid music number received: " +
                    musicNumber
                );

                return;
            }

            musicManager.musicMode =
                musicMode;

            musicManager.ApplyMusicMode();

            Debug.Log(
                "Music applied: " +
                musicMode
            );
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "Music error: " +
                ex.Message
            );
        }
    }


    // =====================================================
    // MUSIC MAPPING
    // =====================================================

    private bool TryGetMusicMode(
    int number,
    out MusicManager.MusicMode musicMode
)
    {
        switch (number)
        {
            case 1:
                musicMode =
                    MusicManager.MusicMode.Relaxing;
                return true;

            case 2:
                musicMode =
                    MusicManager.MusicMode.Motivating;
                return true;

            case 3:
                musicMode =
                    MusicManager.MusicMode.NoMusic;
                return true;

            default:
                musicMode = default;
                return false;
        }
    }

    // =====================================================
    // APPLY GAME MODE
    // =====================================================

    private void ApplyGameMode(string gameMode)
    {
        if (gameManager == null)
        {
            Debug.LogError(
                "GameManager reference is missing!"
            );

            return;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(gameMode))
            {
                Debug.LogError(
                    "Game mode received is empty."
                );

                return;
            }

            if (!Enum.TryParse(
                    gameMode,
                    true,
                    out GameManager.ScoreMode scoreMode))
            {
                Debug.LogError(
                    "Invalid game mode received: " +
                    gameMode
                );

                return;
            }

            gameManager.scoreMode =
                scoreMode;

            Debug.Log(
                "Game Mode applied: " +
                scoreMode
            );
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "Game mode error: " +
                ex.Message
            );
        }
    }


    // =====================================================
    // APPLY AVATAR
    // =====================================================

    private void ApplyAvatar(int avatarNumber)
    {
        if (avatarManager == null)
        {
            Debug.LogError(
                "AvatarManager reference is missing!"
            );

            return;
        }

        try
        {
            string avatarName =
                GetAvatarName(avatarNumber);

            if (string.IsNullOrEmpty(avatarName))
            {
                Debug.LogError(
                    "Invalid avatar number received: " +
                    avatarNumber
                );

                return;
            }

            bool avatarApplied =
                avatarManager.SelectAvatarByName(
                    avatarName
                );

            if (!avatarApplied)
            {
                Debug.LogError(
                    "Avatar not found in AvatarManager: " +
                    avatarName
                );

                return;
            }

            Debug.Log(
                "Avatar applied: " +
                avatarName
            );
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "Avatar error: " +
                ex.Message
            );
        }
    }


    // =====================================================
    // AVATAR MAPPING
    // =====================================================

    private string GetAvatarName(int avatarNumber)
    {
        switch (avatarNumber)
        {
            case 1:
                return "OverWeightedWoman";

            case 2:
                return "UnderWeightedWoman";

            case 3:
                return "OverWeightedMan";

            case 4:
                return "UnderWeightedMan";

            case 5:
                return "OldWoman";

            case 6:
                return "YoungWoman";

            case 7:
                return "OldMan";

            case 8:
                return "YoungMan";

            default:
                return null;
        }
    }


    // =====================================================
    // SERVER DISCONNECT
    // =====================================================

    private void HandleServerDisconnect()
    {
        connected = false;

        if (!shuttingDown)
        {
            Debug.Log(
                "Settings server disconnected."
            );
        }
    }


    // =====================================================
    // CLEANUP
    // =====================================================

    private void OnDestroy()
    {
        shuttingDown = true;
        connected = false;

        CloseConnection();
    }


    // =====================================================
    // CLOSE CONNECTION
    // =====================================================

    private void CloseConnection()
    {
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
            // Ignore cleanup exceptions.
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
            // Ignore cleanup exceptions.
        }
    }
}


// =========================================================
// SETTINGS DATA
// =========================================================

[Serializable]
public class SettingsData
{
    public int environment;
    public int music;
    public string gameMode;
    public int avatar;
}