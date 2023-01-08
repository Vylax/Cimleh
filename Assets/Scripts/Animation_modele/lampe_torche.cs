using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lampe_torche : MonoBehaviour {

    public bool togle = false;
    private bool temp;
    public GameObject bouton;
    
    void Start()
    {
        temp = togle;
    }

	// Update is called once per frame
	void Update () {
		if (temp == !togle)
        {
            if (togle == false) {
                bouton.transform.position += new Vector3(0, 1/10, 0);
            }
            else
            {
                bouton.transform.position += new Vector3(0, 1/10, 0);
            }
        }
        temp = togle;
	}
}
