using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

public class EditorMobSpawn : ScriptableWizard
{
    public int mobsToSpawn = 5;
    private Vector3 spawnPos = new Vector3(64, 64, 64);
    private bool autoChangeHeight = true;
    public bool random = true;

    [MenuItem("My Tools/Mob Spawn")]
    static void EditorMobSpawnWizard()
    {
        ScriptableWizard.DisplayWizard<EditorMobSpawn>("Mob Spawn", "Spawn", "Spawn Hunter");
    }

    void OnWizardCreate()
    {
        for (int i = 0; i < mobsToSpawn; i++)
        {
            SpawnMob();
        }
        GameObject.Find("MapGenV2").GetComponent<EntitiesManager>().RefreshEntities();
    }

    private void OnWizardOtherButton()
    {
        for (int i = 0; i < mobsToSpawn; i++)
        {
            SpawnMob(true);
        }
        GameObject.Find("MapGenV2").GetComponent<EntitiesManager>().RefreshEntities();
    }

    void SpawnMob(bool hunter = false)
    {
        Vector3 temp = spawnPos;
        if (random)
        {
            temp = new Vector3(Random.Range(32, 96), 0, Random.Range(32, 96));
        }
        if (autoChangeHeight)
        {
            for (int y = 63; y > 0; y--)
            {
                if (GameObject.Find("MapGenV2").GetComponent<MapGen2>().blocks[(int)temp.x, y, (int)temp.z] != -1)
                {
                    temp.y = y + 1;
                    break;
                }
            }
        }
        GameObject mob = Instantiate((GameObject)Resources.Load("Prefabs/Mob", typeof(GameObject)), temp, Quaternion.identity);
        if(hunter)
        {
            mob.GetComponent<Mob>().mobType = 1;
            mob.GetComponent<CubeMesh>().id = 101;
            mob.GetComponent<Mob>().sightRange = 30f;
            mob.GetComponent<Mob>().viewAngle = 60f;
        }
        else
        {
            mob.GetComponent<CubeMesh>().id = 100;
        }
    }
}
#endif