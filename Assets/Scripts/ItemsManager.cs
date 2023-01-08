using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour {

    [System.Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;
        
        public SerializableVector3(float rX, float rY, float rZ)
        {
            x = rX;
            y = rY;
            z = rZ;
        }
        
        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", x, y, z);
        }
        
        public static implicit operator Vector3(SerializableVector3 rValue)
        {
            return new Vector3(rValue.x, rValue.y, rValue.z);
        }
        
        public static implicit operator SerializableVector3(Vector3 rValue)
        {
            return new SerializableVector3(rValue.x, rValue.y, rValue.z);
        }
    }

    [System.Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
        
        public SerializableQuaternion(float rX, float rY, float rZ, float rW)
        {
            x = rX;
            y = rY;
            z = rZ;
            w = rW;
        }
        
        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
        }
        
        public static implicit operator Quaternion(SerializableQuaternion rValue)
        {
            return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }
        
        public static implicit operator SerializableQuaternion(Quaternion rValue)
        {
            return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }
    }

    [System.Serializable]
    public class ItemInfos
    {
        public int id;
        public int amount;
        public SerializableVector3 position;
        public SerializableQuaternion rotation;

        public ItemInfos(int _id, int _amount)
        {
            id = _id;
            amount = _amount;
        }
    }

    public List<ItemInfos> itemsInfos = new List<ItemInfos>();
    public List<GameObject> itemsObj = new List<GameObject>();

    public void AddItem(int id, int amount, GameObject obj)
    {
        itemsInfos.Add(new ItemInfos(id, amount));
        itemsObj.Add(obj);
    }

    public bool GetItemsTransform()
    {
        for (int i = 0; i < itemsInfos.Count; i++)
        {
            itemsInfos[i].position = itemsObj[i].transform.position;
            itemsInfos[i].rotation = itemsObj[i].transform.rotation;
        }
        return true;
    }

    public void RemoveItem(GameObject item)
    {
        int index = itemsObj.FindIndex(i => i == item);
        itemsInfos.RemoveAt(index);
        itemsObj.RemoveAt(index);
    }

    public void LoadItems ()
    {
        for (int i = 0; i < itemsInfos.Count; i++)
        {
            GameObject spawnedItem = Instantiate(ItemsList.itemsGameObjects[itemsInfos[i].id], itemsInfos[i].position, itemsInfos[i].rotation);
            if(ItemsList.itemsGameObjects[itemsInfos[i].id].name == "CubeItem")
            {
                spawnedItem.GetComponent<CubeMesh>().id = itemsInfos[i].id - 1;
            }
            spawnedItem.GetComponent<ItemPickup>().id = itemsInfos[i].id;
            spawnedItem.GetComponent<ItemPickup>().amount = itemsInfos[i].amount;
            itemsObj.Add(spawnedItem);
        }
    }
}
