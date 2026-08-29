import socket
import json
import struct


# =========================================================
# SETTINGS
# =========================================================

HOST = "0.0.0.0"
PORT = 12346


# =========================================================
# SETTINGS DATA
# =========================================================

settings = {
    "environment": "Neutral",
    "music": "Calm",
    "gameMode": "CoinsAndMazeScore",
    "avatar": "Female"
}


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