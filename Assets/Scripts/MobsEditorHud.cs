using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

public class MobsEditorHud : ScriptableWizard
{

    public bool displayLookDirection = false;
    public bool displayFOV = false;
    public bool displayLineToPlayer = false;

    [MenuItem("My Tools/Mobs HUD")]
    static void MobsEditorHudWizard()
    {
        ScriptableWizard.DisplayWizard<MobsEditorHud>("Mobs HUD", "Update");
    }

    void OnWizardCreate()
    {
        if(Selection.activeTransform != null)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Selection.gameObjects[i].GetComponent<Mob>().displayLookDirection = displayLookDirection;
                Selection.gameObjects[i].GetComponent<Mob>().displayFOV = displayFOV;
                Selection.gameObjects[i].GetComponent<Mob>().displayLineToPlayer = displayLineToPlayer;
            }
        }
        else
        {
            GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mobs");
            for (int i = 0; i < mobs.Length; i++)
            {
                mobs[i].GetComponent<Mob>().displayLookDirection = displayLookDirection;
                mobs[i].GetComponent<Mob>().displayFOV = displayFOV;
                mobs[i].GetComponent<Mob>().displayLineToPlayer = displayLineToPlayer;
            }
        }
    }
}
#endif