import socket
import struct
import json
import os
import threading
import csv
import re
from datetime import datetime


# ============================================================
# PARTICIPANT INFORMATION
# ============================================================

print()
print("==========================================")
print("PARTICIPANT INFORMATION")
print("==========================================")

PARTICIPANT_NAME = input("Participant Name: ").strip()
PARTICIPANT_ID = input("Participant ID: ").strip()

if not PARTICIPANT_NAME:
    PARTICIPANT_NAME = "Unknown"

if not PARTICIPANT_ID:
    PARTICIPANT_ID = "Unknown"

print()
print("Participant Name:", PARTICIPANT_NAME)
print("Participant ID:", PARTICIPANT_ID)


# ============================================================
# SAFE FILE / FOLDER NAME
# ============================================================

def make_safe_name(name):

    # Replace characters that are not allowed in Windows filenames
    name = re.sub(r'[<>:"/\\|?*]', '_', name)

    # Replace multiple spaces with one underscore
    name = re.sub(r'\s+', '_', name)

    # Remove unnecessary dots/spaces from beginning/end
    name = name.strip(" ._")

    if not name:
        name = "Unknown"

    return name


SAFE_NAME = make_safe_name(PARTICIPANT_NAME)
SAFE_ID = make_safe_name(PARTICIPANT_ID)

PARTICIPANT_FOLDER_NAME = f"{SAFE_NAME}_{SAFE_ID}"


# ============================================================
# SETTINGS
# ============================================================

HOST = "0.0.0.0"
PORT = 12345

# Main analytics folder
DATA_FOLDER = "analytics_data"

os.makedirs(DATA_FOLDER, exist_ok=True)


# ============================================================
# PARTICIPANT-SPECIFIC FOLDER
# ============================================================

PARTICIPANT_FOLDER = os.path.join(
    DATA_FOLDER,
    PARTICIPANT_FOLDER_NAME
)

os.makedirs(
    PARTICIPANT_FOLDER,
    exist_ok=True
)


# ============================================================
# PARTICIPANT-SPECIFIC FILES
# ============================================================

DATA_FILE = os.path.join(
    PARTICIPANT_FOLDER,
    f"continuous_data_{SAFE_NAME}_{SAFE_ID}.jsonl"
)

CSV_FILE = os.path.join(
    PARTICIPANT_FOLDER,
    f"continuous_data_{SAFE_NAME}_{SAFE_ID}.csv"
)

SUMMARY_FILE = os.path.join(
    PARTICIPANT_FOLDER,
    f"experiment_summary_{SAFE_NAME}_{SAFE_ID}.json"
)


# ============================================================
# DISPLAY STORAGE INFORMATION
# ============================================================

print()
print("==========================================")
print("PARTICIPANT STORAGE")
print("==========================================")

print(
    "Participant folder:",
    PARTICIPANT_FOLDER
)

print(
    "Continuous JSON:",
    DATA_FILE
)

print(
    "Continuous CSV:",
    CSV_FILE
)

print(
    "Experiment Summary:",
    SUMMARY_FILE
)

print("==========================================")
print()


# ============================================================
# CSV VARIABLES
# ============================================================

csv_lock = threading.Lock()

csv_file = None
csv_writer = None

csv_headers = []

CSV_FLUSH_EVERY_ROW = True


# ============================================================
# CREATE CSV
# ============================================================

print()
print("Creating CSV file...")

try:

    csv_exists = os.path.isfile(CSV_FILE)

    csv_file = open(
        CSV_FILE,
        "a+",
        newline="",
        encoding="utf-8-sig"
    )

    print(
        "CSV file ready:"
    )

    print(
        CSV_FILE
    )

except Exception as e:

    print(
        "!!! CSV CREATION ERROR !!!"
    )

    print(
        repr(e)
    )

    raise


# ============================================================
# LOAD EXISTING CSV HEADERS
# ============================================================

