using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using Game = SaveSystem.Game;

public class MainMenu : MonoBehaviour {

    int localisation = 0;//0 = fr, 1 = en
    static int menu = 0;//0 = main, 1 = play, 2 = settings, 3 = new game, 4 = load game
    static string gameName = "";
    static string seed = "";
    static int[] mapSize = new int[3] { 128, 64, 128 };

    public GameObject GameManagerObj2;
    static GameObject GameManagerObj;

    private Vector2 scrollPosition = Vector2.zero;

    public class SaveInfos
    {
        public string saveName;
        public Texture2D savePicture;
        public System.DateTime lastPlayTime;
        public float playTime;
        public float version;
        public Game game;

        public SaveInfos(string _saveName, Texture2D _savePic, System.DateTime _lastPlayTime, Game _game)
        {
            saveName = _saveName;
            savePicture = _savePic;
            lastPlayTime = _lastPlayTime;
            game = _game;
            playTime = game.playTime;
            version = game.version;
        }
    }

    public List<SaveInfos> savesInfos = new List<SaveInfos>();

    private Color rainbowColor;

    [SerializeField]
    private Texture2D FrenchFlagTile;
    [SerializeField]
    private Texture2D UKFlagTile;

    private void Start()
    {
        StartCoroutine(CheckIfUpdateIsAvailable());
        GameManagerObj = GameManagerObj2;
        PlayerPrefs.SetInt("localisation", localisation);
        GameManagerObj.GetComponent<SaveSystem>().LoadGame("mainmenu2", true);
        Camera.main.transform.position = new Vector3(61.3f, 25.7f, 45.4f);
        Camera.main.transform.eulerAngles = new Vector3(-4.911f, -130, 0);
        GetSavesFiles();
    }

    private void Update()
    {
        rainbowColor = Color.HSVToRGB((Time.time/2f)%1f, 1, 1);
    }

    private Game GetGame (string gameName)
    {
        if (File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/My Games/Cimleh/Saves/" + gameName + ".sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/My Games/Cimleh/Saves/" + gameName + ".sav", FileMode.Open);
            Game loadedGame = (Game)bf.Deserialize(file);
            PlayerPrefs.SetString("lastLoadedGame", loadedGame.saveName);
            file.Close();
            return loadedGame;
        }
        return null;
    }

