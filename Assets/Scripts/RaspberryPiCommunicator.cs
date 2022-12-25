using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;


public class RaspberryPiCommunicator : MonoBehaviour
{
	public string raspberryPiIP = "192.168.10.244";
    private int webSocketPort = 32323;

    private WebSocket webSocket;

    private void initWebSocket()
    {
        webSocket = new WebSocket($"ws://{raspberryPiIP}:{webSocketPort}");
        webSocket.Connect();

        webSocket.OnOpen += WebSocket_OnOpen;
        webSocket.OnError += WebSocket_OnError;
        webSocket.OnClose += WebSocket_OnClose;
        webSocket.OnMessage += WebSocket_OnMessage;

    }

    private void WebSocket_OnOpen()
    {
        Debug.Log("Connecion opened!");
    }

    private void WebSocket_OnError(string error)
    {
        Debug.Log($"Error: {error}");
    }

    private void WebSocket_OnClose(WebSocketCloseCode closeCode)
    {
        Debug.Log("Connection closed!");
    }

    private void WebSocket_OnMessage(byte[] data)
    {
        Debug.Log(System.Text.Encoding.UTF8.GetString(data));
    }

    // Start is called before the first frame update
    void Start()
    {
        initWebSocket();
        Debug.Log("Started");
    }

    // Update is called once per frame
    void Update()
    {
        webSocket.DispatchMessageQueue();
    }

    private async void OnApplicationQuit()
    {
        await webSocket.Close();
    }
}
