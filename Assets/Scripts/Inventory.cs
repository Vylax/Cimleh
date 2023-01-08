using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Inventory : MonoBehaviour {

    [System.Serializable]
    public struct SerializableVector2
    {
        public float x;
        public float y;

        public SerializableVector2(float rX, float rY)
        {
            x = rX;
            y = rY;
        }
        public override string ToString()
        {
            return string.Format("[{0}, {1}]", x, y);
        }

        public static implicit operator Vector2(SerializableVector2 rValue)
        {
            return new Vector2(rValue.x, rValue.y);
        }

        public static implicit operator SerializableVector2(Vector2 rValue)
        {
            return new SerializableVector2(rValue.x, rValue.y);
        }
    }

    [System.Serializable]
    public class Item//used to define an item
    {
        public int id;
        public int amount;
        public int sizeX;
        public int sizeY;
        public int[] size;
        public string name;
        public SerializableVector2[] corners = new SerializableVector2[2];//top left and bottom right

        public Item(int _id, int _amount)
        {
            id = _id;
            amount = _amount;
            name = ItemsList.itemsNames[_id];
            size = ItemsList.itemsSize[_id];
            sizeX = size[0];
            sizeY = size[1];
        }
    }

    [System.Serializable]
    public class Slot//used to retreive info of any slot of the backpack
    {
        public int itemIndex;//index of the item in the "items" array of bag
        public bool isEmpty;

        public Slot(bool _isEmpty, int index)
        {
            itemIndex = index;
            isEmpty = _isEmpty;
        }
    }

    [System.Serializable]
    public class Bag
    {
        public int sizeX;
        public int sizeY;
        public int size;
        public Item[] items;//Array of all items in the bag
        public List<int> freeItemsIndexes = new List<int>();//list of all available indexes in "items" array
        public Slot[][] slots;//Array of all slots in the bag

        public Bag(int sX, int sY)
        {
            sizeX = sX;
            sizeY = sY;
            size = sizeX * sizeY;
            items = new Item[size];
            slots = new Slot[sizeY][];
            for (int k = 0; k < sizeY; k++)
            {
                slots[k] = new Slot[sizeX];
            }
            for (int i = 0; i < size; i++)
            {
                items[i] = new Item(0, 1);
                freeItemsIndexes.Add(i);
            }
            int j = 0;
            for (int x = 0; x < sX; x++)
            {
                for (int y = 0; y < sY; y++)
                {
                    slots[y][x] = new Slot(true, -1);
                    j++;
                }
            }
        }
    }

    public string AddItem(Item _item, Bag _bag, SerializableVector2[] pos, bool movingItem = false)
    {
        if(_bag.freeItemsIndexes.Count > 0)
        {
            int index = _bag.freeItemsIndexes[0];
            for (int y = (int)pos[0].y; y <= (int)pos[1].y; y++)
            {
                for (int x = (int)pos[0].x; x <= (int)pos[1].x; x++)
                {
                    //Debug.Log("Bug, slot: (" + x + ", " + y + ")");
                    if (!_bag.slots[y][x].isEmpty)
                    {
                        //Debug.Log("Can't place this item here, slot: (" + x + ", " + y + ") is already used by: " + _bag.items[_bag.slots[y][x].itemIndex].name + " || pos[0]: (" + pos[0].x +", " +pos[0].y + "), pos[1]: (" + pos[1].x + ", " + pos[1].y + ")");
                        if(!movingItem)
                        {
                            return "filled slots";
                        }
                        else
                        {
                            if(_bag.slots[y][x].itemIndex != holdItemIndex/*!(x >= holdItem.corners[0].x && x <= holdItem.corners[1].x && y >= holdItem.corners[0].y && y <= holdItem.corners[1].y)*/)
                            {
                                return "filled slots";
                            }
                            else
                            {
                                Debug.Log("moving into the same item spot");
                            }
                        }
                    }
                }
            }
            //if worked : assign the item to every slots
            for (int y = (int)pos[0].y; y <= (int)pos[1].y; y++)
            {
                for (int x = (int)pos[0].x; x <= (int)pos[1].x; x++)
                {
                    _bag.slots[y][x].isEmpty = false;
                    _bag.slots[y][x].itemIndex = index;
                    _bag.freeItemsIndexes.RemoveAt(0);
                }
            }
            _bag.items[index] = _item;
            _bag.items[index].corners = pos;
            
            return "yes";
        }
        else
        {
            Debug.Log("no free indexes");
            return "nope";
        }
    }

    public GameObject itemPrefab;

    public string DropItem(Item _item, int bagIndex, Bag _bag)
    {
        int tempi = 0;
        for (int y = (int)_item.corners[0].y; y <= (int)_item.corners[1].y; y++)
        {
            for (int x = (int)_item.corners[0].x; x <= (int)_item.corners[1].x; x++)
            {
                _bag.slots[y][x].isEmpty = true;
                _bag.slots[y][x].itemIndex = -1;
                _bag.freeItemsIndexes.Add(bagIndex+tempi);
                tempi++;
            }
        }
        _bag.items[bagIndex] = new Item(0, 1);
        GameObject droppedItem = Instantiate(ItemsList.itemsGameObjects[_item.id], new Vector3(transform.position.x, Mathf.FloorToInt(transform.position.y), transform.position.z), Quaternion.identity);
        droppedItem.GetComponent<ItemPickup>().id = _item.id;
        if(droppedItem.GetComponent<CubeMesh>())
        {
            droppedItem.GetComponent<CubeMesh>().id = _item.id-1;
        }
        droppedItem.GetComponent<ItemPickup>().amount = _item.amount;
        GameObject.Find("MapGenV2").GetComponent<ItemsManager>().AddItem(_item.id, _item.amount, droppedItem);
        return "yes";
    }

    public string RemoveItem(Item _item, int bagIndex, Bag _bag)
    {
        int tempi = 0;
        for (int y = (int)_item.corners[0].y; y <= (int)_item.corners[1].y; y++)
        {
            for (int x = (int)_item.corners[0].x; x <= (int)_item.corners[1].x; x++)
            {
                _bag.slots[y][x].isEmpty = true;
                _bag.slots[y][x].itemIndex = -1;
                _bag.freeItemsIndexes.Add(bagIndex+tempi);
                tempi++;
            }
        }
        _bag.items[bagIndex] = new Item(0, 1);
        return "yes";
    }

    public class FreeIndex
    {
        public bool indexAvailable;
        public SerializableVector2[] position;

        public FreeIndex(int[] size, Bag _bag)
        {
            bool[] allFree = new bool[_bag.size];
            SerializableVector2[][] freePos = new SerializableVector2[_bag.size][];
            for (int k = 0; k < _bag.size; k++)
            {
                allFree[k] = true;
                freePos[k] = new SerializableVector2[2];
            }
            //ajouter les si impairs !!!!!
            int i = 0;
            for (int y = 0; y + size[1]-1 < _bag.sizeY; y ++)
            {
                for (int x = 0; x + size[0]-1 < _bag.sizeX; x ++)
                {
                    //Debug.Log("(" + x + ", " + y + "), i:" + i + " | b: (" + _bag.sizeX + ", " + _bag.sizeY + ")");
                    freePos[i][0] = new SerializableVector2(x, y);
                    freePos[i][1] = new SerializableVector2(x + size[0]-1, y + size[1]-1);
                    for (int y2 = 0; y2 < size[1]; y2++)
                    {
                        for (int x2 = 0; x2 < size[0]; x2++)
                        {
                            if(y + y2 < _bag.sizeY && x + x2 < _bag.sizeX)//if isn't out of bag size
                            {
                                if (!_bag.slots[y + y2][x + x2].isEmpty)
                                {
                                    allFree[i] = false;
                                    break;
                                }
                            }
                            else
                            {
                                allFree[i] = false;
                                break;
                            }
                        }
                        if (!allFree[i])
                        {
                            break;
                        }
                    }
                    i++;
                }
            }
            bool temp = false;
            SerializableVector2[] temp2 = new SerializableVector2[] { new SerializableVector2(-1, -1), new SerializableVector2(-1, -1) };
            for (int j = 0; j < allFree.Length; j++)
            {
                if (allFree[j])
                {
                    temp = true;
                    temp2 = freePos[j];
                    break;
                }
            }
            indexAvailable = temp;
            position = temp2;
        }
    }

    public void PickUpItem(int _id, int _amount, GameObject _obj)
    {
        Item pickedItem = new Item(_id, _amount);
        FreeIndex index = new FreeIndex(pickedItem.size, bag);
        if(AddItem(pickedItem, bag, index.position) == "yes")
        {
            GameObject.Find("MapGenV2").GetComponent<ItemsManager>().RemoveItem(_obj);
            Destroy(_obj);
        }
        else
        {
            Debug.Log("Can't pickup: " + pickedItem.name + ", bag doesn't have free slots of this size");
        }
    }

    public bool displayInventory = false;//set to tab press toggle
    public bool bagDefined = false;
    public Bag bag;

    public void InitBag()
    {
        bag = new Bag(6, 6);
        r = new Rect[6][];
        ro = new Rect(195, 95, 320*2-10, 320*2-10);
        for (int i = 0; i < r.Length; i++)
        {
            r[i] = new Rect[r.Length];
        }
        bagDefined = true;
        ml = GetComponent<FirstPersonController>().m_MouseLook;
    }

    public void LoadBag()
    {
        bag = GameObject.Find("MapGenV2").GetComponent<SaveSystem>().loadedGame.bag;
        r = new Rect[6][];
        ro = new Rect(195, 95, 320 * 2 - 10, 320 * 2 - 10);
        for (int i = 0; i < r.Length; i++)
        {
            r[i] = new Rect[r.Length];
        }
        bagDefined = true;
        ml = GetComponent<FirstPersonController>().m_MouseLook;
    }

    public MouseLook ml;

    public int currentlySelectedItem = 0;
    private int maxItemSlot = 4;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            displayInventory = !displayInventory;
            if(displayInventory)
            {
                this.gameObject.GetComponentInChildren<TriangleFinder>().canInteractWithBlocks = false;
                ml.SetCursorLock(false);
            }
            else
            {
                this.gameObject.GetComponentInChildren<TriangleFinder>().canInteractWithBlocks = true;
                ml.SetCursorLock(true);
            }
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentlySelectedItem++;
        }else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentlySelectedItem--;
        }
        if(currentlySelectedItem < 0)
        {
            currentlySelectedItem = maxItemSlot;
        }else if(currentlySelectedItem > maxItemSlot)
        {
            currentlySelectedItem = 0;
        }
    }

    public bool holdingItem = false;
    public int holdItemIndex = -1;
    public int holdItemIndex2 = -2;
    private Item holdItem;

    private Rect[][] r;
    private Rect ro;

    private int[] arrayContainingCursor = new int[] { -11, -11 };
    private int[] originalPos = new int[] { -12, -12 };

    private void OnGUI()
    {
        SerializableVector2 mousePosFromEvent = Event.current.mousePosition;
        if (displayInventory && bagDefined)
        {
            if(!GetComponent<FirstPersonController>().camLock)
            {
                GetComponent<FirstPersonController>().camLock = true;
            }
            GUI.Box(ro, "");
            for (int i = 0; i < bag.sizeX; i++)
            {
                for (int j = 0; j < bag.sizeY; j++)
                {
                    r[j][i] = new Rect(200 + 52 * i*2, 100 + 52 * j*2, 50*2, 50*2);
                    GUI.Box(r[j][i], /*bag.slots[j][i].itemIndex + */"");
                    /*if(r.Contains(mousePosFromEvent))
                    {
                        Debug.Log("(" + i + ", " + j + ")");
                    }*/
                    /*if(r[j][i].Contains(mousePosFromEvent))
                    {
                        arrayContainingCursor = new int[] { j, i };
                    }*/
                    if(holdingItem && Input.GetButtonUp("Fire1") && r[j][i].Contains(mousePosFromEvent))
                    {
                        arrayContainingCursor = new int[] { j, i };
                        Debug.Log("AA" + bag.slots[j][i].itemIndex + " | " + i + ", " + j);
                        if (arrayContainingCursor != new int[] { -11, -11 })
                        {
                            int j2 = arrayContainingCursor[0];
                            int i2 = arrayContainingCursor[1];

                            Debug.Log("(" + i + ", " + j + "), "+ "(" + i2 + ", " + j2 + ")");
                            //Vector2 aaaaaaa = bag.items[bag.slots[j2][i2].itemIndex].corners[0];
                            SerializableVector2[] tempVec = new SerializableVector2[] { new SerializableVector2(i2, j2), new SerializableVector2(i2 + holdItem.sizeX - 1, j2+ holdItem.sizeY - 1) };
                            if(originalPos[1] == i2 && originalPos[0] == j2)//si l'on click juste sur l'item ==> drop
                            {
                                DropItem(bag.items[holdItemIndex], holdItemIndex, bag);
                            }
                            else
                            {
                                Debug.Log("(" + originalPos[1] + ", " + originalPos[0] + "), " + "(" + i2 + ", " + j2 + ")");
                                //Debug.Log("(" + tempVec[0].x + ", " + tempVec[0].y + ") | (" + tempVec[1].x + ", " + tempVec[1].y + ")    |||    (" + holdItem.corners[0].x + ", " + holdItem.corners[0].y + ") | (" + holdItem.corners[1].x + ", " + holdItem.corners[1].y + ")");
                                Item tempItem = new Item(holdItem.id, holdItem.amount);
                                if (AddItem(tempItem, bag, tempVec, true) == "yes")
                                {
                                    if (RemoveItem(holdItem, holdItemIndex, bag) == "yes")
                                    {
                                        Debug.Log("Item moved succesfully");
                                    }
                                    else
                                    {
                                        Debug.Log("error happened while moving Item");
                                    }
                                }
                                else
                                {
                                    Debug.Log("error happened while adding Item: (" + tempVec[0].x + ", " + tempVec[0].y + ") | (" + tempVec[1].x + ", " + tempVec[1].y + ")");
                                }
                            }
                        }
                        holdingItem = false;
                        arrayContainingCursor = new int[] { -11, -11 };
                    }else if(holdingItem && Input.GetButtonUp("Fire1") && !r[j][i].Contains(mousePosFromEvent) && j == bag.sizeY-1 && i == bag.sizeX-1 && !ro.Contains(mousePosFromEvent))//if release item over nothing drop item
                    {
                        DropItem(bag.items[holdItemIndex], holdItemIndex, bag);
                        holdingItem = false;
                        arrayContainingCursor = new int[] { -11, -11 };
                    }
                    else if (holdingItem && Input.GetButtonUp("Fire1") && !r[j][i].Contains(mousePosFromEvent) && j == bag.sizeY - 1 && i == bag.sizeX - 1 && ro.Contains(mousePosFromEvent))//if release item over inventory gui
                    {
                        originalPos = new int[] { -12, -12 };
                        holdItemIndex = -1;
                        holdItemIndex2 = -2;
                        holdingItem = false;
                        holdItem = null;
                    }
                    if (Input.GetButton("Fire1"))
                    {
                        if (!holdingItem)
                        {
                            if(bag.slots[j][i].itemIndex >= 0 && holdItemIndex2 == bag.slots[j][i].itemIndex && r[j][i].Contains(mousePosFromEvent))
                            {
                                holdItem = bag.items[bag.slots[j][i].itemIndex];
                                originalPos = new int[] { j, i };
                                holdItemIndex = bag.slots[j][i].itemIndex;
                                holdingItem = true;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < bag.items.Length; i++)
            {
                if(bag.items[i].id > 0 && bag.items[i].amount > 0)
                {
                    if(GUI.RepeatButton(new Rect(200 + 52 * bag.items[i].corners[0].x*2, 100 + 52 * bag.items[i].corners[0].y*2, 2*(bag.items[i].sizeX * (50 + 2) - 2), 2*(bag.items[i].sizeY * (50 + 2) - 2)), ItemsList.itemsTextures[bag.items[i].id]))
                    {
                        if(!holdingItem)
                        {
                            holdItemIndex2 = i;
                        }
                        //DropItem(bag.items[i], i, bag);
                    }
                }
            }
            if(holdingItem)
            {
                GUI.Box(new Rect(mousePosFromEvent.x - 50, mousePosFromEvent.y - 50, 2 * (bag.items[holdItemIndex].sizeX * (50 + 2) - 2), 2 * (bag.items[holdItemIndex].sizeY * (50 + 2) - 2)), ItemsList.itemsTextures[bag.items[holdItemIndex].id]);
            }
        }
        else if(!displayInventory)
        {
            if (GetComponent<FirstPersonController>().camLock)
            {
                GetComponent<FirstPersonController>().camLock = false;
            }
        }
        int bs = 75;
        int ss = 10;
        if(!displayInventory)
        {
            for (int i = 0; i <= maxItemSlot; i++)
            {
                if (i == currentlySelectedItem)
                {
                    GUI.Box(new Rect(Screen.width / 2f + (bs * (i - 2.5f) + ss * (i - 1.5f)) - 4, Screen.height - 90 - 4, bs + 8, bs + 8), "");
                }
                GUI.Box(new Rect(Screen.width / 2f + (bs * (i - 2.5f) + ss * (i - 1.5f)), Screen.height - 90, bs, bs), ItemsList.itemsTextures[i + 1]);
            }
        }
        //display list for tests
        /*for (int i = 0; i < bag.freeItemsIndexes.Count; i++)
        {
            GUI.Label(new Rect(600, 10+20*i, 50, 20), i + ": " + bag.freeItemsIndexes[i]);
        }*/
        /*if(GUI.Button(new Rect(200, 10, 50, 50), "Add Item"))
        {
            Item randomItem = new Item(Random.Range(1, ItemsList.itemsCount), 1);
            FreeIndex freeIndex = new FreeIndex(randomItem.size, bag);
            if(freeIndex.indexAvailable)
            {
                AddItem(randomItem, bag, freeIndex.position);
            }
            else
            {
                Debug.Log("No available position in bag for this item\n item:" + randomItem.name);
            }
        }*/
    }

}
