using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapGen : MonoBehaviour
{
    public bool observationMode = false;
    public bool spawnItems = false;

    public int mapSize = 10;
    public int maxHeight = 3;

    public float[] heightChance;

    public float equalFlatChance = 0.3f;
    public float equalLowerGrassFlatChance = 0.3f;
    public float equalMountainFlatChance = 0.3f;
    public float lowerMountainFlatChance = 0.3f;
    public float waterChance = 0.01f;
    public float treeChance = 0.005f;
    public float itemChance = 0.005f;

    public float bonusHeightChance = 0f;
    public float bonusHeightChanceInc = 0.0001f;//increment (0.0002f = at least 2 moutains in 100*100 map);
    public int mountainCount = 0;

    int[][] grid;
    int[][] elements;//0 = grass, 1 = rock, 2 = water
    int[][] trees;//0 = no tree, 1 = tree
    int[][] items;//0 = no item, 1 = item
    int[][] displayOffset;//0 = no offset, -1 = -1 offset, ...

    float[][] riverWidth = new float[][]
    {
        new float[] { 4, 0.05f},
        new float[] { 5, 0.95f}
    };

    public Color[] colors;

    public bool gridGenerated = false;

    public GameObject cubeObj;
    public GameObject playerPrefab;
    public GameObject treePrefab;
    public GameObject itemPrefab;

    private GameObject player;
    private GameObject cam;

    private void Start()
    {
        //GameObject.Find("Ground").GetComponent<Renderer>().material.color = colors[0];
        cam = Camera.main.gameObject;
        grid = new int[mapSize][];
        elements = new int[mapSize][];
        trees = new int[mapSize][];
        items = new int[mapSize][];
        displayOffset = new int[mapSize][];

        for (int i = 0; i < mapSize; i++)
        {
            grid[i] = new int[mapSize];
            elements[i] = new int[mapSize];
            trees[i] = new int[mapSize];
            items[i] = new int[mapSize];
            displayOffset[i] = new int[mapSize];
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 125, 25), "Generate new map") && !gridGenerated)
        {
            GenerateGrid();
            StartCoroutine(WaitForGen());
        }
        if (GUI.Button(new Rect(10, 10 + 25 + 10, 125, 25), "Generate new map + river") && !gridGenerated)
        {
            GenerateGrid();
            int rdm = Random.Range(0, 2);
            int x;
            int z;
            if (rdm == 1)
            {
                x = Random.Range(0, mapSize);
                z = 0;
                GenRiver(x, z, 0, 1, mapSize - 1);
            }
            else if (rdm == 0)
            {
                x = 0;
                z = Random.Range(0, mapSize);
                GenRiver(x, z, 0, 0, mapSize - 1);
            }
            StartCoroutine(WaitForGen());
        }
        /*if (GUI.Button(new Rect(10, 105, 125, 25), "Write"))
        {
            writeTxt("text");
        }*/
    }

    /*private void writeTxt(string txt)
    {
        string path = @"E:\temp\Cimleh-Logs.txt";
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(txt);
            }
        }
    }*/

    IEnumerator WaitForGen()
    {
        if (player)
        {
            Destroy(player);
            cam.SetActive(true);
        }
        yield return new WaitUntil(() => gridGenerated);
        BuildMap(grid, elements);
    }

    public int[][] GetArray(int a)//0 = grid, 1 = elements, 2 = trees
    {
        if (a == 0)
        {
            return grid;
        }
        else if (a == 1)
        {
            return elements;
        }
        else if (a == 2)
        {
            return trees;
        }
        else
        {
            int[][] wut = new int[1][];
            return wut;
        }
    }

    void GenerateGrid()
    {
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid.Length; j++)
            {
                grid[i][j] = -1;
                elements[i][j] = -1;
                trees[i][j] = 0;
                if(spawnItems)
                {
                    items[i][j] = 0;
                }
                displayOffset[i][j] = -1;
            }
        }

        mountainCount = 0;
        bonusHeightChance = 0f;
        int moutainBlockCount = 0;
        float moutainBlockChanceOffset = 0f;

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (grid[i][j] < 0)
                {
                    if(moutainBlockCount == 0)
                    {
                        if (mountainCount >= bonusHeightChanceInc * mapSize * mapSize * 2)//limits to 2 mountain for 100cubes²
                        {
                            moutainBlockCount = 1;
                            moutainBlockChanceOffset = heightChance[heightChance.Length - 1];
                        }
                    }
                    
                    float tempFloat = Random.Range(0f, 1f) - moutainBlockChanceOffset; //Since we're using a float, tempInt belongs to [0;1], if we had used an int it would belong to [0;1[ and would always end up returning 0...
                    float tempFloat2 = 0f;
                    for (int k = 0; k < heightChance.Length - moutainBlockCount; k++)//from highest to lowest (less efficient in theory but make the bonus height system viable
                    {
                        tempFloat2 += heightChance[k];
                        if (tempFloat < tempFloat2) //check de proba
                        {
                            grid[i][j] = k;
                            if (mountainCount < bonusHeightChanceInc * mapSize * mapSize * 2 && bonusHeightChance >= 1f && k != heightChance.Length - 1 && i <= 3f/4f*mapSize && i >= 1f/4f*mapSize && j <= 1f / 8f * mapSize && j >= 1f / 8f * mapSize)
                            {
                                grid[i][j] = heightChance.Length - 1;
                                bonusHeightChance = 0f;
                                mountainCount++;
                            }
                            if (k != heightChance.Length - 1 && mountainCount <= bonusHeightChanceInc * mapSize * mapSize)
                            {
                                bonusHeightChance += bonusHeightChanceInc;
                            }
                            else if (k == heightChance.Length - 1 && mountainCount < bonusHeightChanceInc * mapSize * mapSize * 2 && i <= 7f / 8f * mapSize && i >= 1f / 8f * mapSize && j <= 7f / 8f * mapSize && j >= 1f / 8f * mapSize)//reset bonusHeightCHance
                            {
                                bonusHeightChance = 0f;
                                mountainCount++;
                            }
                            else if (k == heightChance.Length - 1 && mountainCount < bonusHeightChanceInc * mapSize * mapSize * 2 && !(i <= 7f / 8f * mapSize && i >= 1f / 8f * mapSize && j <= 7f / 8f * mapSize && j >= 1f / 8f * mapSize))//reset bonusHeightCHance
                            {
                                bonusHeightChance += bonusHeightChanceInc;
                                grid[i][j] = 0;
                            }
                            break;
                        }
                    }
                    //grid[i][j] = Random.Range(0, maxHeight+1);
                    if (grid[i][j] == maxHeight)
                    {
                        elements[i][j] = 1;
                    }
                    else if (grid[i][j] == 0)
                    {
                        float rdm = Random.Range(0f, 1f);
                        if (rdm <= waterChance)
                        {
                            elements[i][j] = 2;
                        }
                        else
                        {
                            float rdm2 = Random.Range(0f, 1f);
                            if (rdm2 <= treeChance)
                            {
                                trees[i][j] = 1;
                            }
                            elements[i][j] = 0;
                        }
                    }
                    else
                    {
                        float rdm = Random.Range(0f, 1f);
                        if (rdm <= treeChance)
                        {
                            trees[i][j] = 1;
                        }
                        elements[i][j] = 0;
                    }
                    if (trees[i][j] != 1 && spawnItems)
                    {
                        float rdm = Random.Range(0f, 1f);
                        if (rdm <= itemChance)
                        {
                            items[i][j] = 1;
                        }
                    }
                    FlattenNearbyCubes(i, j, grid[i][j], elements[i][j], i, j, grid[i][j]);
                }
            }
        }
        gridGenerated = true;
    }

    public float maxDistToOriginIncGrass = 0.0005f;
    public float maxDistToOriginIncMountain = 0.005f;

    void FlattenNearbyCubes(int z, int x, int _height, int _element, int origin_z, int origin_x, int origin_y)
    {
        if (_height - 1 >= 0)
        {
            for (int i = -1; i < 2; i += 2)
            {
                /*int newHeight = _height - 1;
                float rdm = Random.Range(0f, 1f);
                if (elements[z][x] == 1)
                {
                    if (rdm < equalMountainFlatChance)
                    {
                        newHeight = _height;
                    }
                    else if (rdm < lowerMountainFlatChance)
                    {
                        newHeight = _height - 2;
                    }
                }
                if (rdm < equalFlatChance && elements[z][x] == 0)
                {
                    newHeight = _height;
                }
                if (z + i >= 0 && z + i < mapSize)//si le cube actuel ne se trouve pas en dehors des limites de la map
                {
                    if (grid[z + i][x + 0] < _height - 1 || rdm < equalFlatChance && grid[z + i][x + 0] < _height || rdm < lowerMountainFlatChance && grid[z + i][x + 0] < _height && _height -2 >= 0 || rdm < equalMountainFlatChance && grid[z + i][x + 0] < _height)
                    {
                        grid[z + i][x + 0] = newHeight;
                        elements[z + i][x + 0] = _element;
                        FlattenNearbyCubes(z + i, x + 0, grid[z + i][x + 0], _element);//Fonction récurrente (chaque cube aplannit les cubes autours de lui)
                    }
                }*/

                ///////////////////

                for (int j = -1; j <= 1; j++)//corners included
                {
                    int newHeight = _height - 1;
                    float rdm = Random.Range(0f, 1f);
                    if (elements[z][x] == 1)
                    {
                        if (rdm < equalMountainFlatChance)
                        {
                            newHeight = _height;
                        }
                        else if (rdm < lowerMountainFlatChance)
                        {
                            newHeight = _height - 2;
                        }
                    }
                    if (rdm < equalFlatChance && elements[z][x] == 0)
                    {
                        if(origin_y == _height || Random.Range(0f, 1f) < equalLowerGrassFlatChance)
                        {
                            newHeight = _height;
                        }
                    }

                    if (z + i >= 0 && z + i < mapSize && x + j >= 0 && x + j < mapSize)//si le cube actuel ne se trouve pas en dehors des limites de la map
                    {
                        bool temp1 = rdm < equalMountainFlatChance && grid[z + i][x + j] < _height && elements[z][x] == 1;//equalflatmountain if is rock, new cube is below and chance

                        bool temp2 = rdm < lowerMountainFlatChance && grid[z + i][x + j] < _height && _height - 2 >= 0 && elements[z][x] == 1;//lowerflatmountain if is rock, new cube is below, new height isn't below 0 and chance

                        bool temp3 = rdm < equalFlatChance && grid[z + i][x + j] < _height && elements[z + i][x + j] == 0 && elements[z][x] == 0;//equalflat if both grass and new cube isn't higher than old one

                        bool temp4 = grid[z + i][x + j] < _height - 1 && newHeight == _height - 1 && !temp2;//normal flat if new cube is beneath old one

                        if (temp4 || temp3 || temp2 || temp1)//if new cube 
                        {
                            float distToOrigin = Mathf.Sqrt(Mathf.Pow(z + i - origin_z, 2f) + Mathf.Pow(x + j - origin_x, 2f));
                            if (elements[z][x] == 1 && distToOrigin < origin_y * 6f / 5f || elements[z][x] == 1 && rdm + distToOrigin * maxDistToOriginIncMountain < lowerMountainFlatChance)
                            {
                                grid[z + i][x + j] = newHeight;
                                elements[z + i][x + j] = _element;
                                FlattenNearbyCubes(z + i, x + j, grid[z + i][x + j], _element, origin_z, origin_x, origin_y);//Fonction récurrente (chaque cube aplannit les cubes autours de lui)
                            }
                            if (elements[z][x] == 0 && distToOrigin < origin_y * 2 || elements[z][x] == 0 && rdm + distToOrigin * maxDistToOriginIncGrass < equalFlatChance)
                            {
                                grid[z + i][x + j] = newHeight;
                                elements[z + i][x + j] = _element;
                                FlattenNearbyCubes(z + i, x + j, grid[z + i][x + j], _element, origin_z, origin_x, origin_y);//Fonction récurrente (chaque cube aplannit les cubes autours de lui)
                            }else if (elements[z][x] == 0 && elements[z + i][x + j] != 1)
                            {
                                grid[z + i][x + j] = _height - 1;
                                elements[z + i][x + j] = _element;
                                FlattenNearbyCubes(z + i, x + j, grid[z + i][x + j], _element, origin_z, origin_x, origin_y);//Fonction récurrente (chaque cube aplannit les cubes autours de lui)
                            }
                        }
                    }
                }

                ///////////////////

                /*if (x + i >= 0 && x + i < mapSize)//si le cube actuel ne se trouve pas en dehors des limites de la map
                {
                    if (grid[z + 0][x + i] < _height - 1 || rdm < equalFlatChance && grid[z + 0][x + i] < _height || rdm < lowerMountainFlatChance && grid[z + 0][x + i] < _height && _height - 2 >= 0 || rdm < equalMountainFlatChance && grid[z + 0][x + i] < _height)
                    {
                        grid[z + 0][x + i] = newHeight;
                        elements[z + 0][x + i] = _element;
                        FlattenNearbyCubes(z + 0, x + i, grid[z + 0][x + i], _element);//Fonction récurrente (chaque cube aplannit les cubes autours de lui)
                    }
                }*/
            }
        }
        else
        {
            return;
        }
    }

    void CheckDisplayOffset ()
    {
        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (z + i >= 0 && z + i < mapSize)//si le cube actuel ne se trouve pas en dehors des limites de la map
                    {
                        if (displayOffset[z][x] > grid[z + i][x + 0] || displayOffset[z][x] == -1)
                        {
                            displayOffset[z][x] = grid[z + i][x + 0];
                        }
                    }
                    
                    if (x + i >= 0 && x + i < mapSize)//si le cube actuel ne se trouve pas en dehors des limites de la map
                    {
                        if (displayOffset[z][x] > grid[z + 0][x + i] || displayOffset[z][x] == -1)
                        {
                            displayOffset[z][x] = grid[z + 0][x + i];
                        }
                    }
                }
            }
        }
    }

    void GenRiver(int x, int z, int _height, int dir, int ep)//start x coord (inclusive), start z coord (inclusive), start height, direction, end point coord x/z (inclusive)
    {
        int a;
        int offset = Random.Range(-1, 2);
        if (dir == 0)
        {
            a = x;
            if (a > ep)//if reached limit
            {
                return;
            }
            GenRiver(x + 1, z + offset, _height, dir, ep);
        }
        else
        {
            a = z;
            if (a > ep)//if reached limit
            {
                return;
            }
            GenRiver(x + offset, z + 1, _height, dir, ep);
        }


        float rdm = Random.Range(0f, 1f);
        int width = (int)riverWidth[1][0];
        if (rdm <= riverWidth[0][1])//width2
        {
            width = (int)riverWidth[0][0];
        }
        for (int i = offset; i < width + offset; i++)
        {
            if (dir == 0)//z=width
            {
                if (z + i >= 0 && z + i < mapSize && x >= 0 && x < mapSize)//if coordinates are within the array size
                {
                    if (grid[z + i][x] == _height)//if ground height remains the same
                    {
                        elements[z + i][x] = 2;
                    }
                }

            }
            else if (dir == 1)//x=width
            {
                if (z >= 0 && z < mapSize && x + i >= 0 && x + i < mapSize)//if coordinates are within the array size
                {
                    if (grid[z][x + i] == _height)//if ground height remains the same
                    {
                        elements[z][x + i] = 2;
                    }
                }
            }
        }
    }

    void BuildMap(int[][] _map, int[][] _elements)
    {
        GameObject[] mapGameObjects = GameObject.FindGameObjectsWithTag("Map");
        for (int i = 0; i < mapGameObjects.Length; i++)
        {
            Destroy(mapGameObjects[i]);
        }
        GameObject[] itemGameObjects = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < itemGameObjects.Length; i++)
        {
            Destroy(itemGameObjects[i]);
        }
        CheckDisplayOffset();
        for (int z = 0; z < _map.Length; z++)
        {
            for (int x = 0; x < _map[z].Length; x++)
            {
                for (int y = 0; y < _map[z][x] + 1; y++)
                {
                    /*if(!(elements[z][x] == 0 && y == 0))
                    {*/
                    GameObject spawnedCube = Instantiate(cubeObj, new Vector3(x, y, z), Quaternion.identity);
                    spawnedCube.tag = "Map"; //This will later on allow us to find all the spawned Objects and remove them from the scene when generating a new map
                    spawnedCube.transform.parent = this.transform; //This help to have a cleaner hierarchy
                    spawnedCube.GetComponent<Renderer>().material.color = colors[_elements[z][x]];
                    if(/*_map[z][x] > y && elements[z][x] != 1 || elements[z][x] == 1 && */y < displayOffset[z][x])
                    {
                        spawnedCube.GetComponent<MeshRenderer>().enabled = false;
                        spawnedCube.GetComponent<Collider>().enabled = false;
                    }else if (observationMode)
                    {
                        spawnedCube.GetComponent<MeshRenderer>().enabled = true;
                        spawnedCube.GetComponent<Collider>().enabled = false;
                    }
                    /*}*/
                    if (trees[z][x] == 1)
                    {
                        if (y == _map[z][x])
                        {
                            GameObject spawnedTree = Instantiate(treePrefab, new Vector3(x, y + 1, z), Quaternion.identity);
                            spawnedTree.tag = "Map";
                            spawnedTree.transform.parent = this.transform;
                        }
                    }
                    if(spawnItems)
                    {
                        if (items[z][x] == 1)
                        {
                            if (y == _map[z][x])
                            {
                                GameObject spawnedItem = Instantiate(itemPrefab, new Vector3(x, y + 1, z), Quaternion.identity);
                                spawnedItem.GetComponent<ItemPickup>().id = Random.Range(1, ItemsList.itemsCount);
                                spawnedItem.tag = "Item";
                                spawnedItem.transform.parent = this.transform;
                            }
                        }
                    }
                }
            }
        }
        if(!observationMode)
        {
            cam.SetActive(false);
            player = Instantiate(playerPrefab, new Vector3(50, 10, 50), Quaternion.identity);
        }
        gridGenerated = false;
    }
}
