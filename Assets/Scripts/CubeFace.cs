using UnityEngine;

public class CubeFace : MonoBehaviour {

    private int health = 100;

    //public bool hb = false;
    //public bool ds = false;

    public int Health(int value = int.MinValue)//no value in parameter = get, otherwise set
    {
        if(value != int.MinValue)//set
        {
            health = value;
        }
        return health;//return health value in both cases
    }
}
