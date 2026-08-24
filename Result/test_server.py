import socket
import threading

HOST = "0.0.0.0"

def server(port, name):
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

    s.bind((HOST, port))
    s.listen(5)

    print(f"{name} listening on {HOST}:{port}")
    print("Waiting for connection...")

    while True:
        conn, addr = s.accept()

        print(f"{name} CONNECTED: {addr}")

        try:
            while True:
                data = conn.recv(4096)

                if not data:
                    break

                print(f"{name} received {len(data)} bytes")

        except Exception as e:
            print(f"{name} error:", e)

        finally:
            conn.close()
            print(f"{name} disconnected")


threading.Thread(
    target=server,
    args=(12345, "ANALYTICS"),
    daemon=True
).start()

threading.Thread(
    target=server,
    args=(12346, "SETTINGS"),
    daemon=True
).start()

print("===================================")
print("TCP TEST SERVER STARTED")
print("Analytics : 12345")
print("Settings  : 12346")
print("===================================")

input("Press ENTER to stop...\n")