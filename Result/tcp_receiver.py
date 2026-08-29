import socket
import json
import os
from datetime import datetime


# ============================================================
# SERVER SETTINGS
# ============================================================

HOST = "0.0.0.0"
PORT = 12345

# پوشه ذخیره اطلاعات
DATA_FOLDER = "analytics_data"

os.makedirs(DATA_FOLDER, exist_ok=True)


# ============================================================
# FILE PATHS
# ============================================================

CONTINUOUS_FILE = os.path.join(
    DATA_FOLDER,
    "continuous_data.jsonl"
)

EVENT_FILE = os.path.join(
    DATA_FOLDER,
    "events.jsonl"
)

MAZE_VISIT_FILE = os.path.join(
    DATA_FOLDER,
    "maze_visits.jsonl"
)

EXIT_CONFIRM_FILE = os.path.join(
    DATA_FOLDER,
    "exit_confirm.jsonl"
)

START_ROOM_FILE = os.path.join(
    DATA_FOLDER,
    "start_room.jsonl"
)

FINAL_RESULT_FILE = os.path.join(
    DATA_FOLDER,
    "final_result.jsonl"
)


# ============================================================
# SAVE JSON
# ============================================================

def save_json_line(filename, data):
    """
    هر پیام را به صورت یک JSON جداگانه در فایل ذخیره می‌کند.
    """

    with open(
        filename,
        "a",
        encoding="utf-8"
    ) as file:

        json.dump(
            data,
            file,
            ensure_ascii=False
        )

        file.write("\n")


# ============================================================
# PROCESS MESSAGE
# ============================================================

def process_message(data):
    """
    پیام دریافت شده از Unity را بر اساس messageType
    در فایل مناسب ذخیره می‌کند.
    """

    if not isinstance(data, dict):
        print("Invalid message format.")
        return


    message_type = data.get(
        "messageType",
        "UNKNOWN"
    )


    # ========================================================
    # CONTINUOUS DATA
    # ========================================================

    if message_type == "CONTINUOUS_DATA":

        save_json_line(
            CONTINUOUS_FILE,
            data
        )

        print(
            "[CONTINUOUS_DATA] "
            f"Maze={data.get('mazeNumber')} "
            f"Attempt={data.get('attemptNumber')}"
        )

        return


    # ========================================================
    # EVENT
    # ========================================================

    if message_type == "EVENT":

        save_json_line(
            EVENT_FILE,
            data
        )

        print(
            "[EVENT] "
            f"{data.get('eventType')}"
        )

        return


    # ========================================================
    # MAZE VISIT
    # ========================================================

    if message_type == "MAZE_VISIT":

        save_json_line(
            MAZE_VISIT_FILE,
            data
        )

        print(
            "[MAZE_VISIT] "
            f"Visit={data.get('visitNumber')} "
            f"Maze={data.get('mazeNumber')} "
            f"Attempt={data.get('attemptNumber')} "
            f"Result={data.get('result')} "
            f"Duration={data.get('durationSeconds')}"
        )

        return


    # ========================================================
    # EXIT CONFIRM
    # ========================================================

    if message_type == "EXIT_CONFIRM":

        save_json_line(
            EXIT_CONFIRM_FILE,
            data
        )

        print(
            "[EXIT_CONFIRM] "
            f"Interaction={data.get('interactionNumber')} "
            f"Maze={data.get('mazeNumber')} "
            f"Attempt={data.get('attemptNumber')} "
            f"Result={data.get('result')} "
            f"Duration={data.get('durationSeconds')}"
        )

        return


    # ========================================================
    # START ROOM
    # ========================================================

    if message_type == "START_ROOM":

        save_json_line(
            START_ROOM_FILE,
            data
        )

        print(
            "[START_ROOM] "
            f"RoomDuration={data.get('startRoomDuration')} "
            f"QuestionDuration={data.get('startQuestionPanelDuration')}"
        )

        return


    # ========================================================
    # FINAL RESULT
    # ========================================================

    if message_type == "FINAL_RESULT":

        save_json_line(
            FINAL_RESULT_FILE,
            data
        )

        print(
            "[FINAL_RESULT] "
            f"Result={data.get('result')} "
            f"TotalTime={data.get('totalGameTime')} "
            f"FinalScore={data.get('finalScore')}"
        )

        return


    # ========================================================
    # UNKNOWN
    # ========================================================

    print(
        "[UNKNOWN MESSAGE TYPE]",
        message_type
    )


# ============================================================
# HANDLE CLIENT
# ============================================================

def handle_client(conn, address):

    print()
    print("==================================================")
    print("Unity client connected")
    print("Address:", address)
    print("==================================================")

    # --------------------------------------------------------
    # TCP buffer
    # --------------------------------------------------------

    buffer = ""

    try:

        while True:

            # ------------------------------------------------
            # Receive bytes
            # ------------------------------------------------

            received = conn.recv(65536)

            if not received:
                print(
                    "Unity client disconnected."
                )

                break


            # ------------------------------------------------
            # Decode
            # ------------------------------------------------

            buffer += received.decode(
                "utf-8"
            )


            # ------------------------------------------------
            # Process complete messages
            #
            # Unity sends:
            #
            # { ... }\n
            #
            # ------------------------------------------------

            while "\n" in buffer:

                line, buffer = buffer.split(
                    "\n",
                    1
                )

                line = line.strip()

                if not line:
                    continue


                # ------------------------------------------------
                # Parse JSON
                # ------------------------------------------------

                try:

                    data = json.loads(
                        line
                    )

                except json.JSONDecodeError as error:

                    print(
                        "JSON decode error:"
                    )

                    print(error)

                    print(
                        "Received data:",
                        line
                    )

                    continue


                # ------------------------------------------------
                # Process
                # ------------------------------------------------

                process_message(
                    data
                )


    except ConnectionResetError:

        print(
            "Unity connection was reset."
        )

    except Exception as error:

        print(
            "Client error:",
            error
        )

    finally:

        try:
            conn.close()
        except:
            pass

        print(
            "Connection closed:",
            address
        )


# ============================================================
# START SERVER
# ============================================================

def start_server():

    print()
    print("==================================================")
    print("      UNITY TCP ANALYTICS SERVER")
    print("==================================================")

    print(
        "Listening on:",
        HOST,
        PORT
    )

    print(
        "Data folder:",
        DATA_FOLDER
    )

    print("==================================================")
    print()


    # --------------------------------------------------------
    # Create TCP socket
    # --------------------------------------------------------

    server = socket.socket(
        socket.AF_INET,
        socket.SOCK_STREAM
    )


    # --------------------------------------------------------
    # Allow address reuse
    # --------------------------------------------------------

    server.setsockopt(
        socket.SOL_SOCKET,
        socket.SO_REUSEADDR,
        1
    )


    # --------------------------------------------------------
    # Bind
    # --------------------------------------------------------

    server.bind(
        (HOST, PORT)
    )


    # --------------------------------------------------------
    # Listen
    # --------------------------------------------------------

    server.listen(5)


    print(
        "Server started successfully."
    )

    print(
        "Waiting for Unity connection..."
    )

    print()


    try:

        while True:

            conn, address = server.accept()

            handle_client(
                conn,
                address
            )


    except KeyboardInterrupt:

        print()
        print(
            "Server stopped."
        )


    finally:

        server.close()


# ============================================================
# MAIN
# ============================================================

if __name__ == "__main__":

    start_server()