    private void GetSavesFiles()
    {
        string dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/My Games/Cimleh/Saves/";
        if (!Directory.Exists(dir))
        {
            return;
        }

        List<string> FilesPaths = Directory.GetFiles(dir, "*.sav").OrderByDescending(file => File.GetLastWriteTime(file)).ToList();

        foreach (string FilePath in FilesPaths)
        {
            string saveScreenShotFile = FilePath.Replace(".sav", "_savePicture.png");
            if (File.Exists(saveScreenShotFile))
            {
                string saveName = FilePath.Replace(".sav", "");
                saveName = saveName.Replace(dir, "");
                Game currGame = GetGame(saveName);
                if(currGame.version == GameInfos.version)//if save version is corresponding to game client's one
                {
                    byte[] fileData = File.ReadAllBytes(saveScreenShotFile);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData);
                    savesInfos.Add(new SaveInfos(saveName, tex, File.GetLastWriteTime(FilePath), currGame));
                }
            }
        }
    }

    Rect[][] Rects = new Rect[][]
    {
        new Rect[]//Main Menu
        {
            new Rect(Screen.width / 3f, Screen.height * 1f / 13f * 1 + Screen.height * 3f / 13f * 0, Screen.width / 3f, Screen.height * 3f / 13f),
            new Rect(Screen.width / 3f, Screen.height * 1f / 13f * 2 + Screen.height * 3f / 13f * 1, Screen.width / 3f, Screen.height * 3f / 13f),
            new Rect(Screen.width / 3f, Screen.height * 1f / 13f * 3 + Screen.height * 3f / 13f * 2, Screen.width / 3f, Screen.height * 3f / 13f)
        },
        new Rect[]//Play Menu
        {
            new Rect(Screen.width / 3f, Screen.height * 1f / 13f * 1 + Screen.height * 3f / 13f * 0, Screen.width / 3f, Screen.height * 3f / 13f),
            new Rect(Screen.width / 3f, Screen.height * 1f / 13f * 2 + Screen.height * 3f / 13f * 1, Screen.width / 3f, Screen.height * 3f / 13f),
            new Rect(Screen.width / 3f, Screen.height * 1f / 13f * 3 + Screen.height * 3f / 13f * 2, Screen.width / 3f, Screen.height * 3f / 13f),
            new Rect(10, 10, Screen.width / 6f, 30)
        },
        new Rect[]//Settings Menu
        {
            new Rect(300, 10 + 60 * 1, 150, 50),
            new Rect(300, 10 + 60 * 2, 150, 50),
            new Rect(300, 10 + 60 * 3, 150, 50),
            new Rect(300, 10 + 60 * 4, 150, 50),
            new Rect(300, 10 + 60 * 5, 150, 50),
            new Rect(300, 10 + 60 * 6, 150, 50),
            new Rect(10, 10, Screen.width / 6f, 30),
            new Rect(300, 10, 150, 50)
        },
        new Rect[]//New Game Menu
        {
            new Rect(Screen.width / 3f, Screen.height * 1f / 13f * 3 + Screen.height * 3f / 13f * 2, Screen.width / 3f, Screen.height * 3f / 13f),
            new Rect(10, 10, Screen.width / 6f, 30),
            new Rect(Screen.width / 3f + 160 * 0, Screen.height * 1f / 13f * 1 + 55 * 2+20, 150, 50),
            new Rect(Screen.width / 3f + 160 * 1, Screen.height * 1f / 13f * 1 + 55 * 2+20, 150, 50),
            new Rect(Screen.width / 3f + 160 * 2, Screen.height * 1f / 13f * 1 + 55 * 2+20, 150, 50),
            new Rect(Screen.width / 3f + 160 * 3, Screen.height * 1f / 13f * 1 + 55 * 2+20, 150, 50),
            new Rect(Screen.width / 3f, Screen.height * 1f / 13f * 1 + 20, Screen.width / 3f, 30),
            new Rect(Screen.width / 3f, Screen.height * 1f / 13f * 1 + 55+20, Screen.width / 3f, 30),
            new Rect(10, Screen.height * 1f / 13f * 1 + 20, Screen.width / 3f-20, 30),
            new Rect(10, Screen.height * 1f / 13f * 1 + 55+20, Screen.width / 3f-20, 30),
            new Rect(10, Screen.height * 1f / 13f * 1 + 55 * 2+20, Screen.width / 3f-20, 50)
        },
    };

    string[][][] texts = new string[][][]
    {
        new string[][]//Main Menu
        {
            new string[]//fr
            {
                "Jouer",
                "Paramètres",
                "Quitter"
            },
            new string[]//en
            {
                "Play",
                "Settings",
                "Quit"
            },
        },
        new string[][]//Play Menu
        {
            new string[]//fr
            {
                "Continuer",
                "Nouvelle partie",
                "Charger une partie",
                "Retour"
            },
            new string[]//en
            {
                "Continue",
                "New Game",
                "Load Game",
                "Back"
            },
        },
        new string[][]//Settings Menu
        {
            new string[]//fr
            {
                "Très faibles",
                "Faibles",
                "Moyens",
                "Bon",
                "Très bon",
                "Ultra",
                "Retour",
                "Graphismes:"
            },
            new string[]//en
            {
                "Very low",
                "Low",
                "Medium",
                "High",
                "Very high",
                "Ultra",
                "Back",
                "Graphics:"
            },
        },
        new string[][]//New Game Menu
        {
            new string[]//fr
            {
                "Créer",
                "Retour",
                "Petite (128x128)",
                "Moyenne (256x256)",
                "Grande (512x512)",
                "Très Grande (1024x1024)",
                "Champ Nom",
                "Champ Graine",
                "Nom:",
                "Graine de génération (facultatif):",
                "Taille map:",
            },
            new string[]//en
            {
                "Create",
                "Back",
                "Small (128x128)",
                "Medium (256x256)",
                "Big (512x512)",
                "Huge (1024x1024)",
                "Name field",
                "Seed field",
                "Name:",
                "Seed (optional):",
                "Map Size:",
            },
        },
        new string[][]//Load Game Menu
        {
            new string[]//fr
            {
                "Nom: ",
                "Temps de jeu: ",
                "Dernière sauvegarde le: ",
                "Graine de génération: ",
                "Charger la partie",
                "Aucun fichier de sauvegarde à jour trouvé!",
            },
            new string[]//en
            {
                "Name: ",
                "Time spent: ",
                "Last saved on: ",
                "Seed: ",
                "Load Game",
                "No up to date save file found!",
            },
        },
    };

    //delegate void actionsVoids(int num);
    System.Action[][] actions = new System.Action[][]
    {
        new System.Action[]//Main Menu
        {
            () => ChangeMenu(1),
            () => ChangeMenu(2),
            () => Application.Quit()
        },
        new System.Action[]//Play Menu
        {
            () => LoadLastGame(),//load last game
            () => ChangeMenu(3),
            () => ChangeMenu(4),
            () => ChangeMenu(0)
        },
        new System.Action[]//Settings Menu
        {
            () => QualitySettings.SetQualityLevel(0, true),
            () => QualitySettings.SetQualityLevel(1, true),
            () => QualitySettings.SetQualityLevel(2, true),
            () => QualitySettings.SetQualityLevel(3, true),
            () => QualitySettings.SetQualityLevel(4, true),
            () => QualitySettings.SetQualityLevel(5, true),
            () => ChangeMenu(0)
        },
        new System.Action[]//New Game Menu
        {
            () => CreateAndLoad(),//Create game with seed
            () => ChangeMenu(1),
            () => SetMapSize(128),
            () => SetMapSize(256),
            () => SetMapSize(512),
            () => SetMapSize(1024),
        },
    };

    private void OnGUI()
    {
        if(menu == -1)
        {
            return;
        }
        if(menu == 0)
        {
            if (localisation == 1)
            {
                GUI.Box(new Rect(Screen.width - 20 - 205, 5, 110, 110), "");
            }
            else if (localisation == 0)
            {
                GUI.Box(new Rect(Screen.width - 10 - 105, 5, 110, 110), "");
            }
            if (GUI.Button(new Rect(Screen.width - 10 - 100, 10, 100, 100), FrenchFlagTile))
            {
                localisation = 0;
                PlayerPrefs.SetInt("localisation", localisation);
                PlayerPrefs.Save();
            }
            if (GUI.Button(new Rect(Screen.width - 20 - 200, 10, 100, 100), UKFlagTile))
            {
                localisation = 1;
                PlayerPrefs.SetInt("localisation", localisation);
                PlayerPrefs.Save();
            }
            GUIStyle style1 = new GUIStyle();
            style1.alignment = TextAnchor.MiddleCenter;
            style1.fontSize = 20;
            GUI.Box(new Rect(10, 10, 225, 100), "Cimleh V" + GameInfos.version, style1);
            if (isUpdateAvailable)
            {
                GUIStyle temp = new GUIStyle();
                temp.alignment = TextAnchor.MiddleCenter;
                //temp.font.material.color = new Color(0, 0, 0);
                temp.normal.textColor = rainbowColor;
                //temp.normal.background = GUI.skin.box.normal.background;
                if(GUI.Button(new Rect(Screen.width - 10 - 225, Screen.height - 10 - 100, 225, 100), "A new update is available!\n Click here to download Cimleh V" + availableUpdateVersion, temp))
                {
                    Application.OpenURL("http://vylax.free.fr/cimleh/index.html");
                }
            }
        }
        if(menu != 4)
        {
            try
            {
                for (int i = 0; i < Rects[menu].Length; i++)
                {
                    if (menu == 2 && i == 7 || menu == 3 && i > 7)//labels
                    {
                        GUI.Box(Rects[menu][i], texts[menu][localisation][i]);
                    }
                    else if (menu == 3 && i > 5)//textfields
                    {
                        if (i == 6)//name txt box
                        {
                            gameName = GUI.TextField(Rects[menu][i], gameName, 16);
                            gameName.Replace(" ", "_");
                            gameName = Regex.Replace(gameName, "[^a-zA-Z0-9_-]", "");
                        }
                        else if (i == 7)//seed txt box
                        {
                            seed = GUI.TextField(Rects[menu][i], seed, 16);
                            seed = Regex.Replace(seed, "[^0-9]", "");
                        }
                    }
                    else //buttons
                    {
                        if (GUI.Button(Rects[menu][i], texts[menu][localisation][i]))
                        {
                            actions[menu][i]();
                        }
                    }
                }
            }
            catch
            {
                Debug.Log("User left main menu");
            }
        }
        else if (menu == 4)
        {
            if (GUI.Button(Rects[1][3], texts[1][localisation][3]))
            {
                ChangeMenu(1);
            }
            scrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 6f, Screen.height / 6f, Screen.width * 2 / 3f+20, Screen.height * 2 / 3f), scrollPosition, new Rect(0, 0, Screen.width * 2 / 3f, 410 * (savesInfos.Count)));
            for (int j = 0; j < savesInfos.Count; j++)
            {
                GUI.Box(new Rect(0, 10 + j * 410, Screen.width * 2 / 3f, 400), "");
                GUI.DrawTexture(new Rect(10, 10 + 10 + j * 410, 337, 190), savesInfos[j].savePicture);
                for (int k = 0; k < texts[menu][localisation].Length-1; k++)
                {
                    string txt = texts[menu][localisation][k];
                    if(k == 0)
                    {
                        txt += savesInfos[j].saveName;
                    }
                    else if (k == 1)
                    {
                        string playTimeString = Mathf.FloorToInt(savesInfos[j].playTime / 3600).ToString() + "h " + Mathf.FloorToInt(savesInfos[j].playTime / 60).ToString() + "min " + Mathf.FloorToInt(savesInfos[j].playTime % 60).ToString() + "sec";
                        txt += playTimeString;
                    }
                    else if (k == 2)
                    {
                        txt += savesInfos[j].lastPlayTime;
                    }
                    else if (k == 3)
                    {
                        txt += savesInfos[j].game.seed;
                    }
                    if (k != 4)
                    {
                        GUI.Label(new Rect(10, 10 + 200 + j * 410 + 35 * (k+1), 350, 25), txt);
                    }
                    else
                    {
                        if(GUI.Button(new Rect(10 + 50 + 337, 350 + j * 410, 200, 50), txt))
                        {
                            LoadChoosenGame(savesInfos[j].saveName);
                        }
                    }
                }
            }
            if(savesInfos.Count == 0)
            {
                GUI.Box(new Rect(0, 10, Screen.width * 2 / 3f, 400), texts[menu][localisation][texts[menu][localisation].Length-1]);
            }
            GUI.EndScrollView();
        }
    }

    public static void ChangeMenu(int menuIndex)
    {
        menu = menuIndex;
    }

    public void ChangeMenu2(int menuIndex)
    {
        ChangeMenu(menuIndex);
    }

    public int GetCurrentMenuIndex()
    {
        return menu;
    }

    static void SetMapSize(int size)
    {
        mapSize = new int[3] { size, 64, size };
    }

    private static void CreateAndLoad()
    {
        if(Create())
        {
            GameManagerObj.GetComponent<SaveSystem>().LoadGame(gameName, false, true);
            ChangeMenu(-1);
        }
    }

    private static void LoadLastGame()
    {
        if(PlayerPrefs.HasKey("lastLoadedGame"))
        {
            GameManagerObj.GetComponent<SaveSystem>().LoadGame(PlayerPrefs.GetString("lastLoadedGame"));
            ChangeMenu(-1);
        }
    }

    private static void LoadChoosenGame(string gameName)
    {
        GameManagerObj.GetComponent<SaveSystem>().LoadGame(gameName);
        ChangeMenu(-1);
    }

    private static bool Create()
    {
        if(gameName.Length == 0)
        {
            return false;
        }
        if(seed.Length != 16)//no seed/invalid seed ==> random gen
        {
            Debug.Log("Random seed");
            seed = "";
            for (int i = 0; i < 4; i++)
            {
                string temp = Random.Range(0, 10000).ToString();
                while(temp.Length < 4)
                {
                    temp = "0" + temp;
                }
                seed += temp;
            }
        }else if(seed.Length == 16)//seed given ==> check if it's valid
        {
            int temp;
            int[] vals = new int[7];
            for (int i = 0; i < 4; i++)
            {
                if (int.TryParse(seed.Substring(i * 4, 4), out temp))
                {
                    vals[i] = temp;
                }
                else
                {
                    return false;
                }
            }
        }
        //Create map and save
        GameManagerObj.GetComponent<SaveSystem>().CreateGame(GameManagerObj.GetComponent<MapGen2>(), gameName, seed, mapSize);
        return true;
    }

    bool isUpdateAvailable = false;
    float availableUpdateVersion;

    private IEnumerator CheckIfUpdateIsAvailable()
    {
        WWW lastestVersion = new WWW("http://vylax.free.fr/cimleh/getVersion");
        yield return lastestVersion;
        availableUpdateVersion = float.Parse(lastestVersion.text);
        if (GameInfos.version != availableUpdateVersion)
        {
            isUpdateAvailable = true;
        }
    }
}
