using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OC : MonoBehaviour {

    //public float fov = 75f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Map")
        {
            other.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Map")
        {
            other.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    /*private Vector3 rot;
    public LayerMask mask;

    private void Update()
    {
        if(rot != this.transform.eulerAngles)
        {
            RecalculateVisibility();
        }
        rot = this.transform.eulerAngles;
    }

    List<GameObject> visibleEntyties = new List<GameObject>();

    void RecalculateVisibility ()
    {
        Collider[] entytiesInFOV = Physics.OverlapSphere(transform.position + Vector3.forward*(fov-5), fov, mask);

        for (int i = 0; i < entytiesInFOV.Length; i++)
        {
            Transform target = entytiesInFOV[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < fov / 2f)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);
                RaycastHit hit;
                if (Physics.Raycast(transform.position, dirToTarget, out hit, distToTarget, mask))
                {
                    if(hit.transform != target)
                    {
                        Debug.Log(hit.transform.name);
                        target.gameObject.GetComponent<Renderer>().enabled = false;
                    }
                    else
                    {
                        target.gameObject.GetComponent<Renderer>().enabled = true;
                    }
                }
            }
        }
    }*/
}