def load_csv_headers():

    global csv_headers
    global csv_writer

    try:

        if os.path.isfile(CSV_FILE):

            csv_file.seek(0)

            first_line = csv_file.readline()

            if first_line.strip():

                csv_headers = next(
                    csv.reader(
                        [first_line]
                    )
                )

                csv_file.seek(
                    0,
                    os.SEEK_END
                )

                csv_writer = csv.DictWriter(
                    csv_file,
                    fieldnames=csv_headers,
                    extrasaction="ignore"
                )

                print(
                    "Existing CSV headers loaded:"
                )

                print(
                    csv_headers
                )

    except Exception as e:

        print(
            "!!! CSV HEADER LOAD ERROR !!!"
        )

        print(
            repr(e)
        )


load_csv_headers()


# ============================================================
# CONVERT VALUE FOR CSV
# ============================================================

def convert_value_for_csv(value):

    if value is None:

        return ""

    if isinstance(value, bool):

        return value

    if isinstance(
        value,
        (int, float)
    ):

        return value

    if isinstance(
        value,
        (list, dict)
    ):

        return json.dumps(
            value,
            ensure_ascii=False,
            separators=(",", ":")
        )

    return value


# ============================================================
# REWRITE CSV WITH NEW COLUMNS
# ============================================================

def rewrite_csv_with_new_columns(data):

    global csv_headers
    global csv_writer
    global csv_file

    # --------------------------------------------------------
    # Find new columns
    # --------------------------------------------------------

    new_keys = [
        key
        for key in data.keys()
        if key not in csv_headers
    ]

    if not new_keys:

        return False

    # --------------------------------------------------------
    # Add new columns
    # --------------------------------------------------------

    csv_headers.extend(
        new_keys
    )

    print()
    print(
        "New CSV columns detected:"
    )

    print(
        new_keys
    )

    # --------------------------------------------------------
    # Read existing CSV
    # --------------------------------------------------------

    existing_rows = []

    try:

        csv_file.flush()

        with open(
            CSV_FILE,
            "r",
            newline="",
            encoding="utf-8-sig"
        ) as old_file:

            reader = csv.DictReader(
                old_file
            )

            for row in reader:

                existing_rows.append(
                    row
                )

    except Exception as e:

        print(
            "!!! CSV READ ERROR !!!"
        )

        print(
            repr(e)
        )

    # --------------------------------------------------------
    # Close current file
    # --------------------------------------------------------

    try:

        csv_file.close()

    except:

        pass

    # --------------------------------------------------------
    # Rewrite entire CSV
    # --------------------------------------------------------

    try:

        csv_file = open(
            CSV_FILE,
            "w",
            newline="",
            encoding="utf-8-sig"
        )

        csv_writer = csv.DictWriter(
            csv_file,
            fieldnames=csv_headers
        )

        csv_writer.writeheader()

        # ----------------------------------------------------
        # Restore previous rows
        # ----------------------------------------------------

        for row in existing_rows:

            csv_writer.writerow(
                row
            )

        csv_file.flush()

        print(
            "CSV structure updated successfully."
        )

        return True

    except Exception as e:

        print(
            "!!! CSV REWRITE ERROR !!!"
        )

        print(
            repr(e)
        )

        return False


# ============================================================
# ADD DATA TO CSV
# ============================================================

def add_to_csv(data):

    global csv_headers
    global csv_writer

    with csv_lock:

        try:

            # ------------------------------------------------
            # Create headers if necessary
            # ------------------------------------------------

            if not csv_headers:

                csv_headers = list(
                    data.keys()
                )

                csv_writer = csv.DictWriter(
                    csv_file,
                    fieldnames=csv_headers
                )

                csv_writer.writeheader()

                print()
                print(
                    "CSV headers created:"
                )

                print(
                    csv_headers
                )

            else:

                # --------------------------------------------
                # Check for new fields
                # --------------------------------------------

                rewrite_csv_with_new_columns(
                    data
                )

            # ------------------------------------------------
            # Prepare row
            # ------------------------------------------------

            row = {}

            for key in csv_headers:

                if key in data:

                    row[key] = convert_value_for_csv(
                        data[key]
                    )

                else:

                    row[key] = ""

            # ------------------------------------------------
            # Write row
            # ------------------------------------------------

            csv_writer.writerow(
                row
            )

            # ------------------------------------------------
            # Immediate save
            # ------------------------------------------------

            if CSV_FLUSH_EVERY_ROW:

                csv_file.flush()

        except Exception as e:

            print()
            print(
                "!!! CSV DATA ERROR !!!"
            )

            print(
                repr(e)
            )


