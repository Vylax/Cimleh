using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapElement : MonoBehaviour {

    void OnBecameInvisible()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
    }
}
