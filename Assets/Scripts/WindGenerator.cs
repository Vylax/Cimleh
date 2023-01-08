using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindGenerator : MonoBehaviour
{

    float uptade = -1;

    public int dir;
    public int power;

    int cld = 0;

    // Use this for initialization
    void Start()
    {
        windChange();
    }

    // Update is called once per frame
    void Update()
    {
        // On définie une mise à jour du vent toutes les 5 minutes (soit 300sec)
        if (Time.time - uptade > cld)
        {
            windChange();
            cld = Random.Range(250, 400);
        }
    }

    void windChange()
    {
        dir = Random.Range(0, 4);
        power = Random.Range(2500, 8500);
        uptade = Time.time;

    }
    public int[] getWindsetting()
    {
        int[] a = new int[2];
        a[0] = dir; //0=N 1=O 2=S 3=E
        a[1] = power;
        return a;
    }
}