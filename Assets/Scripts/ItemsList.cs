using UnityEngine;

public static class ItemsList
{
    public static int itemsCount = 6;
    public static string[] itemsNames = new string[]
    {
        "",
        "grass",
        "stone",
        "water",
        "dirt",
        "machine",
        "shotgun"
    };
    public static Texture2D[] itemsTextures = new Texture2D[]
    {
        GetTexture("empty"),
        GetTexture("grass"),
        GetTexture("stone"),
        GetTexture("water"),
        GetTexture("dirt"),
        GetTexture("machine"),
        GetTexture("shotgun")
    };
    public static int[][] itemsSize = new int[][]
    {
        new int[] { 1, 1 },
        new int[] { 1, 1 },
        new int[] { 1, 1 },
        new int[] { 1, 1 },
        new int[] { 1, 1 },
        new int[] { 2, 2 },
        new int[] { 2, 1 },
    };
    public static GameObject[] itemsGameObjects = new GameObject[]
    {
        GetPrefab("Item"),
        GetPrefab("CubeItem"),
        GetPrefab("CubeItem"),
        GetPrefab("CubeItem"),
        GetPrefab("CubeItem"),
        GetPrefab("CubeItem"),
        GetPrefab("Item"),
    };

    private static Texture2D GetTexture(string fileName)
    {
        return (Texture2D)Resources.Load("Tiles/" + fileName);
    }

    private static GameObject GetPrefab(string fileName)
    {
        return (GameObject)Resources.Load("Prefabs/" + fileName);
    }
}