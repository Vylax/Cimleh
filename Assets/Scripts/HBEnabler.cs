using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HBEnabler : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Item")
        {
            other.GetComponent<ItemPickup>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            other.GetComponent<ItemPickup>().enabled = false;
        }
    }
}
