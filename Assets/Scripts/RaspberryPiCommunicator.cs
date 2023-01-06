using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using System.Runtime.CompilerServices;

public class RaspberryPiCommunicator : MonoBehaviour
{
	public bool showDebug; //Turn this on to show Sensor values
	public string raspberryPiIP;
	public string tellStickID;
	private int webSocketPort = 32323;
    
	private int potentiometerValue;
	private int oldPotentiometerValue = 0;
	private int hearRateValue;
	private int forceValue;
	
	[HideInInspector]
	public bool lightON = false;
	private bool hiddenUI = true;
	
	public float lightIntensityMultiplier = 1f;
	
	private List<float> baseLightsIntensity = new List<float>();
	private List<float> midLightsIntensity = new List<float>();
	private List<float> topLightsIntensity = new List<float>();
	private List<float> starLightsIntensity = new List<float>();
	
	private Component[] baseLightsArray;
	private Component[] midLightsArray;
	private Component[] topLightsArray;
	private Component[] starLightsArray;





    public GameObject[] presents;


	public GameObject treeLightsBase;
	public GameObject treeLightsMid;
	public GameObject treeLightsTop;
	public GameObject treeLightsStar;

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
	
	
	private void getXmasLightsValues() //Saves all the Xmas Tree Lights Values in Lists as a reference to when they are changed by the potentiometer values
	{
		baseLightsArray = treeLightsBase.GetComponentsInChildren<Light>();
		midLightsArray = treeLightsMid.GetComponentsInChildren<Light>();
		topLightsArray = treeLightsTop.GetComponentsInChildren<Light>();
		starLightsArray = treeLightsStar.GetComponentsInChildren<Light>();

		for(int i = 0; i < baseLightsArray.Length; i++)
		{
			baseLightsIntensity.Add(baseLightsArray[i].GetComponent<Light>().intensity);
		}
		
		for(int i = 0; i < midLightsArray.Length; i++)
		{
			midLightsIntensity.Add(midLightsArray[i].GetComponent<Light>().intensity);
		}
		
		for(int i = 0; i < topLightsArray.Length; i++)
		{
			topLightsIntensity.Add(topLightsArray[i].GetComponent<Light>().intensity);
		}
		
		for(int i = 0; i < starLightsArray.Length; i++)
		{
			starLightsIntensity.Add(starLightsArray[i].GetComponent<Light>().intensity);
		}
	}
	
    
	private void changeXmasLights(string socketMessage) //Changes the Christmas Tree lights depending on Potentiometer Socket Value
	{
		
		float percentage = potentiometerValue/1023f;

		for(int i = 0; i < baseLightsIntensity.Count; i++)
		{
			float newIntensity = baseLightsIntensity[i] * percentage;
			baseLightsArray[i].GetComponent<Light>().intensity = newIntensity * lightIntensityMultiplier;
		}
		
		for(int i = 0; i < midLightsIntensity.Count; i++)
		{
			float newIntensity = midLightsIntensity[i] * percentage;
			midLightsArray[i].GetComponent<Light>().intensity = newIntensity * lightIntensityMultiplier;
		}
		
		for(int i = 0; i < topLightsIntensity.Count; i++)
		{
			float newIntensity = topLightsIntensity[i] * percentage;
			topLightsArray[i].GetComponent<Light>().intensity = newIntensity * lightIntensityMultiplier;
		}
		
		for(int i = 0; i < starLightsIntensity.Count; i++)
		{
			float newIntensity = starLightsIntensity[i] * percentage;
			starLightsArray[i].GetComponent<Light>().intensity = newIntensity * lightIntensityMultiplier;
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
			
			if(oldPotentiometerValue != potentiometerValue){
				changeXmasLights(socketMessage);
				oldPotentiometerValue = potentiometerValue;
			}
		    
		}
		
		
		if(socketMessage.Contains("Force"))
		{
			openHandUI(socketMessage);
		    
		}
		
		if(socketMessage.Contains("Button"))
		{
			Debug.Log(socketMessage);
			GenerateSpawnPosition();

			
		}
		
		if(showDebug)
		{
			
			Debug.Log("Potentiometer Value = " + potentiometerValue);
			Debug.Log("Heart Rate Value = " + hearRateValue);
			Debug.Log("Force Value = " + forceValue);
			
		}
		
		

        
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
		getXmasLightsValues();
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
