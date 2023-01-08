using UnityEngine;

public class Cube : MonoBehaviour {

    //public int type;

    public GameObject[] faces = new GameObject[6];//0 = front, 1 = back, 2 = top, 3 = bottom, 4 = right, 5 = left

    public bool[] hitBoxes = new bool[6];
    //public bool[] displayState = new bool[6];

    public bool hbState = false;

    //public Texture2D[] texture = new Texture2D[6];

    //public int[] healths = new int[6];

    /*public void EditFace(int id, bool hb, bool ds, Texture2D t, int h = int.MinValue)
    {
        faces[id].GetComponent<Collider>().enabled = hb;
        faces[id].GetComponent<Renderer>().enabled = ds;
        faces[id].GetComponent<Renderer>().material.SetTexture(0, t);
        faces[id].GetComponent<CubeFace>().Health(h);
    }*/

    public void EditHitBox(int id, bool value)
    {
        faces[id].GetComponent<Collider>().enabled = value;
        hitBoxes[id] = value;
        if(value && !hbState)
        {
            hbState = true;
        }
        //faces[id].SetActive(value);
    }

    public void EditDisplayState(int id, bool value)
    {
        faces[id].GetComponent<Renderer>().enabled = value;
        //displayState[id] = value;
        //faces[id].SetActive(value);
    }

    public void EditFace(int id, bool value)
    {
        EditHitBox(id, value);
        EditDisplayState(id, value);
    }

    /*public bool CheckHBState()
    {
        for (int i = 0; i < hitBoxes.Length; i++)
        {
            if(hitBoxes[i])
            {
                return true;
            }
        }
        return false;
    }*/
}
