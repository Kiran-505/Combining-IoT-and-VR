using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using System.Runtime.CompilerServices;

public class RaspberryPiCommunicator : MonoBehaviour
{
	public string raspberryPiIP;
	public string tellStickID;
	private int webSocketPort = 32323;
    
	private int potentiometerValue;
	private int hearRateValue;
	private int forceValue;
	
	public bool lightON = false;
	private bool hiddenUI = true;


    public GameObject[] presents;


	public GameObject potentionCube;
	public Material mat1;
	public Material mat2;
	public Material mat3;

	private WebSocket webSocket;

	public GameObject handUI;


	public float minX, maxX, minZ, maxZ, minY;

	private Vector3 GenerateSpawnPosition() //Generates random position for Presents
    {
        float spawnPosX = Random.Range(minX, maxX);
        float spawnPosZ = Random.Range(minZ, maxZ);
	    Vector3 randomPos = new Vector3(spawnPosX, minY, spawnPosZ);
        
		spawnPresent(randomPos);
	    return randomPos;
        
    }

	public void spawnPresent(Vector3 position) //Spawns a present
    {
		Instantiate(presents[Random.Range(0, presents.Length)], position, Quaternion.identity);
    }


	public void sendTellStickSocket(string tellID) //Sends TellStick WebSocket to Raspberry Pi to turn On or Off actuator
	{
        if (lightON == false)
        {
            SendWebSocketMessage("tdtool --on " + tellID);
            lightON = true;
        }
        else
        {
            SendWebSocketMessage("tdtool --off " + tellID);
            lightON = false;
        }

    }

	public void openHandUI(string socketMessage) //Shows or hides HandUI on Force Sensor press in Arduino and receiving the respective WebSocket
	{
        string[] value = socketMessage.Split("=");
        forceValue = int.Parse(value[1]);
		if (forceValue > 700)
		{
			Debug.Log("STATUS IS: " + handUI.activeSelf);

			if (!hiddenUI)
			{
				handUI.SetActive(true);
			}
			else
			{
				handUI.SetActive(false);
			}

		}
		else
		{
			hiddenUI = !hiddenUI;
		}
    }


	private void initWebSocket() //Starts WebSocket Client Connection
	{
		webSocket = new WebSocket($"ws://{raspberryPiIP}:{webSocketPort}");
		webSocket.Connect();

		webSocket.OnOpen += WebSocket_OnOpen;
		webSocket.OnError += WebSocket_OnError;
		webSocket.OnClose += WebSocket_OnClose;
		webSocket.OnMessage += WebSocket_OnMessage;

	}

	private void WebSocket_OnOpen() //Alerts on console when WebSocket Connection is Successfull
	{
		Debug.Log("Connecion opened!");
	}

	private void WebSocket_OnError(string error) //Alerts on console when WebSocket Connection is Unsuccessfull
	{
		Debug.Log($"Error: {error}");
	}

	private void WebSocket_OnClose(WebSocketCloseCode closeCode) //Alerts on console when WebSocket Connection is Closed
	{
		Debug.Log("Connection closed!");
	}

	private void WebSocket_OnMessage(byte[] data) //Receives webSocket message and handles it respectively depending on what it contains
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
		
		/*if(socketMessage.Contains("Heart"))
		{
			string[] value = socketMessage.Split("=");
			hearRateValue = int.Parse(value[1]);
		    
		}*/
		
		if(socketMessage.Contains("Force"))
		{
			openHandUI(socketMessage);
		    
		}
		
		if(socketMessage.Contains("Button"))
		{
			Debug.Log(socketMessage);
			GenerateSpawnPosition();
			//spawnPresent();
			//sendTellStickSocket(tellStickID);

			
		}
		
		
		//Debug.Log("Potentiometer Value = " + potentiometerValue);
		//Debug.Log("Heart Rate Value = " + hearRateValue);
		//Debug.Log("Force Value = " + forceValue);
        
	}
	
	
		
	async void SendWebSocketMessage(string text) //Sends Websocket Message to Raspberry Pi
	{
		if (webSocket.State == WebSocketState.Open)
		{
			// Sending plain text socket
			await webSocket.SendText(text);
		}
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
	

	private async void OnApplicationQuit() //Closes Websocket Connection Correctly when app is closed
	{
		await webSocket.Close();
	}
	
}
