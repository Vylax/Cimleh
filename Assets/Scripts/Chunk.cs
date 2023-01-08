using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public int[,,] blocks;

    public int[] chunkPos = new int[3] { -1, -1, -1 };

    /*public void ReloadChunk(bool value)
    {
        foreach (Transform child in transform)
        {
            if(child.gameObject.GetComponent<Cube>().hbState)
            {
                child.gameObject.SetActive(value);
            }
        }
    }*/
}
