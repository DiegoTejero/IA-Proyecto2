using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gif_Code : MonoBehaviour {

    public RawImage image;
    public Texture[] gifFrames;

    public float fps = 0.05f;

    private int index = 0;
    private float time;

    private Shader shaderSpritesDefault;
    private Shader shaderGUItext;

    // Use this for initialization
    void Start () {
        index = 0;
        time = Time.time;

        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");

        image.material.shader = shaderSpritesDefault;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (time + fps <= Time.time)
        {
            if (index == gifFrames.Length)
            {
                index = 0;
            }
            image.texture = gifFrames[index];
            index++;
            time = Time.time;
        }
    }

    public IEnumerator BlinkColor(Color32 color , bool solidColor)
    {
        if (solidColor)
            solidColorSprite(color);
        else
            NormalSprite(color);

        yield return new WaitForSeconds(0.1f);

        NormalSprite(new Color32(255, 255, 255, 255));

        yield return new WaitForSeconds(0.1f);

        if (solidColor)
            solidColorSprite(color);
        else
            NormalSprite(color);

        yield return new WaitForSeconds(0.1f);

        NormalSprite(new Color32(255, 255, 255, 255));
    }

    private void solidColorSprite(Color32 color)
    {
        image.material.shader = shaderGUItext;
        image.color = color;
        
    }

    private void NormalSprite(Color32 color)
    {
        image.material.shader = shaderSpritesDefault;
        image.color = color;
    }
}
