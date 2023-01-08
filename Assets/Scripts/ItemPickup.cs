using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

    private GameObject player;
    [SerializeField]
    private GameObject silhouetteObj;

    public float pickUpRange = 2.5f;

    public int id = 1;
    public int amount = 1;
    
    public bool pickable = false;

    private void Start()
    {
        //id = Random.Range(1, ItemsList.itemsCount);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private Ray ray;
    private RaycastHit hit;

    private void Update()
    {
        if(!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if(player)//if isn't too far from player and there's a player character in the scene (this prevent overusage of cpu)
        {
            if(Vector3.Distance(player.transform.position, transform.position) <= pickUpRange)//if is within pickup range
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if(hit.collider.gameObject.GetInstanceID() == this.gameObject.GetInstanceID())//if cursor is over this item
                    {
                        pickable = true;
                    }
                    else
                    {
                        pickable = false;
                    }
                }
                else
                {
                    pickable = false;
                }
            }
            else
            {
                pickable = false;
            }
        }
        else
        {
            pickable = false;
        }
        if(pickable)
        {
            silhouetteObj.GetComponent<MeshRenderer>().enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUp();
            }
        }
        else
        {
            silhouetteObj.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnGUI()
    {
        if(pickable)
        {
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height - 60, 200, 50), "Press E to pickup " + ItemsList.itemsNames[id]);
        }
    }

    private void PickUp ()
    {
        pickable = false;
        player.GetComponent<Inventory>().PickUpItem(id, amount, this.gameObject);
    }
}
