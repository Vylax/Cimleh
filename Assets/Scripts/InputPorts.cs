using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPorts : MonoBehaviour {

    public bool[] types = new bool[3] { true, false, false };

    public float[] stocked = new float[3] { 0f, 0f, 0f };
    public float[] stockSize = new float[3] { 1000f, 1000f, 1000f };

    public Machine machine;

    public void Update()
    {
        if(machine && stocked[0] > 0f)
        {
            machine.Input(0, stocked[0]);
            stocked[0] = 0;
        }
    }

    public float Input(int type, float amount)
    {
        if(!types[type])
        {
            return amount;
        }

        stocked[type] += amount;

        if(stocked[type] > stockSize[type])
        {
            float temp = stocked[type] - stockSize[type];
            stocked[type] = stockSize[type];
            return temp;
        }
        else
        {
            return 0f;
        }
    }
}
