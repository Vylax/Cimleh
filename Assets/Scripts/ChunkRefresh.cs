using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRefresh : MonoBehaviour {

    int chunkSize;
    public int loadRange = 1;
    public List<GameObject> lastLoadedChunks = new List<GameObject>();

    private Vector3 lastChunkRelativePos = Vector3.positiveInfinity;

    private void Start()
    {
        chunkSize = GameObject.Find("MapGenV2").GetComponent<MapGen2>().chunkSize;
    }

    private void Update()
    {
        int[] tempPos = new int[] { (int)transform.position.x / chunkSize, (int)transform.position.y / chunkSize, (int)transform.position.z / chunkSize };
        Vector3 tempVec = new Vector3(tempPos[0], tempPos[1], tempPos[2]);
        if (lastChunkRelativePos != tempVec)
        {
            lastLoadedChunks = GameObject.Find("MapGenV2").GetComponent<MapGen2>().ReloadChunks(loadRange, tempVec, lastLoadedChunks);
            lastChunkRelativePos = tempVec;
        }
    }
}
