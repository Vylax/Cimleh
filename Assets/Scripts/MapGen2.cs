using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen2 : MonoBehaviour {

    public int[,,] blocks;//x, y, z
    //-1 = air, 0 = grass, 1 = stone, 2 = water, 3 = dirt, 4 = machine
    public GameObject[,,] blocksObjs;//x, y, z
    public GameObject[,,] chunks;
    public int[] chunkSizes;

    public float treeChance = 0.005f;

    int[,,] neighborsFacesDir = new int[3, 3, 3]//-1, 0, 0 => 4 | 1, 0, 0 => 5 | 0, 0, -1 => 0 | 0, 0, 1 => 1 | 0, 1, 0 => 2 | 0, -1, 0 => 3
    {
        { { -1, -1, -1 }, { -1, 4, -1 }, { -1, -1, -1 } },
        { { -1, 3, -1 }, { 0, -1, 1 }, { -1, 2, -1 } },
        { { -1, -1, -1 }, { -1, 5, -1 }, { -1, -1, -1 } }
    };

    int[][] neighborsFacesDir2 = new int[][]
    {
        new int[] { 1, 0, 0 },
        new int[] { -1, 0, 0 },
        new int[] { 0, 1, 0 },
        new int[] { 0, -1, 0 },
        new int[] { 0, 0, 1 },
        new int[] { 0, 0, -1 },
    };

    public GameObject[] cubesPrefabs;
    public GameObject treePrefab;
    public bool isGenerating = false;
    public bool fixedGen = false;
    public bool genLake = true;
    public bool checkDisplayCubes = false;
    public Material air;

    public int[] sizes = new int[3] { 96, 96, 96 };
    public bool[] enabledCubes = new bool[] { };

    public float[] stoneLayer1 = new float[5] { 0, 10, 3, 1.2f, 0 };//flat
    //public float[] stoneLayer2 = new float[5] { 0, 20, 4, 0, 0 };//rolling hills
    public float[] stoneLayer3 = new float[5] { 0, 20, 4, 0, 0 };//mountains
    public float[] grassLayer1 = new float[5] { 0, 50, 2, 0, 5 };
    public float[] mineralLayer1 = new float[5] { 1, 12, 16, 1, 10 };
    public float[] cavesLayer1 = new float[5] { 2, 16, 14, 1, 10 };
    public float[] stoneHillLayer1 = new float[5] { 0, 50, 8, 2, 10 };
    public float[] grassHillLayer1 = new float[5] { 0, 50, 8, 2, 21 };
    
    public List<int[]> trees = new List<int[]>();//0 = no tree, 1 = tree

    public Vector3 playerPos = Vector3.zero;
    public Quaternion playerRot = Quaternion.identity;

    public int lakeHeight = 3;
    public int chunkSize = 16;

    private GameObject player;
    public GameObject playerPrefab;
    public GameObject chunkObj;
    private GameObject cam;

    public bool observationMode = false;

    public bool hill = false;

    private void Start()
    {
        System.GC.Collect();
        cam = Camera.main.gameObject;
    }

    private void ToggleCubesFaces(int id, bool value)
    {
        foreach (Transform child in transform)
        {
            if(child.name.Contains("TestBlock"))
            {
                for (int i = 0; i < 6; i++)
                {
                    child.GetComponent<Cube>().EditFace(i, value);
                }
            }
        }
    }

    public void ResetMap()
    {
        GameObject[] mapObjs = GameObject.FindGameObjectsWithTag("Map");
        for (int i = 0; i < mapObjs.Length; i++)
        {
            Destroy(mapObjs[i]);
        }
        System.GC.Collect();
    }

    public int[] lowestPoint = new int[3] { int.MaxValue, int.MaxValue, int.MaxValue };

    public bool GenMap (int[] _sizes, string _seed)
    {
        sizes = _sizes;
        
        lowestPoint = new int[3] { int.MaxValue, int.MaxValue, int.MaxValue };
        blocks = new int[sizes[0], sizes[1], sizes[2]];
        blocksObjs = new GameObject[sizes[0], sizes[1], sizes[2]];
        chunkSizes = new int[3] { sizes[0] / chunkSize, sizes[1] / chunkSize, sizes[2] / chunkSize };
        int[] rdms = new int[4] { int.Parse(_seed.Substring(0, 4)), int.Parse(_seed.Substring(4, 4)), int.Parse(_seed.Substring(8, 4)), int.Parse(_seed.Substring(12, 4)) };
        chunks = new GameObject[chunkSizes[0], chunkSizes[1], chunkSizes[2]];
        trees.Clear();

        for (int xChunk = 0; xChunk < chunkSizes[0]; xChunk++)
        {
            for (int yChunk = 0; yChunk < chunkSizes[1]; yChunk++)
            {
                for (int zChunk = 0; zChunk < chunkSizes[2]; zChunk++)
                {
                    chunks[xChunk, yChunk, zChunk] = Instantiate(chunkObj, new Vector3(xChunk * chunkSize, yChunk * chunkSize, zChunk * chunkSize), Quaternion.identity);
                    //chunks[xChunk, yChunk, zChunk].GetComponent<Chunk>().blocks = new int[chunkSize, chunkSize, chunkSize];
                    chunks[xChunk, yChunk, zChunk].GetComponent<Chunk>().chunkPos = new int[3] { xChunk, yChunk, zChunk };
                    chunks[xChunk, yChunk, zChunk].GetComponent<Chunk>().blocks = new int[chunkSize, chunkSize, chunkSize];
                    chunks[xChunk, yChunk, zChunk].name = "Chunk (" + xChunk + ", " + yChunk + ", " + zChunk + ")";
                    chunks[xChunk, yChunk, zChunk].transform.parent = transform;
                    chunks[xChunk, yChunk, zChunk].transform.tag = "Map";
                    //chunks[xChunk, yChunk, zChunk].SetActive(false);
                }
            }
        }

       /* if (fixedGen)
        {
            rdms = new int[4] { (int)stoneLayer1[0], (int)stoneLayer3[0], (int)grassLayer1[0], 2 };
        }*/
        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            int chunkX = x / chunkSize;
            for (int z = 0; z < blocks.GetLength(2); z++)
            {
                int chunkZ = z / chunkSize;
                int stone;
                int grass;
                if(hill)
                {
                    stone = PerlinNoise(x, rdms[0], z, stoneHillLayer1[1], stoneHillLayer1[2], stoneHillLayer1[3]) + (int)stoneHillLayer1[4];
                    grass = PerlinNoise(x, rdms[0], z, grassHillLayer1[1], grassHillLayer1[2], grassHillLayer1[3]) + (int)grassHillLayer1[4];
                }
                else
                {
                    stone = PerlinNoise(x, rdms[0], z, stoneLayer1[1], stoneLayer1[2], stoneLayer1[3]) + (int)stoneLayer1[4];
                    stone += PerlinNoise(x, rdms[1] * (int)stoneLayer3[0], z, stoneLayer3[1], stoneLayer3[2], stoneLayer3[3]) + (int)stoneLayer3[4];
                    grass = PerlinNoise(x, rdms[2], z, grassLayer1[1], grassLayer1[2], grassLayer1[3]) + (int)grassLayer1[4];
                }

                if(grass < lowestPoint[1]-1 && stone < grass)
                {
                    lowestPoint = new int[3] { x, grass+1, z };
                }else if(grass == lowestPoint[1]-1 && stone < grass && Mathf.Abs(x-sizes[0]) < 1f/3f* sizes[0] && Mathf.Abs(z - sizes[2]) < 1f / 3f * sizes[2])
                {
                    lowestPoint = new int[3] { x, grass + 1, z };
                }
                
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    int chunkY = y / chunkSize;

                    if (y < stone)
                    {
                        blocks[x, y, z] = 1;

                        int temp2 = PerlinNoise(x, y + rdms[3], z, cavesLayer1[1], cavesLayer1[2], cavesLayer1[3]);
                        if (temp2 > cavesLayer1[4])
                        {
                            if(y < grass)
                            {
                                blocks[x, y, z] = -1;
                            }
                        }
                    }
                    else if (y == grass)
                    {
                        //Debug.Log("wutm8");
                        blocks[x, y, z] = 0;
                        float rdm2 = Random.Range(0f, 1f);
                        if (rdm2 <= treeChance)
                        {
                            trees.Add(new int[3] { x, y, z });
                        }
                    }
                    else if (y < grass)
                    {
                        blocks[x, y, z] = 3;
                    }
                    else
                    {
                        blocks[x, y, z] = -1;
                    }
                    chunks[chunkX, chunkY, chunkZ].GetComponent<Chunk>().blocks[x - chunkX * chunkSize, y - chunkY * chunkSize, z - chunkZ * chunkSize] = blocks[x, y, z];
                }
            }
        }
        if(genLake)
        {
            //Debug.Log("lake point: (" + lowestPoint[0] + ", " + lowestPoint[1] + ", " + lowestPoint[2] + ")");
            for (int i = 0; i < lakeHeight; i++)
            {
                GenerateLake(lowestPoint[0], lowestPoint[1]+i, lowestPoint[2], 0);
            }
        }
        return true;
    }

    public bool LoadMap()
    {
        chunkSizes = new int[3] { sizes[0] / chunkSize, sizes[1] / chunkSize, sizes[2] / chunkSize };
        chunks = new GameObject[chunkSizes[0], chunkSizes[1], chunkSizes[2]];

        for (int xChunk = 0; xChunk < chunkSizes[0]; xChunk++)
        {
            for (int yChunk = 0; yChunk < chunkSizes[1]; yChunk++)
            {
                for (int zChunk = 0; zChunk < chunkSizes[2]; zChunk++)
                {
                    chunks[xChunk, yChunk, zChunk] = Instantiate(chunkObj, new Vector3(xChunk * chunkSize, yChunk * chunkSize, zChunk * chunkSize), Quaternion.identity);
                    chunks[xChunk, yChunk, zChunk].GetComponent<Chunk>().chunkPos = new int[3] { xChunk, yChunk, zChunk };
                    chunks[xChunk, yChunk, zChunk].GetComponent<Chunk>().blocks = new int[chunkSize, chunkSize, chunkSize];
                    chunks[xChunk, yChunk, zChunk].name = "Chunk (" + xChunk + ", " + yChunk + ", " + zChunk + ")";
                    chunks[xChunk, yChunk, zChunk].transform.parent = transform;
                }
            }
        }
        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            int chunkX = x / chunkSize;
            for (int z = 0; z < blocks.GetLength(2); z++)
            {
                int chunkZ = z / chunkSize;
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    int chunkY = y / chunkSize;
                    chunks[chunkX, chunkY, chunkZ].GetComponent<Chunk>().blocks[x - chunkX * chunkSize, y - chunkY * chunkSize, z - chunkZ * chunkSize] = blocks[x, y, z];
                }
            }
        }
        return true;
    }

    public List<int[]>[] Lakes = new List<int[]>[1];//array de listes d'arrays d'entiers

    void GenerateLake(int x, int y, int z, int id)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j <= 1; j++)//corners included
            {
                if (z + j >= 0 && z + j < sizes[2] && x + i >= 0 && x + i < sizes[0])//si le cube actuel ne se trouve pas en dehors des limites de la map
                {
                    if(blocks[x+i, y, z+j] == -1)//if it's air
                    {
                        blocks[x + i, y, z + j] = 2;//fill it with water
                        int chunkX = (x + i) / chunkSize;
                        int chunkY = y / chunkSize;
                        int chunkZ = (z + j) / chunkSize;

                        chunks[chunkX, chunkY, chunkZ].GetComponent<Chunk>().blocks[x + i - chunkX * chunkSize, y - chunkY * chunkSize, z + j - chunkZ * chunkSize] = blocks[x + i, y, z + j];

                        List<int> temp = new List<int>();

                        for (int k = 0; k < trees.Count; k++)
                        {
                            if(trees[k][0] == x+i && trees[k][2] == z+j)
                            {
                                temp.Add(k);
                            }
                        }

                        temp.Sort();

                        for (int k = temp.Count-1; k > 0; k--)
                        {
                            Debug.Log(k + " " + temp[k]);
                            trees.RemoveAt(temp[k]);
                        }

                        //Lakes[id].Add(new int[] { x + i, y, z + j });
                        if (i != 0 && j != 0)//prevent infinite self-calling method
                        {
                            GenerateLake(x + i, y, z + j, id);
                        }
                    }
                }
            }
        }

    }

    public bool BuildMap2()
    {
        if (!observationMode)
        {
            Destroy(player);
            cam.SetActive(true);
        }
        for (int x = 0; x < chunkSizes[0]; x++)
        {
            for (int y = 0; y < chunkSizes[1]; y++)
            {
                for (int z = 0; z < chunkSizes[2]; z++)
                {
                    chunks[x, y, z].GetComponent<ChunkMesh>().GenMesh();
                }
            }
        }
        for (int i = 0; i < trees.Count; i++)
        {
            GameObject spawnedTree = Instantiate(treePrefab, new Vector3(trees[i][0], trees[i][1], trees[i][2]), Quaternion.Euler(-90, 0, Random.Range(0f, 360f)));
            spawnedTree.transform.parent = this.transform;
            spawnedTree.tag = "Map";
        }
        if (!observationMode)
        {
            cam.SetActive(false);
            if(playerPos == Vector3.zero)
            {
                playerPos = new Vector3(sizes[0] / 2, 65, sizes[2] / 2);
            }
            player = Instantiate(playerPrefab, playerPos, playerRot);
        }
        GameObject.FindGameObjectWithTag("MapBounds").GetComponent<MapBounds>().GenMesh(sizes[0], sizes[1], sizes[2]);
        return true;
    }

    bool CheckBoolArray (bool[] array, bool lookedState)//check if at least one value is equal to lookedState in the bool array and if so return true, otherwise return false
    {
        for (int i = 0; i < array.Length; i++)
        {
            if(array[i] == lookedState)
            {
                return lookedState;
            }
        }
        return !lookedState;
    }

    int Noise2(int x, int y, float scale, float mag, float exp)
    {

        return (int)(Mathf.Pow((Mathf.PerlinNoise(x / scale, y / scale) * mag), (exp)));

    }

    int PerlinNoise(int x, int y, int z, float scale, float height, float power)
    {
        float rValue;
        rValue = Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
        rValue *= height;

        if (power != 0)
        {
            rValue = Mathf.Pow(rValue, power);
        }

        return (int)rValue;
    }

    void BuildMap ()
    {
        for (int xChunk = 0; xChunk < chunkSizes[0]; xChunk++)
        {
            for (int yChunk = 0; yChunk < chunkSizes[1]; yChunk++)
            {
                for (int zChunk = 0; zChunk < chunkSizes[2]; zChunk++)
                {
                    for (int x = 0; x < chunkSize; x++)
                    {
                        for (int y = 0; y < chunkSize; y++)
                        {
                            for (int z = 0; z < chunkSize; z++)
                            {
                                int[] xyz = new int[3] { x + xChunk * chunkSize, y + yChunk * chunkSize, z + zChunk * chunkSize };
                                if (blocks[xyz[0], xyz[1], xyz[2]] != -1 && enabledCubes[blocks[xyz[0], xyz[1], xyz[2]]])
                                {
                                    blocksObjs[xyz[0], xyz[1], xyz[2]] = Instantiate(cubesPrefabs[blocks[xyz[0], xyz[1], xyz[2]]], new Vector3(xyz[0], xyz[1], xyz[2]), Quaternion.identity);
                                    //blocksObjs[x, y, z].tag = "Map";
                                    blocksObjs[xyz[0], xyz[1], xyz[2]].transform.parent = chunks[xChunk, yChunk, zChunk].transform;

                                    if (blocks[xyz[0], xyz[1], xyz[2]] == 0)
                                    {
                                        //blocksObjs[xyz[0], xyz[1], xyz[2]].GetComponent<Renderer>().material.color = Color.green;
                                        blocksObjs[xyz[0], xyz[1], xyz[2]].name = "Grass (" + x + ", " + y + ", " + z + ")";
                                    }
                                    else if (blocks[xyz[0], xyz[1], xyz[2]] == 1)
                                    {
                                        //blocksObjs[xyz[0], xyz[1], xyz[2]].GetComponent<Renderer>().material.color = Color.grey;
                                        blocksObjs[xyz[0], xyz[1], xyz[2]].name = "Stone (" + x + ", " + y + ", " + z + ")";
                                    }
                                    else if (blocks[xyz[0], xyz[1], xyz[2]] == 2)
                                    {
                                        //blocksObjs[xyz[0], xyz[1], xyz[2]].GetComponent<Renderer>().material.color = Color.grey;
                                        blocksObjs[xyz[0], xyz[1], xyz[2]].name = "Water (" + x + ", " + y + ", " + z + ")";
                                    }
                                    else if (blocks[xyz[0], xyz[1], xyz[2]] == 3)
                                    {
                                        //blocksObjs[xyz[0], xyz[1], xyz[2]].GetComponent<Renderer>().material.color = Color.grey;
                                        blocksObjs[xyz[0], xyz[1], xyz[2]].name = "Dirt (" + x + ", " + y + ", " + z + ")";
                                    }
                                    else if (blocks[xyz[0], xyz[1], xyz[2]] == 4)
                                    {
                                        //blocksObjs[xyz[0], xyz[1], xyz[2]].GetComponent<Renderer>().material.color = Color.grey;
                                        blocksObjs[xyz[0], xyz[1], xyz[2]].name = "TestBlock (" + x + ", " + y + ", " + z + ")";
                                    }
                                    else if (blocks[xyz[0], xyz[1], xyz[2]] == 5)
                                    {
                                        //blocksObjs[xyz[0], xyz[1], xyz[2]].GetComponent<Renderer>().material.color = Color.grey;
                                        blocksObjs[xyz[0], xyz[1], xyz[2]].name = "TestMineral (" + x + ", " + y + ", " + z + ")";
                                    }
                                    if(!observationMode)
                                    {
                                        //blocksObjs[xyz[0], xyz[1], xyz[2]].SetActive(false);
                                    }
                                }
                                //Debug.Log(x + ", " + y + ", " + z);
                                //chunks[xChunk, yChunk, zChunk].GetComponent<Chunk>().blocks[x, y, z] = blocks[x, y, z];
                            }
                        }
                    }
                }
            }
        }

        #region old version
        /*
        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int z = 0; z < blocks.GetLength(2); z++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    if(blocks[x, y, z] != -1 && enabledCubes[blocks[x, y, z]])
                    {
                        blocksObjs[x, y, z] = Instantiate(cubesPrefabs[blocks[x, y, z]], new Vector3(x, y, z), Quaternion.identity);
                        //blocksObjs[x, y, z].tag = "Map";
                        blocksObjs[x, y, z].transform.parent = transform;
                        
                        if (blocks[x, y, z] == 0)
                        {
                            //blocksObjs[x, y, z].GetComponent<Renderer>().material.color = Color.green;
                            blocksObjs[x, y, z].name = "Grass (" + x + ", " + y + ", " + z + ")";
                        }
                        else if (blocks[x, y, z] == 1)
                        {
                            //blocksObjs[x, y, z].GetComponent<Renderer>().material.color = Color.grey;
                            blocksObjs[x, y, z].name = "Stone (" + x + ", " + y + ", " + z + ")";
                        }
                        else if (blocks[x, y, z] == 2)
                        {
                            //blocksObjs[x, y, z].GetComponent<Renderer>().material.color = Color.grey;
                            blocksObjs[x, y, z].name = "Water (" + x + ", " + y + ", " + z + ")";
                        }
                        else if (blocks[x, y, z] == 3)
                        {
                            //blocksObjs[x, y, z].GetComponent<Renderer>().material.color = Color.grey;
                            blocksObjs[x, y, z].name = "Dirt (" + x + ", " + y + ", " + z + ")";
                        }
                        else if (blocks[x, y, z] == 4)
                        {
                            //blocksObjs[x, y, z].GetComponent<Renderer>().material.color = Color.grey;
                            blocksObjs[x, y, z].name = "TestBlock (" + x + ", " + y + ", " + z + ")";
                        }
                        else if (blocks[x, y, z] == 5)
                        {
                            //blocksObjs[x, y, z].GetComponent<Renderer>().material.color = Color.grey;
                            blocksObjs[x, y, z].name = "TestMineral (" + x + ", " + y + ", " + z + ")";
                        }
                    }
                }
            }
        }*/
        #endregion

        CheckCubesFaces();
        if (!observationMode)
        {
            cam.SetActive(false);
            player = Instantiate(playerPrefab, new Vector3(8, 50, 8), Quaternion.identity);
        }
        System.GC.Collect();
        isGenerating = false;
    }

    void CheckCubesFaces ()
    {
        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int z = 0; z < blocks.GetLength(2); z++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    if (blocks[x, y, z] != -1 && enabledCubes[blocks[x, y, z]])
                    {
                        for (int i2 = 0; i2 < neighborsFacesDir2.Length; i2++)
                        {
                            if (x + neighborsFacesDir2[i2][0] >= 0 && x + neighborsFacesDir2[i2][0] < sizes[0] && y + neighborsFacesDir2[i2][1] >= 0 && y + neighborsFacesDir2[i2][1] < sizes[1] && z + neighborsFacesDir2[i2][2] >= 0 && z + neighborsFacesDir2[i2][2] < sizes[2])
                            {
                                if (blocks[x + neighborsFacesDir2[i2][0], y + neighborsFacesDir2[i2][1], z + neighborsFacesDir2[i2][2]] == -1 || blocks[x + neighborsFacesDir2[i2][0], y + neighborsFacesDir2[i2][1], z + neighborsFacesDir2[i2][2]] == 2)//air or water
                                {
                                    if (neighborsFacesDir[neighborsFacesDir2[i2][0] + 1, neighborsFacesDir2[i2][1] + 1, neighborsFacesDir2[i2][2] + 1] != -1)
                                    {
                                        blocksObjs[x, y, z].GetComponent<Cube>().EditHitBox(neighborsFacesDir[neighborsFacesDir2[i2][0] + 1, neighborsFacesDir2[i2][1] + 1, neighborsFacesDir2[i2][2] + 1], true);
                                    }
                                }
                                else
                                {
                                    if (neighborsFacesDir[neighborsFacesDir2[i2][0] + 1, neighborsFacesDir2[i2][1] + 1, neighborsFacesDir2[i2][2] + 1] != -1)
                                    {
                                        blocksObjs[x, y, z].GetComponent<Cube>().EditHitBox(neighborsFacesDir[neighborsFacesDir2[i2][0] + 1, neighborsFacesDir2[i2][1] + 1, neighborsFacesDir2[i2][2] + 1], false);
                                    }
                                }
                            }
                            else
                            {
                                if (neighborsFacesDir[neighborsFacesDir2[i2][0] + 1, neighborsFacesDir2[i2][1] + 1, neighborsFacesDir2[i2][2] + 1] != -1)
                                {
                                    blocksObjs[x, y, z].GetComponent<Cube>().EditHitBox(neighborsFacesDir[neighborsFacesDir2[i2][0] + 1, neighborsFacesDir2[i2][1] + 1, neighborsFacesDir2[i2][2] + 1], true);
                                }
                            }
                        }
                        if(!blocksObjs[x, y, z].GetComponent<Cube>().hbState)
                        {
                            blocksObjs[x, y, z].SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public List<GameObject> ReloadChunks(int range, Vector3 relativeOrigin, List<GameObject> lastLoadedChunks)
    {
        List<GameObject> temp = new List<GameObject>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                for (int z = -range; z <= range; z++)
                {
                    int relativeX = x + (int)relativeOrigin.x;
                    int relativeY = y + (int)relativeOrigin.y;
                    int relativeZ = z + (int)relativeOrigin.z;
                    if (relativeX >= 0 && relativeX < chunkSizes[0] && relativeY >= 0 && relativeY < chunkSizes[1] && relativeZ >= 0 && relativeZ < chunkSizes[2])
                    {
                        if (Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2) <= Mathf.Pow(range, 2))
                        {
                            //load/unload chunk
                            temp.Add(chunks[relativeX, relativeY, relativeZ]);
                            chunks[relativeX, relativeY, relativeZ].SetActive(true);
                            /*if (!lastLoadedChunks.Contains(chunks[relativeX, relativeY, relativeZ]))
                            {
                                chunks[relativeX, relativeY, relativeZ].GetComponent<Chunk>().ReloadChunk(true);
                            }*/
                        }
                    }
                }
            }
        }
        for (int i = 0; i < lastLoadedChunks.Count; i++)
        {
            if(!temp.Contains(lastLoadedChunks[i]))
            {
                //lastLoadedChunks[i].GetComponent<Chunk>().ReloadChunk(false);
                lastLoadedChunks[i].SetActive(false);
                Debug.Log("unloaded chunk: " + lastLoadedChunks[i].name);
            }
        }
        return temp;
    }

    public void PlaceBlock(int x, int y, int z, int blockId)
    {
        int chunkX = x / chunkSize;
        int chunkY = y / chunkSize;
        int chunkZ = z / chunkSize;

        blocks[x, y, z] = blockId;
        chunks[chunkX, chunkY, chunkZ].GetComponent<Chunk>().blocks[x - chunkX * chunkSize, y - chunkY * chunkSize, z - chunkZ * chunkSize] = blockId;
        chunks[chunkX, chunkY, chunkZ].GetComponent<ChunkMesh>().GenMesh();
    }

    public void DestroyBlock(int x, int y, int z, int blockId)
    {
        int chunkX = x / chunkSize;
        int chunkY = y / chunkSize;
        int chunkZ = z / chunkSize;

        int oldBlock = blocks[x, y, z];

        blocks[x, y, z] = -1;
        chunks[chunkX, chunkY, chunkZ].GetComponent<Chunk>().blocks[x - chunkX * chunkSize, y - chunkY * chunkSize, z - chunkZ * chunkSize] = -1;
        chunks[chunkX, chunkY, chunkZ].GetComponent<ChunkMesh>().GenMesh();

        for (int i = 0; i < neighborsFacesDir2.Length; i++)
        {
            int neighborBlockX = x + neighborsFacesDir2[i][0];
            int neighborBlockY = y + neighborsFacesDir2[i][1];
            int neighborBlockZ = z + neighborsFacesDir2[i][2];

            //if neighborBlock is within map bounds
            if(neighborBlockX >= 0 && neighborBlockX < sizes[0] && neighborBlockY >= 0 && neighborBlockY < sizes[1] && neighborBlockZ >= 0 && neighborBlockZ < sizes[2])
            {
                int neighborBlockChunkX = neighborBlockX / chunkSize;
                int neighborBlockChunkY = neighborBlockY / chunkSize;
                int neighborBlockChunkZ = neighborBlockZ / chunkSize;

                //if neighbor block is in another chunk
                if (chunkX != neighborBlockChunkX || chunkY != neighborBlockChunkY || chunkZ != neighborBlockChunkZ)
                {
                    chunks[chunkX + neighborsFacesDir2[i][0], chunkY + neighborsFacesDir2[i][1], chunkZ + neighborsFacesDir2[i][2]].GetComponent<ChunkMesh>().GenMesh();
                }
            }
        }
        //spawn block item
        GameObject item = Instantiate((GameObject)Resources.Load("Prefabs/CubeItem", typeof(GameObject)), new Vector3(x, y, z), Quaternion.identity);
        item.GetComponent<CubeMesh>().id = oldBlock;
        item.GetComponent<ItemPickup>().id = oldBlock+1;
        GetComponent<ItemsManager>().AddItem(oldBlock+1, 1, item);
    }

    private void OnGUI()
    {
        /*if (GUI.Button(new Rect(150, 10, 100, 100), "Gen2") && !isGenerating)
        {
            isGenerating = true;
            if (!observationMode)
            {
                Destroy(player);
                cam.SetActive(true);
            }
            ResetMap();
            GenMap();
            BuildMap();
        }*/

        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "CubicMapGen")
        {
            if (GUI.Button(new Rect(10, 10, 100, 100), "Gen Map V2.2") && !isGenerating)
            {
                isGenerating = true;
                if (!observationMode)
                {
                    Destroy(player);
                    cam.SetActive(true);
                }
                ResetMap();
                string seed = "";
                for (int i = 0; i < 4; i++)
                {
                    string temp = Random.Range(0, 10000).ToString();
                    while (temp.Length < 4)
                    {
                        temp = "0" + temp;
                    }
                    seed += temp;
                }
                if (GenMap(sizes, seed))
                {
                    Debug.Log("Map gen done");
                };
                BuildMap2();
                isGenerating = false;
            }

            /*int baseOffsetX = 550;
            int baseOffsetY = 10;
            int btnWidth = 90;
            int btnHeight = 30;
            int linesOffset = 10;
            int elementsOffset = 10;

            GUI.Box(new Rect(Screen.width - baseOffsetX, 10, 200, 200), "");
            GUI.Label(new Rect(Screen.width - baseOffsetX + elementsOffset, 10 + linesOffset, 200, 200), "Map Size:");

            for (int i = 0; i < 4; i++)
            {
                int tempSize = 128 * Mathf.FloorToInt(Mathf.Pow(2, i));

                if (GUI.Button(new Rect(Screen.width - baseOffsetX + (i + 2) * elementsOffset + (i + 1) * btnWidth, baseOffsetY + linesOffset * 1, btnWidth, btnHeight), tempSize + "x" + tempSize))
                {
                    sizes[0] = tempSize;
                    sizes[2] = tempSize;
                }
            }*/

        }
        
       /* if (GUI.Button(new Rect(10, 10, 100, 100), "Refresh Meshes") && !isGenerating)
        {
            isGenerating = true;
            for (int x = 0; x < chunkSizes[0]; x++)
            {
                for (int y = 0; y < chunkSizes[1]; y++)
                {
                    for (int z = 0; z < chunkSizes[2]; z++)
                    {
                        chunks[x, y, z].GetComponent<ChunkMesh>().GenMesh();
                    }
                }
            }
            isGenerating = false;
        }*/
    }
}
