using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using Bag = Inventory.Bag;
using Item = Inventory.Item;
using Slot = Inventory.Slot;
using ItemInfos = ItemsManager.ItemInfos;
using SerializableQuaternion = ItemsManager.SerializableQuaternion;

public class SaveSystem : MonoBehaviour {

    [System.Serializable]
    public class Game
    {
        public string saveName;
        public string seed;//seedStone(4: 0-3),seedHills(4: 4-7),seedGrass(4: 8-11),seedCaves(4: 12-15)
        public int[,,] blocks;
        public List<int[]> trees;
        public float[] playerPos;
        public SerializableQuaternion playerRot;
        public int[] sizes;
        public Bag bag;
        public List<ItemInfos> itemsInfos;
        public float health;
        public float hunger;
        public float thirst;
        public float stamina;
        public float playTime;
        public float version;

        public Game(string _saveName, string _seed, int[,,] _blocks, List<int[]> _trees, float[] _playerPos, int[] _sizes)
        {
            saveName = _saveName;
            seed = _seed;
            blocks = _blocks;
            trees = _trees;
            playerPos = _playerPos;
            sizes = _sizes;
            playTime = 0f;
            version = GameInfos.version;
        }
    }

    public Game loadedGame;

    public void Save(Game saveGame)
    {
        if (!Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) +"/My Games/Cimleh/Saves/"))
        {
            Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) +"/My Games/Cimleh/Saves/");
        }
        TakeScreenShot(saveGame.saveName);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) +"/My Games/Cimleh/Saves/" + saveGame.saveName + ".sav");
        bf.Serialize(file, saveGame);
        file.Close();
        PlayerPrefs.SetString("lastLoadedGame", loadedGame.saveName);
        Debug.Log("Saved Game: " + saveGame.saveName);
    }

    public void Load(string gameToLoad)
    {
        if (File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) +"/My Games/Cimleh/Saves/" + gameToLoad + ".sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) +"/My Games/Cimleh/Saves/" + gameToLoad + ".sav", FileMode.Open);
            loadedGame = (Game)bf.Deserialize(file);
            PlayerPrefs.SetString("lastLoadedGame", loadedGame.saveName);
            file.Close();
            Debug.Log("Loaded Game: " + loadedGame.saveName);
        }
    }

    public void LoadMenu(string gameToLoad)
    {
        TextAsset save = Resources.Load("Saves/" + gameToLoad) as TextAsset;
        Stream s = new MemoryStream(save.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        loadedGame = (Game)bf.Deserialize(s);
        Debug.Log("Loaded Game: " + loadedGame.saveName);
    }

    public void CreateGame (MapGen2 mg2, string gameName, string seed, int[] sizes)
    {
        if(mg2.GenMap(sizes, seed))
        {
            Game currentGame = new Game(gameName, seed, mg2.blocks, mg2.trees, new float[3] { 0, 0, 0 }, sizes);
            Save(currentGame);
        }
    }

    //Only for debugging
    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.M))
        {
            int[,,] temp = new int[16, 32, 48];
            temp[1, 2, 3] = 2;
            temp[4, 5, 6] = 9;
            Game newGame = new Game("aaa", "0016003200480001000100010001", temp, new List<int>(), new int[3] { -1, 2, -3 });
            Save(newGame);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load("aaa");
            Debug.Log(loadedGame.saveName + " " + loadedGame.seed + " " + loadedGame.trees.Count + " " + loadedGame.sizes[0] + " " + loadedGame.sizes[1] + " " + loadedGame.sizes[2]);
            Debug.Log(loadedGame.blocks[0, 0, 0]);
            Debug.Log(loadedGame.blocks[1, 2, 3]);
            Debug.Log(loadedGame.blocks[4, 5, 6]);
        }*/
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveGame();
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveGame()
    {
        MapGen2 mg2 = gameObject.GetComponent<MapGen2>();
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Game currentGame = new Game(loadedGame.saveName, loadedGame.seed, mg2.blocks, mg2.trees, new float[3] { playerPos.x, playerPos.y, playerPos.z }, mg2.sizes);
        currentGame.bag = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().bag;
        currentGame.playerRot = GameObject.FindGameObjectWithTag("Player").transform.rotation;
        currentGame.health = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().health;
        currentGame.hunger = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().hunger;
        currentGame.thirst = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().thirst;
        currentGame.stamina = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().stamina;
        currentGame.playTime = loadedGame.playTime + Time.time - startPlayTime;
        if (GetComponent<ItemsManager>().GetItemsTransform())
        {
            currentGame.itemsInfos = GetComponent<ItemsManager>().itemsInfos;
            Save(currentGame);
        }
    }

    float startPlayTime = 0f;

    public void LoadGame(string saveName, bool observationMode = false, bool firstLoad = false)
    {
        //   Load Save
        if (!observationMode)
        {
            GameObject.Find("Sun").GetComponent<daylight>().enabled = true;
            Load(saveName);
            gameObject.GetComponent<MapGen2>().ResetMap();
            startPlayTime = Time.time;
        }
        else
        {
            LoadMenu(saveName);
        }
        
        //   Apply loaded save data to scene
        gameObject.GetComponent<MapGen2>().blocks = loadedGame.blocks;
        gameObject.GetComponent<MapGen2>().trees = loadedGame.trees;
        gameObject.GetComponent<MapGen2>().playerPos = new Vector3(loadedGame.playerPos[0], loadedGame.playerPos[1], loadedGame.playerPos[2]);
        gameObject.GetComponent<MapGen2>().playerRot = loadedGame.playerRot;
        gameObject.GetComponent<MapGen2>().sizes = loadedGame.sizes;
        gameObject.GetComponent<MapGen2>().observationMode = observationMode;
        //   load map
        gameObject.GetComponent<MapGen2>().LoadMap();
        if(gameObject.GetComponent<MapGen2>().BuildMap2() && !observationMode)
        {
            if (firstLoad)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().InitBag();
            }
            else
            {
                gameObject.GetComponent<ItemsManager>().itemsInfos = loadedGame.itemsInfos;
                GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().LoadBag();
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().LoadStats(loadedGame.health, loadedGame.hunger, loadedGame.thirst, loadedGame.stamina);
            }
        }
        gameObject.GetComponent<ItemsManager>().LoadItems();
        StartCoroutine(GetComponent<GetData>().SendData(saveName));
    }

    private void OnApplicationQuit()
    {
        if(GetComponent<MainMenu>().GetCurrentMenuIndex() == -1)
        {
            SaveGame();
        }
    }

    public void TakeScreenShot(string _saveName, int resWidth = 1280, int resHeight = 720)
    {
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        Camera.main.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(resWidth, resHeight, _saveName);
        File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

    public static string ScreenShotName(int width, int height, string saveName)
    {
        return string.Format("{0}/My Games/Cimleh/Saves/{1}_savePicture.png",
                             System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                             saveName);
    }
}
