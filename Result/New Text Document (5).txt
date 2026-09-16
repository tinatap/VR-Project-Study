import socket
import struct
import json
import os
from datetime import datetime


# ============================================================
# SETTINGS
# ============================================================

HOST = "0.0.0.0"
PORT = 12345

DATA_FOLDER = "analytics_data"

os.makedirs(
    DATA_FOLDER,
    exist_ok=True
)

DATA_FILE = os.path.join(
    DATA_FOLDER,
    "continuous_data.jsonl"
)


# ============================================================
# RECEIVE EXACT NUMBER OF BYTES
# ============================================================

def receive_exact(conn, size):

    data = b""

    while len(data) < size:

        chunk = conn.recv(
            size - len(data)
        )

        if not chunk:
            return None

        data += chunk

    return data


# ============================================================
# RECEIVE ONE MESSAGE
# ============================================================

def receive_message(conn):

    # --------------------------------------------------------
    # First 4 bytes = message length
    # --------------------------------------------------------

    length_bytes = receive_exact(
        conn,
        4
    )

    if length_bytes is None:
        return None


    # --------------------------------------------------------
    # BIG-ENDIAN
    # --------------------------------------------------------

    message_length = struct.unpack(
        "!I",
        length_bytes
    )[0]


    # --------------------------------------------------------
    # Safety check
    # --------------------------------------------------------

    if message_length <= 0:

        print(
            "Invalid message length:",
            message_length
        )

        return None


    if message_length > 10 * 1024 * 1024:

        print(
            "Message too large:",
            message_length
        )

        return None


    # --------------------------------------------------------
    # Receive JSON
    # --------------------------------------------------------

    json_bytes = receive_exact(
        conn,
        message_length
    )

    if json_bytes is None:
        return None


    # --------------------------------------------------------
    # Decode UTF-8
    # --------------------------------------------------------

    json_string = json_bytes.decode(
        "utf-8"
    )


    # --------------------------------------------------------
    # Parse JSON
    # --------------------------------------------------------

    try:

        data = json.loads(
            json_string
        )

        return data

    except json.JSONDecodeError as e:

        print(
            "JSON ERROR:",
            e
        )

        print(
            "Received:",
            json_string
        )

        return None


# ============================================================
# SAVE DATA
# ============================================================

def save_data(data):

    with open(
        DATA_FILE,
        "a",
        encoding="utf-8"
    ) as file:

        file.write(
            json.dumps(
                data,
                ensure_ascii=False,
                separators=(",", ":")
            )
        )

        file.write("\n")


# ============================================================
# HANDLE UNITY CLIENT
# ============================================================

def handle_client(
    conn,
    address
):

    print()
    print(
        "=========================================="
    )

    print(
        "Unity connected!"
    )

    print(
        "Client:",
        address
    )

    print(
        "=========================================="
    )


    message_count = 0


    try:

        while True:

            data = receive_message(
                conn
            )

            if data is None:

                print(
                    "Unity disconnected."
                )

                break


            message_count += 1


            # ------------------------------------------------
            # Add server receive time
            # ------------------------------------------------

            data["serverReceivedTime"] = (
                datetime.now().isoformat()
            )


            # ------------------------------------------------
            # Save
            # ------------------------------------------------

            save_data(
                data
            )


            # ------------------------------------------------
            # Console
            # ------------------------------------------------

            print(
                f"[{message_count}] "
                f"Maze={data.get('mazeNumber')} | "
                f"Attempt={data.get('attemptNumber')} | "

                f"PlayerRot=("
                f"{data.get('playerRotationX', 0):.2f}, "
                f"{data.get('playerRotationY', 0):.2f}, "
                f"{data.get('playerRotationZ', 0):.2f}"
                f") | "

                f"RightGrab="
                f"{data.get('rightGrab', 0):.2f} | "

                f"Head=("
                f"{data.get('headPositionX', 0):.2f}, "
                f"{data.get('headPositionY', 0):.2f}, "
                f"{data.get('headPositionZ', 0):.2f}"
                f")"
            )


    except ConnectionResetError:

        print(
            "Connection reset by Unity."
        )


    except Exception as e:

        print(
            "Client error:",
            e
        )


    finally:

        conn.close()

        print(
            "Connection closed:",
            address
        )


# ============================================================
# START SERVER
# ============================================================

def start_server():

    print()
    print(
        "=========================================="
    )

    print(
        "PYTHON TCP SERVER"
    )

    print(
        "=========================================="
    )

    print(
        f"Listening on {HOST}:{PORT}"
    )

    print(
        "Data file:",
        DATA_FILE
    )

    print(
        "=========================================="
    )


    server = socket.socket(
        socket.AF_INET,
        socket.SOCK_STREAM
    )


    # Prevent "Address already in use"

    server.setsockopt(
        socket.SOL_SOCKET,
        socket.SO_REUSEADDR,
        1
    )


    server.bind(
        (HOST, PORT)
    )


    server.listen(
        1
    )


    print(
        "Waiting for Unity..."
    )


    try:

        while True:

            conn, address = server.accept()

            handle_client(
                conn,
                address
            )

            print()

            print(
                "Waiting for Unity again..."
            )


    except KeyboardInterrupt:

        print(
            "\nServer stopped."
        )


    finally:

        server.close()


# ============================================================
# MAIN
# ============================================================

if __name__ == "__main__":

    start_server()