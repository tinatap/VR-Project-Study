import socket
import struct
import json
import csv
import os
from datetime import datetime


HOST = "0.0.0.0"
PORT = 12345

OUTPUT_FOLDER = "MazeData"

os.makedirs(
    OUTPUT_FOLDER,
    exist_ok=True
)


# =====================================================
# CREATE FILE
# =====================================================

timestamp = datetime.now().strftime(
    "%Y%m%d_%H%M%S"
)

csv_file_path = os.path.join(
    OUTPUT_FOLDER,
    f"MazeData_{timestamp}.csv"
)


# =====================================================
# CSV COLUMNS
# =====================================================

FIELDS = [
    "timestamp",
    "eventType",

    "mazeNumber",
    "attemptNumber",

    "collectedCoins",
    "totalCoins",
    "totalScore",

    "mazeElapsedTime",
    "totalGameElapsedTime",

    "headPositionX",
    "headPositionY",
    "headPositionZ",

    "headRotationX",
    "headRotationY",
    "headRotationZ",

    "playerPositionX",
    "playerPositionY",
    "playerPositionZ",

    "playerRotationY",

    "rightThumbstickX",
    "rightThumbstickY",

    "rightTrigger",

    "leftThumbstickX",
    "leftThumbstickY",

    "leftTrigger"
]


# =====================================================
# RECEIVE EXACT NUMBER OF BYTES
# =====================================================

def recv_exact(sock, size):

    data = b""

    while len(data) < size:

        chunk = sock.recv(
            size - len(data)
        )

        if not chunk:
            return None

        data += chunk

    return data


# =====================================================
# START SERVER
# =====================================================

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

server.listen(1)


print("======================================")
print("Maze TCP Receiver")
print(f"Listening on port {PORT}")
print(f"Saving to: {csv_file_path}")
print("Waiting for Quest...")
print("======================================")


# =====================================================
# OPEN CSV
# =====================================================

with open(
    csv_file_path,
    "w",
    newline="",
    encoding="utf-8-sig"
) as csv_file:

    writer = csv.DictWriter(
        csv_file,
        fieldnames=FIELDS
    )

    writer.writeheader()


    # =================================================
    # WAIT FOR QUEST
    # =================================================

    while True:

        client, address = server.accept()

        print(
            f"Quest connected from {address}"
        )

        try:

            while True:

                # =====================================
                # READ LENGTH
                # =====================================

                length_data = recv_exact(
                    client,
                    4
                )

                if length_data is None:

                    print(
                        "Quest disconnected."
                    )

                    break


                message_length = struct.unpack(
                    ">I",
                    length_data
                )[0]


                # =====================================
                # SAFETY CHECK
                # =====================================

                if message_length <= 0:

                    print(
                        "Invalid message length:",
                        message_length
                    )

                    continue


                if message_length > 10 * 1024 * 1024:

                    print(
                        "Message too large:",
                        message_length
                    )

                    break


                # =====================================
                # READ JSON
                # =====================================

                json_data = recv_exact(
                    client,
                    message_length
                )

                if json_data is None:

                    print(
                        "Connection closed."
                    )

                    break


                # =====================================
                # DECODE JSON
                # =====================================

                try:

                    text = json_data.decode(
                        "utf-8"
                    )

                    data = json.loads(
                        text
                    )

                except Exception as e:

                    print(
                        "JSON error:",
                        e
                    )

                    print(
                        "Raw data:",
                        repr(json_data[:200])
                    )

                    continue


                # =====================================
                # PRINT RECEIVED DATA
                # =====================================

                print(
                    "RECEIVED -> "
                    "Maze:",
                    data.get("mazeNumber"),
                    "| Attempt:",
                    data.get("attemptNumber"),
                    "| Coins:",
                    data.get("collectedCoins"),
                    "| Score:",
                    data.get("totalScore"),
                    "| Event:",
                    data.get("eventType")
                )


                # =====================================
                # SAVE CSV
                # =====================================

                row = {}

                for field in FIELDS:

                    row[field] = data.get(
                        field,
                        ""
                    )

                writer.writerow(row)

                csv_file.flush()


                # =====================================
                # IMPORTANT EVENTS
                # =====================================

                event = data.get(
                    "eventType",
                    ""
                )

                if event != "DATA":

                    print(
                        "EVENT:",
                        event,
                        "| Maze:",
                        data.get(
                            "mazeNumber"
                        ),
                        "| Attempt:",
                        data.get(
                            "attemptNumber"
                        )
                    )


        except Exception as e:

            print(
                "Connection error:",
                e
            )

        finally:

            client.close()