# ============================================================
# REMOVE UNWANTED FIELDS
# ============================================================

def remove_unwanted_fields(data):

    # --------------------------------------------------------
    # Remove Start Question Panel Duration
    # from top-level data
    # --------------------------------------------------------

    data.pop(
        "startQuestionPanelDuration",
        None
    )

    # --------------------------------------------------------
    # Remove it from mazeVisits
    # --------------------------------------------------------

    maze_visits = data.get(
        "mazeVisits"
    )

    if isinstance(
        maze_visits,
        list
    ):

        for visit in maze_visits:

            if isinstance(
                visit,
                dict
            ):

                visit.pop(
                    "startQuestionPanelDuration",
                    None
                )

    return data


# ============================================================
# ADD PARTICIPANT INFORMATION
# ============================================================

def add_participant_information(data):

    data["participantName"] = PARTICIPANT_NAME
    data["participantID"] = PARTICIPANT_ID

    return data


# ============================================================
# RECEIVE EXACT BYTES
# ============================================================

def receive_exact(
    conn,
    size
):

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
# RECEIVE MESSAGE
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
    # Big Endian unsigned integer
    # --------------------------------------------------------

    message_length = struct.unpack(
        "!I",
        length_bytes
    )[0]

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
    # Receive JSON bytes
    # --------------------------------------------------------

    json_bytes = receive_exact(
        conn,
        message_length
    )

    if json_bytes is None:

        return None

    try:

        json_string = json_bytes.decode(
            "utf-8"
        )

        data = json.loads(
            json_string
        )

        return data

    except Exception as e:

        print(
            "JSON ERROR:",
            repr(e)
        )

        return None


# ============================================================
# SAVE CONTINUOUS JSON
# ============================================================

def save_continuous_json(data):

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

        file.write(
            "\n"
        )


# ============================================================
# SAVE EXPERIMENT SUMMARY
# ============================================================

def save_experiment_summary(data):

    try:

        # ----------------------------------------------------
        # Remove unwanted field completely
        # ----------------------------------------------------

        remove_unwanted_fields(
            data
        )

        # ----------------------------------------------------
        # Add participant information
        # ----------------------------------------------------

        add_participant_information(
            data
        )

        # ----------------------------------------------------
        # Add server time
        # ----------------------------------------------------

        data["serverReceivedTime"] = (
            datetime.now().isoformat()
        )

        # ----------------------------------------------------
        # Save formatted JSON
        # ----------------------------------------------------

        with open(
            SUMMARY_FILE,
            "w",
            encoding="utf-8"
        ) as file:

            json.dump(
                data,
                file,
                ensure_ascii=False,
                indent=4
            )

        # ----------------------------------------------------
        # Console information
        # ----------------------------------------------------

        maze_visits = data.get(
            "mazeVisits",
            []
        )

        print()
        print(
            "=========================================="
        )

        print(
            "EXPERIMENT SUMMARY RECEIVED"
        )

        print(
            "=========================================="
        )

        print(
            "Participant Name:",
            PARTICIPANT_NAME
        )

        print(
            "Participant ID:",
            PARTICIPANT_ID
        )

        print(
            "Final Result:",
            data.get("finalResult")
        )

        print(
            "Total Game Time:",
            data.get("totalGameTime")
        )

        print(
            "Final Score:",
            data.get("finalScore")
        )

        print(
            "Highest Completed Maze:",
            data.get("highestCompletedMaze")
        )

        print(
            "Start Room Duration:",
            data.get("startRoomDuration")
        )

        print(
            "Maze Visits:",
            len(maze_visits)
        )

        print(
            "Summary file:",
            SUMMARY_FILE
        )

        print(
            "=========================================="
        )

        # ----------------------------------------------------
        # Print each maze visit
        # ----------------------------------------------------

        for visit in maze_visits:

            print(
                "Visit #{visit} | "
                "Maze={maze} | "
                "Attempt={attempt} | "
                "Coins={coins}/{totalCoins} | "
                "Duration={duration:.3f}s | "
                "Result={result} | "
                "End={end}".format(

                    visit=visit.get(
                        "visitNumber"
                    ),

                    maze=visit.get(
                        "mazeNumber"
                    ),

                    attempt=visit.get(
                        "attemptNumber"
                    ),

                    coins=visit.get(
                        "collectedCoins",
                        0
                    ),

                    totalCoins=visit.get(
                        "totalCoins",
                        0
                    ),

                    duration=float(
                        visit.get(
                            "durationSeconds",
                            0
                        )
                    ),

                    result=visit.get(
                        "result"
                    ),

                    end=visit.get(
                        "endReason"
                    )
                )
            )

        print(
            "=========================================="
        )

    except Exception as e:

        print()
        print(
            "!!! SUMMARY SAVE ERROR !!!"
        )

        print(
            repr(e)
        )


