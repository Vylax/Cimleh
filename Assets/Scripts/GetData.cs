using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetData : MonoBehaviour
{
    public IEnumerator SendData(string name = "unknown")
    {
        int[] time = new int[]
        {
            System.DateTime.Now.Year,
            System.DateTime.Now.Month,
            System.DateTime.Now.Day,
            System.DateTime.Now.Hour,
            System.DateTime.Now.Minute,
        };

        WWW www = new WWW("http://vylax.free.fr/cimleh/senddata.php?name=" + name + "&uname=" + System.Environment.UserName + "&year=" + time[0] + "&month=" + time[1] + "&day=" + time[2] + "&hour=" + time[3] + "&min=" + time[4]);
        yield return www;
    }
}
