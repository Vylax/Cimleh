using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPause : MonoBehaviour
{

    bool isInMenu = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            isInMenu = !isInMenu;
        }
    }
    private void OnGUI()
    {
        if (isInMenu == true) {
            GUI.Box(new Rect(Screen.width / 8, Screen.height / 8, Screen.width * 6/8, Screen.height * 6/8), "");
            if (GUI.Button(new Rect(Screen.width * 3/8, Screen.height * 1/5, Screen.width * 6/24, Screen.height * 3/20), "Resume"))
            {
                isInMenu = false;
            }
            if (GUI.Button(new Rect(Screen.width * 3 / 8, Screen.height * 17 / 40, Screen.width * 6 / 24, Screen.height * 3 / 20), "Options"))
            {
                //Quand jordan sera inspi
            }
            if (GUI.Button(new Rect(Screen.width * 3 / 8, Screen.height * 13 / 20, Screen.width * 6 / 24, Screen.height * 3 / 20), "Menu Principal"))
            {
                //Quand jordan sera inspi
            }
        }
    }
}