# ============================================================
# HANDLE CONTINUOUS DATA
# ============================================================

def handle_continuous_data(data):

    # --------------------------------------------------------
    # Remove unwanted field completely
    # --------------------------------------------------------

    remove_unwanted_fields(
        data
    )

    # --------------------------------------------------------
    # Add participant information
    # --------------------------------------------------------

    add_participant_information(
        data
    )

    # --------------------------------------------------------
    # Add server reception time
    # --------------------------------------------------------

    data["serverReceivedTime"] = (
        datetime.now().isoformat()
    )

    # --------------------------------------------------------
    # Save JSONL
    # --------------------------------------------------------

    save_continuous_json(
        data
    )

    # --------------------------------------------------------
    # Save CSV immediately
    # --------------------------------------------------------

    add_to_csv(
        data
    )


# ============================================================
# HANDLE CLIENT
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
        "Participant:",
        PARTICIPANT_NAME,
        "| ID:",
        PARTICIPANT_ID
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

            # =================================================
            # IDENTIFY MESSAGE TYPE
            # =================================================

            record_type = data.get(
                "recordType",
                ""
            )

            message_type = data.get(
                "messageType",
                ""
            )

            # =================================================
            # EXPERIMENT SUMMARY
            # =================================================

            if (
                record_type ==
                "EXPERIMENT_SUMMARY"
                or
                message_type ==
                "EXPERIMENT_SUMMARY"
            ):

                save_experiment_summary(
                    data
                )

                continue

            # =================================================
            # CONTINUOUS DATA / EVENT
            # =================================================

            handle_continuous_data(
                data
            )

            # =================================================
            # CONSOLE
            # =================================================

            print(
                f"[{message_count}] "
                f"Type={record_type} | "
                f"Maze={data.get('mazeNumber')} | "
                f"Attempt={data.get('attemptNumber')}"
            )

    except ConnectionResetError:

        print(
            "Connection reset by Unity."
        )

    except Exception as e:

        print(
            "CLIENT ERROR:"
        )

        print(
            repr(e)
        )

    finally:

        # ----------------------------------------------------
        # Final CSV flush
        # ----------------------------------------------------

        with csv_lock:

            try:

                csv_file.flush()

                print(
                    "CSV final flush completed."
                )

            except Exception as e:

                print(
                    "!!! FINAL CSV FLUSH ERROR !!!"
                )

                print(
                    repr(e)
                )

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

    print()

    print(
        "Participant Name:",
        PARTICIPANT_NAME
    )

    print(
        "Participant ID:",
        PARTICIPANT_ID
    )

    print()

    print(
        "Participant Folder:",
        PARTICIPANT_FOLDER
    )

    print()

    print(
        "CONTINUOUS JSON:",
        DATA_FILE
    )

    print(
        "CONTINUOUS CSV:",
        CSV_FILE
    )

    print()

    print(
        "EXPERIMENT SUMMARY:",
        SUMMARY_FILE
    )

    print(
        "=========================================="
    )

    # ========================================================
    # CREATE SERVER
    # ========================================================

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
        (
            HOST,
            PORT
        )
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

        print(
            "Final CSV flush..."
        )

        with csv_lock:

            try:

                csv_file.flush()

                print(
                    "Final CSV save completed."
                )

            except Exception as e:

                print(
                    "!!! FINAL CSV SAVE ERROR !!!"
                )

                print(
                    repr(e)
                )

            try:

                csv_file.close()

            except:

                pass

        try:

            server.close()

        except:

            pass


# ============================================================
# MAIN
# ============================================================

if __name__ == "__main__":

    start_server()