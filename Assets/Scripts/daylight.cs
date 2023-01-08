using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class daylight : MonoBehaviour {

    public float dayLength = 300f;

    private void Start()
    {
        InvokeRepeating("RotateSun", 0f, 0.001f);
    }

    private void RotateSun()
    {
        transform.Rotate(new Vector3(360f / dayLength / 1000f, 0, 0));
    }
}
