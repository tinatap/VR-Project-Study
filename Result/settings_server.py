import socket
import json
import struct


# =========================================================
# SETTINGS
# =========================================================

HOST = "0.0.0.0"
PORT = 12346


# =========================================================
# CHOOSE SETTINGS
# =========================================================

def choose_settings():

    print()
    print("====================================")
    print("VR MAZE SETTINGS")
    print("====================================")


    # -----------------------------------------------------
    # MUSIC
    # -----------------------------------------------------

    print()
    print("Music:")
    print("1 - Relaxing")
    print("2 - Motivating")

    while True:
        try:
            music = int(input("Enter choice: "))

            if music in [1, 2]:
                break

            print("Please enter 1 or 2.")

        except ValueError:
            print("Please enter a number.")


    # -----------------------------------------------------
    # ENVIRONMENT
    # -----------------------------------------------------

    print()
    print("Environment:")
    print("1 - Galaxy")
    print("2 - Desert")
    print("3 - Neutral")
    print("4 - Park")

    while True:
        try:
            environment = int(input("Enter choice: "))

            if environment in [1, 2, 3, 4]:
                break

            print("Please enter a number between 1 and 4.")

        except ValueError:
            print("Please enter a number.")


    # -----------------------------------------------------
    # AVATAR
    # -----------------------------------------------------

    print()
    print("Avatar:")
    print("1 - OverWeightedWoman")
    print("2 - UnderWeightedWoman")
    print("3 - OverWeightedMan")
    print("4 - UnderWeightedMan")
    print("5 - OldWoman")
    print("6 - YoungWoman")
    print("7 - OldMan")
    print("8 - YoungMan")

    while True:
        try:
            avatar = int(input("Enter choice: "))

            if avatar in range(1, 9):
                break

            print("Please enter a number between 1 and 8.")

        except ValueError:
            print("Please enter a number.")


    # -----------------------------------------------------
    # GAME MODE
    # -----------------------------------------------------

    game_mode = "CoinsAndMazeScore"


    # -----------------------------------------------------
    # CREATE SETTINGS
    # -----------------------------------------------------

    settings = {
        "environment": environment,
        "music": music,
        "avatar": avatar,
        "gameMode": game_mode
    }


    # -----------------------------------------------------
    # SHOW SELECTED SETTINGS
    # -----------------------------------------------------

    music_names = {
        1: "Relaxing",
        2: "Motivating"
    }

    environment_names = {
        1: "Galaxy",
        2: "Desert",
        3: "Neutral",
        4: "Park"
    }

    avatar_names = {
        1: "OverWeightedWoman",
        2: "UnderWeightedWoman",
        3: "OverWeightedMan",
        4: "UnderWeightedMan",
        5: "OldWoman",
        6: "YoungWoman",
        7: "OldMan",
        8: "YoungMan"
    }


    print()
    print("====================================")
    print("SELECTED SETTINGS")
    print("====================================")
    print(f"Music:        {music_names[music]}")
    print(f"Environment:  {environment_names[environment]}")
    print(f"Avatar:       {avatar_names[avatar]}")
    print(f"Game Mode:    {game_mode}")
    print("====================================")


    return settings


# =========================================================
# CREATE MESSAGE
# =========================================================

def create_message(settings_data):

    # Convert dictionary to JSON
    json_data = json.dumps(
        settings_data,
        separators=(",", ":")
    )

    # Convert JSON to UTF-8 bytes
    json_bytes = json_data.encode("utf-8")

    # 4-byte BIG-ENDIAN length prefix
    length_prefix = struct.pack(
        "!I",
        len(json_bytes)
    )

    return length_prefix + json_bytes


# =========================================================
# GET SETTINGS FROM USER
# =========================================================

settings = choose_settings()


# =========================================================
# START SERVER
# =========================================================

server = socket.socket(
    socket.AF_INET,
    socket.SOCK_STREAM
)

server.setsockopt(
    socket.SOL_SOCKET,
    socket.SO_REUSEADDR,
    1
)

server.bind(
    (HOST, PORT)
)

server.listen(5)


print()
print("====================================")
print("TCP SETTINGS SERVER")
print("====================================")
print(f"Listening on {HOST}:{PORT}")
print("Waiting for Unity / Quest...")
print("====================================")


# =========================================================
# WAIT FOR QUEST
# =========================================================

while True:

    client_socket, client_address = server.accept()

    print()
    print("====================================")
    print("QUEST CONNECTED")
    print(f"Address: {client_address}")
    print("====================================")

    try:

        # -------------------------------------------------
        # Create settings message
        # -------------------------------------------------

        message = create_message(settings)


        print("Sending settings:")
        print(json.dumps(
            settings,
            indent=4
        ))


        # -------------------------------------------------
        # Send message
        # -------------------------------------------------

        client_socket.sendall(message)


        print("Settings sent successfully.")

    except Exception as e:

        print(
            "Error sending settings:",
            e
        )

    finally:

        client_socket.close()

        print("Connection closed.")