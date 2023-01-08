using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGen : MonoBehaviour {

    public int size = 6;

    public int woodType = 0;//0 = oak

    //prefabs
    public GameObject[] WoodBlock;
    public GameObject[][] WoodBranch;
    public GameObject[] Leaves;

    //children
    public GameObject[] BodyParts;
    public List<GameObject> LeavesParts;

    private bool bodyGenerated = false;
    private bool leavesGenerated = false;

    //leaves Gen
    public float maxChance = 1f;
    private Vector3[] leavesDir = new Vector3[4];

    public bool isGenerating = false;
    public bool firstGen = true;

    private void Start()
    {
        BodyParts = new GameObject[size];
        leavesDir[0] = Vector3.right;
        leavesDir[1] = Vector3.left;
        leavesDir[2] = Vector3.forward;
        leavesDir[3] = Vector3.back;

        ////temppppp
        isGenerating = true;
        bodyGenerated = false;
        leavesGenerated = false;
        if (!firstGen)
        {
            Collapse();
        }
        GenBody();
    }

    void GenBody()
    {
        for (int i = 0; i < size; i++)
        {
            Vector3 temp = transform.position + Vector3.up * i;
            BodyParts[i] = Instantiate(WoodBlock[woodType], temp, Quaternion.identity);
            BodyParts[i].tag = "Map";
            BodyParts[i].transform.parent = this.transform;
        }
        bodyGenerated = true;
        //temp, remove when testing done
        GenLeaves();
    }

    void GenLeaves()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 temp = BodyParts[size - 1].transform.position + Vector3.down * i;
            SetupLeaves(temp, Vector3.right, temp, maxChance / (Mathf.Abs(2.5f - i) + 1));
            SetupLeaves(temp, Vector3.left, temp, maxChance / (Mathf.Abs(2.5f - i) + 1));
            SetupLeaves(temp, Vector3.forward, temp, maxChance / (Mathf.Abs(2.5f - i) + 1));
            SetupLeaves(temp, Vector3.back, temp, maxChance / (Mathf.Abs(2.5f - i) + 1));
        }
        leavesGenerated = true;
        firstGen = false;
        isGenerating = false;
    }

    void SetupLeaves (Vector3 basePos, Vector3 dir, Vector3 origin, float chance)
    {
        float rdm = Random.Range(0f, 1f);
        if(rdm <= chance && basePos+dir != origin)
        {
            GameObject tempObj = Instantiate(Leaves[woodType], basePos + dir, Quaternion.identity);
            tempObj.tag = "Map";
            tempObj.transform.parent = this.transform;
            LeavesParts.Add(tempObj);
            for (int i = 0; i < leavesDir.Length; i++)
            {
                if(-dir != leavesDir[i])
                {
                    SetupLeaves(basePos + dir, leavesDir[i], origin, chance*3f/4f);
                }
            }
        }
        else
        {
            return;
        }
    }

    public void Collapse ()
    {
        for (int i = 0; i < BodyParts.Length; i++)
        {
            Destroy(BodyParts[i]);
        }
        for (int i = 0; i < LeavesParts.Count; i++)
        {
            Destroy(LeavesParts[i]);
        }
        Destroy(this.gameObject);
    }
}
