using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesManager : MonoBehaviour {

    public List<GameObject> entities;
    public List<GameObject> passiveMobs;
    public List<GameObject> hostileMobs;
    public List<GameObject> neutralMobs;

    [SerializeField]
    private GameObject player;

    public void AddEntity(GameObject ent)
    {
        if(!entities.Contains(ent))
        {
            entities.Add(ent);
        }
        if(ent.tag == "Mobs")
        {
            if(ent.GetComponent<Mob>().mobType == 0)
            {
                if (!passiveMobs.Contains(ent))
                {
                    passiveMobs.Add(ent);
                }
            }
            else if (ent.GetComponent<Mob>().mobType == 1)
            {
                if (!hostileMobs.Contains(ent))
                {
                    hostileMobs.Add(ent);
                }
            }
            else if (ent.GetComponent<Mob>().mobType == 2)
            {
                if (!neutralMobs.Contains(ent))
                {
                    neutralMobs.Add(ent);
                }
            }
        }
    }

    public void RemoveEntity(GameObject ent)
    {
        if (entities.Contains(ent))
        {
            entities.Remove(ent);
        }
    }

    public void ClearEntities()
    {
        entities.Clear();
        passiveMobs.Clear();
        hostileMobs.Clear();
        neutralMobs.Clear();
    }

    public void RefreshEntities()
    {
        ClearEntities();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Mobs");
        for (int i = 0; i < temp.Length; i++)
        {
            AddEntity(temp[i]);
        }
        if(GameObject.FindGameObjectWithTag("Player"))
        {
            AddEntity(GameObject.FindGameObjectWithTag("Player"));
        }
    }

    public void SpawnEntity(int id, int mode)
    {
        bool canSpawn = false;
        Vector3 spawnPos;
        while(!canSpawn)
        {
            spawnPos = new Vector3(Random.Range(0, 128), 0, Random.Range(0, 128));
            if(Vector3.Angle(player.transform.TransformDirection(transform.forward), spawnPos) < 60)
            {
                //a refaire
            }
        }
    }

    public void SpawnEntityAt(int id, int mode, Vector3 pos)
    {

    }
}
