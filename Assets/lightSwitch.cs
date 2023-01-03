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



    private void OnTriggerEnter(Collider other){

        Debug.Log("Lightswitch collided");
        turnOnLight();


        
    }


    public void turnOnLight()
    {

        if (communicator.lightON) // false
        {
            pointLight.SetActive(false);
            communicator.sendTellStickSocket(tellID);
            lampShade.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            buttonObject.transform.eulerAngles = new Vector3(buttonObject.transform.eulerAngles.x, buttonObject.transform.eulerAngles.y, 12);
        }
        else
        {
            pointLight.SetActive(true); //here 
            communicator.sendTellStickSocket(tellID);
            lampShade.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            buttonObject.transform.eulerAngles = new Vector3(buttonObject.transform.eulerAngles.x, buttonObject.transform.eulerAngles.y, -8);
        }

    }


}
