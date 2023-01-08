using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputPorts : MonoBehaviour {

    public InputPorts outputTarget;

    public bool[] transferType = new bool[3] { true, false, false };

    public float[] transferRate = new float[3] { 10f, 10f, 10f };
    public float[] stocked = new float[3] { 0f, 0f, 0f };
    public float[] nextTransferTime = new float[3] { -10f, -10f, -10f };
    public float[] transferSpeed = new float[3] { 10f, 10f, 10f };

    private void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if(nextTransferTime[i] <= Time.time && transferType[i])
            {
                Output(i);
            }
        }
    }

    private void Output(int type)
    {
        nextTransferTime[type] = Time.time + transferSpeed[type];
        if(stocked[type] >= transferRate[type])
        {
            stocked[type] -= (transferRate[type]-outputTarget.Input(type, transferRate[type]));
        }
        else if(stocked[type] > 0)
        {
            stocked[type] = outputTarget.Input(type, stocked[type]);
        }
    }

}
