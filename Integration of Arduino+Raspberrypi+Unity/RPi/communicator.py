#import websockets
import threading
from websocket_server import WebsocketServer
import json
import serial

def on_message(client, server, message):
    print("Client(%d) said: %s" % (client['id'], message))

def new_client(client, server):
    print("New client connected with id %d" % client['id'])

def listen_to_arduino(server):
    arduino = serial.Serial('/dev/serial/by-id/usb-Arduino__www.arduino.cc__0043_95730333937351201141-if00', 9600, timeout=.1)
    while True:
        data = arduino.readline()
        if data:
            print(data)
            server.send_message_to_all(data)

def create_websocket():
    server = WebsocketServer(host="0.0.0.0", port=32323)
    server.set_fn_new_client(new_client)
    server.set_fn_message_received(on_message)
    thread = threading.Thread(target=server.run_forever)
    thread.daemon = True
    thread.start()
    listen_to_arduino(server)

def main():
    create_websocket()

main()
