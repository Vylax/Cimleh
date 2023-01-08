using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMesh : MonoBehaviour
{
    int[,,] neighborsFacesId = new int[3, 3, 3]//-1, 0, 0 => 4 | 1, 0, 0 => 5 | 0, 0, -1 => 0 | 0, 0, 1 => 1 | 0, 1, 0 => 2 | 0, -1, 0 => 3
    {
        { { 7, 7, 7 }, { 7, 4, 7 }, { 7, 7, 7 } },
        { { 7, 3, 7 }, { 0, 7, 1 }, { 7, 2, 7 } },
        { { 7, 7, 7 }, { 7, 5, 7 }, { 7, 7, 7 } }
    };

    int[][] neighborsFacesDir = new int[][]
    {
        new int[] { 1, 0, 0 },
        new int[] { -1, 0, 0 },
        new int[] { 0, 1, 0 },
        new int[] { 0, -1, 0 },
        new int[] { 0, 0, 1 },
        new int[] { 0, 0, -1 },
    };

    Vector3[] vertices2 = new Vector3[8]
    {
        new Vector3(-0.5f, -0.5f, 0.5f),//back bottom right
        new Vector3(0.5f, -0.5f, 0.5f),//back bottom left
        new Vector3(0.5f, -0.5f, -0.5f),//front bottom left
        new Vector3(-0.5f, -0.5f, -0.5f),//front bottom right

        new Vector3(-0.5f, 0.5f, 0.5f),//back top right
        new Vector3(0.5f, 0.5f, 0.5f),//back top left
        new Vector3(0.5f, 0.5f, -0.5f),//front top left
        new Vector3(-0.5f, 0.5f, -0.5f),//front top right
    };

    int[][] facesVerticesArrayPos = new int[6][]//0 = front, 1 = back, 2 = top, 3 = bottom, 4 = right, 5 = left
    {
        new int[4]{3, 2, 7, 6},//ok
        new int[4]{1, 0, 5, 4},//ok
        new int[4]{7, 6, 4, 5},//ok
        new int[4]{0, 1, 3, 2},//ok
        new int[4]{0, 3, 4, 7},//ok
        new int[4]{2, 1, 6, 5}//ok
    };

    /*int[][][] triangles = new int[6][][]
    {
        new int[2][]//front
        {
            new int[3] { 1, 0, 4 },
            new int[3] { 1, 4, 5 },
        },
        new int[2][]//back
        {
            new int[3] { 2, 3, 7 },
            new int[3] { 2, 7, 6 },
        },
        new int[2][]//top
        {
            new int[3] { 5, 4, 7 },
            new int[3] { 5, 7, 6 },
        },
        new int[2][]//bottom
        {
            new int[3] { 1, 0, 3 },
            new int[3] { 1, 3, 2 },
        },
        new int[2][]//right
        {
            new int[3] { 0, 3, 7 },
            new int[3] { 0, 7, 4 },
        },
        new int[2][]//gauche
        {
            new int[3] { 2, 1, 5 },
            new int[3] { 2, 5, 6 },
        }
    };*/

    Vector2[] uvs = new Vector2[]
    {
        new Vector2(1f, 1f),
        new Vector2(0f, 1f),
        new Vector2(0f, 0f),
        new Vector2(1f, 0f)
    };

    Vector2[][][] facesUVs = new Vector2[][][]
    {
        new Vector2[][]//bloc id 0
        {
            new Vector2[4]// face id 0
            {
                new Vector2(.4f, .2f),
                new Vector2(.6f, .2f),
                new Vector2(.4f, 0f),
                new Vector2(.6f, 0f)
            },
            new Vector2[4]// face id 1
            {
                new Vector2(.4f, .2f),
                new Vector2(.6f, .2f),
                new Vector2(.4f, 0f),
                new Vector2(.6f, 0f)
            },
            new Vector2[4]// face id 2 top
            {
                new Vector2(.8f, .6f),
                new Vector2(1f, .6f),
                new Vector2(.8f, .4f),
                new Vector2(1f, .4f),
            },
            new Vector2[4]// face id 3 bottom
            {
                new Vector2(.6f, .6f),
                new Vector2(.4f, .6f),
                new Vector2(.4f, .4f),
                new Vector2(.6f, .4f)
            },
            new Vector2[4]// face id 4
            {
                new Vector2(.4f, .2f),
                new Vector2(.6f, .2f),
                new Vector2(.4f, 0f),
                new Vector2(.6f, 0f)
            },
            new Vector2[4]// face id 5
            {
                new Vector2(.4f, .2f),
                new Vector2(.6f, .2f),
                new Vector2(.4f, 0f),
                new Vector2(.6f, 0f)
            },
        }
    };

    Vector2[][] facesUVs2 = new Vector2[][]
    {
        new Vector2[4]// bloc id 0 grass
        {
            new Vector2(.8f, .6f),
            new Vector2(1f, .6f),
            new Vector2(.8f, .4f),
            new Vector2(1f, .4f),
        },
        new Vector2[4]// bloc id 1 stone
        {
            new Vector2(0f, .6f),
            new Vector2(.2f, .6f),
            new Vector2(0f, .4f),
            new Vector2(.2f, .4f)
        },
        new Vector2[4]// bloc id 2 water
        {
            new Vector2(.4f, .8f),
            new Vector2(.6f, .8f),
            new Vector2(.4f, 1f),
            new Vector2(.6f, 1f)
        },
        new Vector2[4]// bloc id 3 dirt a
        {
            new Vector2(.4f, .6f),
            new Vector2(.6f, .6f),
            new Vector2(.4f, .4f),
            new Vector2(.6f, .4f)
        },
        new Vector2[4]// bloc id 4
        {
            new Vector2(.8f, .2f),
            new Vector2(1f, .2f),
            new Vector2(.8f, 0f),
            new Vector2(1f, 0f)
        }
    };

    int[] tris2 = new int[6]
    {
        1,
        0,
        2,

        1,
        2,
        3,
    };

    Vector3[] facesNormales = new Vector3[6]
    {
        Vector3.back,// Front
    	Vector3.forward,// Back
	    Vector3.up,// Top
    	Vector3.down,// Bottom
	    Vector3.right,// Right
    	Vector3.left,// Left
    };

    List<Vector3> verticesList = new List<Vector3>();
    List<Vector3> normalesList = new List<Vector3>();
    List<int> trisList = new List<int>();
    public List<int> associatedTrisList = new List<int>();
    public List<int> faceNeighborCube = new List<int>();
    public List<int> faceCube = new List<int>();
    List<Vector2> uvsList = new List<Vector2>();
    List<Color> colors = new List<Color>();
    /*
    public Color[] blocksColors = new Color[]
    {
        Color.green,
        Color.grey,
        Color.blue, //water
        new Color(0.53f, 0.35f, 0.18f), //dirt
        Color.magenta, //test
        Color.black, //mineral
    };*/

    MeshFilter filter;

    MapGen2 mg2;
    public int[] chunkPos;
    Chunk chunk;

    bool d = true;
    bool e = true;

    public void GenMesh()
    {
        if(!GetComponent<MeshFilter>())
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }

        Mesh mesh = filter.mesh;
        mesh.Clear();

        verticesList.Clear();
        normalesList.Clear();
        trisList.Clear();
        associatedTrisList.Clear();
        faceNeighborCube.Clear();
        faceCube.Clear();
        uvsList.Clear();
        colors.Clear();

        chunk = this.GetComponent<Chunk>();
        mg2 = GameObject.Find("MapGenV2").GetComponent<MapGen2>();
        int[,,][] faces = new int[mg2.chunkSize, mg2.chunkSize, mg2.chunkSize][];
        chunkPos = chunk.chunkPos;

        for (int x = 0; x < mg2.chunkSize; x++)
        {
            for (int y = 0; y < mg2.chunkSize; y++)
            {
                for (int z = 0; z < mg2.chunkSize; z++)
                {
                    faces[x, y, z] = new int[6] { 0, 0, 0, 0, 0, 0 };
                    if (chunk.blocks[x, y, z] != -1)
                    {
                        for (int i = 0; i < neighborsFacesDir.Length; i++)
                        {
                            int faceX = neighborsFacesDir[i][0];
                            int faceY = neighborsFacesDir[i][1];
                            int faceZ = neighborsFacesDir[i][2];

                            int neighborCubeX = x + faceX + chunkPos[0] * mg2.chunkSize;
                            int neighborCubeY = y + faceY + chunkPos[1] * mg2.chunkSize;
                            int neighborCubeZ = z + faceZ + chunkPos[2] * mg2.chunkSize;

                            int neighBorsFaceId = neighborsFacesId[faceX + 1, faceY + 1, faceZ + 1];

                            if (neighborCubeX >= 0 && neighborCubeX < mg2.sizes[0] && neighborCubeY >= 0 && neighborCubeY < mg2.sizes[1] && neighborCubeZ >= 0 && neighborCubeZ < mg2.sizes[2] && neighBorsFaceId != 7)//if neighborsCube is inbounds
                            {
                                int neighBorsCube = mg2.blocks[neighborCubeX, neighborCubeY, neighborCubeZ];
                                if (neighBorsCube == -1 || neighBorsCube == 2)//air or water
                                {
                                    faces[x, y, z][i] = 1;
                                    AddFace(x, y, z, neighBorsFaceId, neighborCubeX, neighborCubeY, neighborCubeZ);
                                    //mainBlock = true;
                                }
                                else
                                {
                                    faces[x, y, z][i] = 0;
                                }
                            }
                            else
                            {
                                if (neighBorsFaceId == 7)
                                {
                                    Debug.Log("ya un pb");
                                }
                                faces[x, y, z][i] = 1;
                                AddFace(x, y, z, neighBorsFaceId, -1, -1, -1);
                                //mainBlock = true;
                            }
                        }
                    }
                    if (z > 50 && !e)
                    {
                        Debug.Log("mesh bonz");
                        e = true;
                    }
                }
            }
            //debug
            if(x> 50 && !d)
            {
                Debug.Log("mesh bonx");
                d = true;
            }
        }

        //Debug.Log(colors.Count + " " + verticesList.Count);
        
        mesh.vertices = verticesList.ToArray();
        mesh.colors = colors.ToArray();
        mesh.normals = normalesList.ToArray();
        mesh.uv = uvsList.ToArray();
        mesh.triangles = trisList.ToArray();

        mesh.RecalculateBounds();
        if(gameObject.GetComponent<MeshCollider>())
        {
            DestroyImmediate(GetComponent<MeshCollider>());
        }
        gameObject.AddComponent<MeshCollider>();

        GetComponent<MeshRenderer>().material.mainTexture = (Texture2D)Resources.Load("Tiles/textures");

        //mesh.RecalculateNormals();
        //Debug.Log("hehe boiii2");
    }

    void AddFace(int x, int y, int z, int neighBorsFaceId, int neighborCubeX, int neighborCubeY, int neighborCubeZ)
    {
        int currCount = verticesList.Count;
        Vector3[] temp = faceVertices(neighBorsFaceId, x, y, z);
        for (int j = 0; j < temp.Length; j++)
        {
            verticesList.Add(temp[j]);//add vertices
            normalesList.Add(facesNormales[neighBorsFaceId]);//add vertices normales
            /*if(!(neighBorsFaceId != 2 && this.GetComponent<Chunk>().blocks[x, y, z] == 0))
            {
                colors.Add(blocksColors[this.GetComponent<Chunk>().blocks[x, y, z]]);//add vertices colors
            }
            else
            {
                colors.Add(blocksColors[3]);//dirt color
            }*/
            colors.Add(Color.white);
            
        }

        //add triangles
        for (int k = 0; k < tris2.Length; k++)
        {
            trisList.Add(currCount + tris2[k]);
            int tempIndex = k - 3 * (k / 3) + 3 * Mathf.FloorToInt(Mathf.Abs(1 - Mathf.FloorToInt(k / 3f)));
            associatedTrisList.Add(currCount + tris2[tempIndex]);
        }

        //add uvs
        for (int k = 0; k < uvs.Length; k++)
        {
            if(chunk.blocks[x, y, z] != -1)
            {
                if(chunk.blocks[x, y, z] == 0)
                {
                    uvsList.Add(facesUVs[chunk.blocks[x, y, z]][neighBorsFaceId][k]);
                }
                else
                {
                    uvsList.Add(facesUVs2[chunk.blocks[x, y, z]][k]);
                }
            }
        }

        //add facing cube coordinates for both triangles of the face
        for (int i = 0; i < 2; i++)
        {
            faceNeighborCube.Add(neighborCubeX);
            faceNeighborCube.Add(neighborCubeY);
            faceNeighborCube.Add(neighborCubeZ);
            faceCube.Add(x + chunkPos[0] * mg2.chunkSize);
            faceCube.Add(y + chunkPos[1] * mg2.chunkSize);
            faceCube.Add(z + chunkPos[2] * mg2.chunkSize);
        }
    }

    Vector3[] faceVertices(int faceId, int x, int y, int z)
    {
        Vector3[] temp = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            Vector3 tempVec = vertices2[facesVerticesArrayPos[faceId][i]];
            temp[i] = new Vector3(tempVec.x + x, tempVec.y + y, tempVec.z + z);
        }
        return temp;
    }
}
