using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightSwitch : MonoBehaviour
{
    public GameObject pointLight;
    public GameObject buttonObject;
    public GameObject lampShade;

    public string tellID;
    public RaspberryPiCommunicator communicator;



	private void OnTriggerEnter(Collider other){ //Turs on or off virtual light when it detects a collision between the virtual and and the virtual switch

        Debug.Log("Lightswitch collided");
        turnOnLight();


        
    }

	public void turnOnLight() //Turns on Light and Material Emission if the tellstick light is off and vice-versa and rotates the switch to simulate it being turned on/off
    {

        if (communicator.lightON)
        {
            pointLight.SetActive(false);
            communicator.sendTellStickSocket(tellID);
            lampShade.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
	        buttonObject.transform.eulerAngles = new Vector3(0, 0, 12);
        }
        else
        {
            pointLight.SetActive(true); 
            communicator.sendTellStickSocket(tellID);
            lampShade.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
	        buttonObject.transform.eulerAngles = new Vector3(180, 0, 12);
        }

    }


}
