using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaisonGenerator : MonoBehaviour
{

    public GameObject maison;
    bool CanBuild;
    public int essaie = 50;

    // Use this for initialization



    Vector2 RandomCord()
    {
        int x = Random.Range(0, 100);
        int z = Random.Range(0, 100);
        Vector2 vect = new Vector2(x, z);
        return (vect);
    }
    void Generate()
    {
        Vector2 cord = RandomCord();
        float x_ = cord.x;
        float z_ = cord.y;
        int x = (int)x_;
        int z = (int)z_;
        CanBuild = true;
        essaie--;
        for (int a = 0; a <= 5; a++)
        {
            for (int b = 0; b <= 5; b++)
            {
                //int[][] hauteur_terrain = GetComponent<MapGen>().GetArray(0);
                //int[][] hauteur_plante = GetComponent<MapGen>().GetArray(2);
                if (GameObject.Find("MapGenerator").GetComponent<MapGen>().GetArray(0)[z][x] > 0 || GameObject.Find("MapGenerator").GetComponent<MapGen>().GetArray(2)[z][x] > 0)
                {
                    CanBuild = false;
                    break;
                }
            }
            if (!CanBuild)
            {
                break;
            }
        }

        if (essaie <= 0)
        {
            Debug.Log("Pas de place");
        }

        else if (!CanBuild)
        {
            Generate();
        }
        else
        {
            GameObject mais = Instantiate(maison, new Vector3(x, -2, z), Quaternion.identity);
        }
    }


    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 100, 100, 25), "Gen maison"))
        {
            Generate();
        }
    }
}