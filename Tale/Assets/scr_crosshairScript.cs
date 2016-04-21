using UnityEngine;
using System.Collections;

public class scr_crosshairScript : MonoBehaviour {

	// Use this for initialization
    Rect crosshairRect;
    Texture crosshairTexture;
    
	void Start () 
    {


        float crosshairSize = Screen.width * 0.01f;
        crosshairTexture = Resources.Load("Textures/crosshair") as Texture;
        crosshairRect = new Rect(Screen.width / 2 - crosshairSize / 2, Screen.height / 2 - crosshairSize / 2, crosshairSize, crosshairSize);

	}
    void OnGUI()
    {
        GUI.DrawTexture(crosshairRect, crosshairTexture);
    }
}
