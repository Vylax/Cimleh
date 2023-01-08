using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour {

    float energy = 0f;

	public float Input(int inputType, float amount)
    {
        energy += amount;
        return energy;
    }

    public float Output(int outputType, float amount)
    {
        energy -= amount;
        return energy;
    }

    public void OnGUI()
    {
        GUILayout.Box(transform.name + ": " + energy);
    }
}
