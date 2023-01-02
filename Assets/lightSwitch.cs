using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightSwitch : MonoBehaviour
{
    public GameObject pointLight;
    public bool isOn;

    private void OnTriggerEnter(Collider other){
        if (isOn){
        pointLight.SetActive(false);
        isOn = false;
        }
        else {
        pointLight.SetActive(true);
        isOn = true;
        }
    }
}
