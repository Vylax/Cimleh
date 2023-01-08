using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation : MonoBehaviour {
    public float speed = 1;

    // Update is called once per frame
    void Update()
    {
        Vector3 transform = new Vector3(0, 0, speed * 1 * Time.deltaTime);
        this.transform.Rotate(transform);
    }
}
