using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGen : MonoBehaviour {

    public GameObject tree;

    public List<GameObject> trees;

    public bool isGenerating = false;

    //temporary, remove when testing is done
    private void OnGUI()
    {
        //if (GUI.Button(new Rect(10, 70, 100, 25), "Regen tree") && !isGenerating)
        if (GUI.Button(new Rect(10, 70, 100, 25), "Gen forest") && !isGenerating)
        {
            isGenerating = true;
            ResetMap();
            GenForest();
        }
    }

    void ResetMap ()
    {
        /*for (int i = 0; i < trees.Count; i++)
        {
            Destroy(trees[i]);
        }*/
        GameObject[] mapGameObjects = GameObject.FindGameObjectsWithTag("Map");
        for (int i = 0; i < mapGameObjects.Length; i++)
        {
            Destroy(mapGameObjects[i]);
        }
    }

    void GenForest()
    {
        for (int i = 5; i <= 95; i+=5)
        {
            for (int j = 5; j <= 95; j+=5)
            {
                GameObject temp = Instantiate(tree, new Vector3(i, 2, j), Quaternion.identity);
                trees.Add(temp);
            }
        }
        isGenerating = false;
    }
}
