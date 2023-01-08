using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPannel : MonoBehaviour {

    float cd;
    [SerializeField]
    float temps;
    [SerializeField]
    float Tmax;
    [SerializeField]
    float Eproduit;

    [SerializeField]
    int Emaxlvl = 0;
    [SerializeField]
    int Eminlvl = 0;
    [SerializeField]
    int Rendementlvl = 0;
    [SerializeField]
    int arrondilvl = 0;

    float[] Emax = new float[3];
    float[] Emin = new float[3];
    float[] rendement = new float[3];
    float[] arrondi = new float[3];

    // Use this for initialization
    void Start()
    {
        GetEnergy();
        Emax[0] = 30;
        Emax[1] = 60;
        Emax[2] = 75;

        Emin[0] = 5;
        Emin[1] = 10;
        Emin[2] = 13;

        rendement[0] = 0.3f;
        rendement[1] = 0.5f;
        rendement[2] = 0.6f;

        arrondi[0] = 1;
        arrondi[1] = 0.5f;
        arrondi[2] = 0.25f;
    }
        // Update is called once per frame
        void Update () {
        if (Time.deltaTime - cd < 1)
        {
            GetEnergy();
        }
	}

    void GetEnergy()
    {
        Eproduit = ((Mathf.Abs(arrondi[arrondilvl] * Mathf.Sin(temps * 2 * Mathf.PI / (2f * Tmax) + (4 * Mathf.PI * Tmax / 2f)/24f - Mathf.PI)) + (1- arrondi[arrondilvl])) * Emax[Emaxlvl] + Emin[Eminlvl]) * rendement[Rendementlvl];
        cd = Time.deltaTime;
    }
}