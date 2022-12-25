using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;


public class RaspberryPiCommunicator : MonoBehaviour
{
	public string raspberryPiIP = "192.168.10.244";
	private int webSocketPort = 32323;
    
	private int potentiometerValue;
	private int hearRateValue;
	private int forceValue;


	public GameObject cube;
	public GameObject potentionCube;
	public Material mat1;
	public Material mat2;
	public Material mat3;

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
		string socketMessage = System.Text.Encoding.UTF8.GetString(data);
	    //Debug.Log(System.Text.Encoding.UTF8.GetString(data));
        
	    if(socketMessage.Contains("Potentiometer"))
	    {
		    string[] value = socketMessage.Split("=");
		    potentiometerValue = int.Parse(value[1]);
		    if(potentiometerValue >= 0 && potentiometerValue <= 350)
		    {
		    	potentionCube.GetComponent<Renderer>().material = mat1;
		    }
		    else if (potentiometerValue > 350 && potentiometerValue <= 750)
		    {
		    	potentionCube.GetComponent<Renderer>().material = mat2;
		    }
		    else if (potentiometerValue > 751)
		    {
		    	potentionCube.GetComponent<Renderer>().material = mat3;
		    }

		    
	    }
		
		if(socketMessage.Contains("Heart"))
		{
			string[] value = socketMessage.Split("=");
			hearRateValue = int.Parse(value[1]);
		    
		}
		
		if(socketMessage.Contains("Force"))
		{
			string[] value = socketMessage.Split("=");
			forceValue = int.Parse(value[1]);
			if(forceValue > 900) 
			{
			
				Instantiate(cube);
			}
		    
		}
		
		if(socketMessage.Contains("Button"))
		{
			Debug.Log(socketMessage);
			Instantiate(cube);
			
		}
		
		
		Debug.Log("Potentiometer Value = " + potentiometerValue);
		//Debug.Log("Heart Rate Value = " + hearRateValue);
		Debug.Log("Force Value Value = " + forceValue);
        
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
