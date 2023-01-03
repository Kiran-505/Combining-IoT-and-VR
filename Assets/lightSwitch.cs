using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightSwitch : MonoBehaviour
{
    public GameObject pointLight;
    public bool isOn;

    private void OnTriggerEnter(Collider other){
        if (isOn){
            Debug.Log("REEEEEEEEEEEEEE");
            pointLight.GetComponent<Light>().intensity = 0f;
        }
        else {
            Debug.Log("AHHHHHHHHHHHHHHH");
            pointLight.GetComponent<Light>().intensity = 1.2f;

        }
    }

}
