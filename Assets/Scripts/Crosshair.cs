using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {
    
    [SerializeField]
    private int ch_size = 5; //min 0, max 32
    [SerializeField]
    private int ch_spacing = 3; // min 0, max 32
    [SerializeField]
    private int ch_dot = 0; // min 0, max 16 radius
    [SerializeField]
    private Color ch_color = Color.cyan;
    [SerializeField]
    private int ch_color_r = 0;
    [SerializeField]
    private int ch_color_g = 0;
    [SerializeField]
    private int ch_color_b = 0;
    [SerializeField]
    private int ch_thickness = 2; // min 0, max 32
    [SerializeField]
    private int ch_outline = 0; // min 0, max 5
    [SerializeField]
    private Shader unlit;
    [SerializeField]
    private int ecart = 5;

    Texture2D texture;

    void Start()
    {
        unlit = Shader.Find("Unlit/Transparent Cutout");
        DrawCrosshair();
    }

    void DrawCrosshair()
    {
        texture = new Texture2D(128, 128);
        /*GetComponent<Renderer>().material.mainTexture = texture;
        GetComponent<Renderer>().material.shader = unlit;*/
        //clear texture
        for (int y = 0; y < texture.height; y++) {
            for (int x = 0; x < texture.width; x++) {
                texture.SetPixel(x, y, Color.clear);
            }
        }
        //prepare spacing
        /*for (var xxx: int = texture.width/2 - (ch_spacing-1)/2; xxx < texture.width/2 + (ch_spacing-1)/2; xxx++) {
            for (var yyy: int = texture.height/2 - (ch_spacing-1)/2; yyy < texture.height/2 + (ch_spacing-1)/2; yyy++) {
                texture.SetPixel(xxx, yyy, Color.red);// test
            }
        }*/
        // left ch bar
        for (int x = texture.width / 2 - ch_spacing / 2 - ch_size; x < texture.width / 2 - ch_spacing / 2; x++) {
            for (int y = texture.height / 2 - ch_thickness / 2 + ecart; y < texture.height / 2 + ch_thickness / 2 + ecart; y++) {
                texture.SetPixel(x, y, ch_color);
            }
        }
        // right ch bar
        for (int x = texture.width / 2 + ch_spacing / 2; x < texture.width / 2 + ch_spacing / 2 + ch_size; x++) {
            for (int y = texture.height / 2 - ch_thickness / 2 + ecart; y < texture.height / 2 + ch_thickness / 2 + ecart; y++) {
                texture.SetPixel(x, y, ch_color);
            }
        }
        // up ch bar
        for (int x = texture.width / 2 - ch_thickness / 2; x < texture.width / 2 + ch_thickness / 2; x++) {
            for (int y = texture.height / 2 + ch_spacing / 2 + ecart; y < texture.height / 2 + ch_spacing / 2 + ch_size + ecart; y++) {
                texture.SetPixel(x, y, ch_color);
            }
        }
        // down ch bar
        for (int x = texture.width / 2 - ch_thickness / 2; x < texture.width / 2 + ch_thickness / 2; x++) {
            for (int y = texture.height / 2 - ch_spacing / 2 - ch_size + ecart; y < texture.height / 2 - ch_spacing / 2 + ecart; y++) {
                texture.SetPixel(x, y, ch_color);
            }
        }
        texture.Apply();
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width/2f-64, Screen.height / 2f-64, 128, 128), texture, ScaleMode.ScaleToFit, true);
    }
}
