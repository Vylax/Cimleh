using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

public class EditorChunkEdit : ScriptableWizard
{
    public bool selectAll = false;


    [MenuItem("My Tools/Chunks Edit")]
    static void EditorChunkEditWizard()
    {
        ScriptableWizard.DisplayWizard<EditorChunkEdit>("Chunks Edit", "Update selection", "Destroy selection");
    }

    void OnWizardCreate()
    {
        MapGen2 mg2 = GameObject.Find("MapGenV2").GetComponent<MapGen2>();

        GameObject[] chunks;
        if(!selectAll)
        {
            chunks = Selection.gameObjects;
        }
        else
        {
            chunks = new GameObject[mg2.chunks.GetLength(0) * mg2.chunks.GetLength(1) * mg2.chunks.GetLength(2)];
            int i = 0;
            for (int x = 0; x < mg2.chunks.GetLength(0); x++)
            {
                for (int y = 0; y < mg2.chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < mg2.chunks.GetLength(2); z++)
                    {
                        chunks[i] = mg2.chunks[x, y, z];
                        i++;
                    }
                }
            }
        }
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].GetComponent<ChunkMesh>().GenMesh();
        }
    }

    void OnWizardOtherButton()
    {
        MapGen2 mg2 = GameObject.Find("MapGenV2").GetComponent<MapGen2>();

        GameObject[] chunks;
        if (!selectAll)
        {
            chunks = Selection.gameObjects;
        }
        else
        {
            chunks = new GameObject[mg2.chunks.GetLength(0) * mg2.chunks.GetLength(1) * mg2.chunks.GetLength(2)];
            int i = 0;
            for (int x = 0; x < mg2.chunks.GetLength(0); x++)
            {
                for (int y = 0; y < mg2.chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < mg2.chunks.GetLength(2); z++)
                    {
                        chunks[i] = mg2.chunks[x, y, z];
                        i++;
                    }
                }
            }
        }
        for (int i = 0; i < chunks.Length; i++)
        {
            int[] pos = new int[3];
            for (int j = 0; j < pos.Length; j++)
            {
                pos[j] = chunks[i].GetComponent<ChunkMesh>().chunkPos[j] * mg2.chunkSize;
            }
            for (int x = 0; x < mg2.chunkSize; x++)
            {
                for (int y = 0; y < mg2.chunkSize; y++)
                {
                    for (int z = 0; z < mg2.chunkSize; z++)
                    {
                        if(x+y+z != 0 && x+y+z != (16-1)*3)
                        {
                            mg2.blocks[x + pos[0], y + pos[1], z + pos[2]] = -1;
                            chunks[i].GetComponent<Chunk>().blocks[x, y, z] = -1;
                        }
                    }
                }
            }
            mg2.DestroyBlock(pos[0], pos[1], pos[2], -1);
            mg2.DestroyBlock(pos[0] + mg2.chunkSize - 1, pos[1] + mg2.chunkSize - 1, pos[2] + mg2.chunkSize - 1, -1);
        }
    }
}
#endif