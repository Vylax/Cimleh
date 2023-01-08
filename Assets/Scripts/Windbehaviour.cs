using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windbehaviour : MonoBehaviour {

    int[] windSetting = new int[2]; //Génère un array qui acceuillera les paramètre du vent

    [SerializeField]
    GameObject generator; //Game object qui possède les valeurs global du vent (force & direction)

    [SerializeField]
    int dir; //La direction stoqué du vent
    int input; //Variable temporaire
    [SerializeField]
    float power; //Puissance du vent
    [SerializeField]
    int palmeLevel; //le niveau de la palme qui influera sur le rendement finale
    [SerializeField]
    int bobineLevel; //le niveau de la bobine qui influera également sur le rendement final
    float uptade = 0; //Variable permettant de mêtre à jour toutes les 10sec les paramètres du vents
    [SerializeField]
    float energie; //L'énergie renvoyer par l'éolienne
    [SerializeField]
    GameObject pivot;

    float[] nPalme = new float[3]; //L'array contenant les rendements des trois niveaux de palmes
    float[] nBobine = new float[3]; //L'array contenant les rendements des trois niveaux de bobine

    Vector3 transforme;

    // Use this for initialization
    void Start()
    {
        nPalme[0] = 0.33f;
        nPalme[1] = 0.5f;
        nPalme[2] = 0.6f;
        nBobine[0] = 0.33f;
        nBobine[1] = 0.5f;
        nBobine[2] = 0.6f;
    }

    // Update is called once per frame
    void Update() {
        if (Time.time - uptade > 9)
        {
            MAJ();
            uptade = Time.time;
        }

        if (power > 2)
        {//On vérifie qu'il y ait du vent pour éviter que l'éolienne ne produise de l'énergie dans le vide 
            if ((transform.rotation.eulerAngles.z == 0 || transform.rotation.eulerAngles.z == 180) && (dir == 0 || dir == 2))
            {
                energie = 1.875f * Mathf.Pow(power, 2) - 16.75f * power + 43;

                energie = energie * nPalme[palmeLevel] * nBobine[bobineLevel];
                energie = Mathf.Round(energie);
                Vector3 transform = new Vector3(0, 0, power / 8.5f * 1 * Time.deltaTime);
                if (dir == 0)
                {
                    Vector3 transforme = new Vector3(0, 0, power / 8.5f * 100 * Time.deltaTime);
                }
                else
                {
                    Vector3 transforme = new Vector3(0, 0, power / 8.5f * -100 * Time.deltaTime);
                }
            }
            else if ((transform.rotation.eulerAngles.z == 90 || transform.rotation.eulerAngles.z == 270) && (dir == 1 || dir == 3)) //On vérifie que le vent soit orienté dans la bonne direction par rapport aux palmes
            {

                energie = 1.875f * Mathf.Pow(power, 2) - 16.75f * power + 43;
                energie = energie * nPalme[palmeLevel] * nBobine[bobineLevel];
                energie = Mathf.Round(energie);
                Vector3 transform = new Vector3(0, 0, power / 8.5f * 1 * Time.deltaTime);
                if (dir==1)
                {
                    Vector3 transforme = new Vector3(0, 0, power / 8.5f * 100 * Time.deltaTime);
                }
                else
                {
                    Vector3 transforme = new Vector3(0, 0, power / 8.5f * -100 * Time.deltaTime);
                }
            }
            pivot.transform.Rotate(transforme);
        }
    }


    void MAJ()
        {
            windSetting = generator.GetComponent<WindGenerator>().getWindsetting();
            dir = windSetting[0]; //0=N 1=O 2=S 3=E
            input = windSetting[1];
            power = (int)input;
            power = power / 1000;

         }
}


