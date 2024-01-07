using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteMerger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spritesToMerge = null; 
    [SerializeField] private SpriteRenderer finalSpriteRenderer = null; 
    [SerializeField] private Sprite finalSprite = null; 

    void Start() {
        Merge();
    }

    public void Merge() {
        Resources.UnloadUnusedAssets();

        var newTex = new Texture2D(spritesToMerge[0].sprite.texture.width, spritesToMerge[0].sprite.texture.height); 

        for (int x = 0; x < newTex.width; x++) {
            for (int y = 0; y < newTex.height; y++) {
                newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
        }

        for (int i = 0; i < spritesToMerge.Length; i++) {
            for (int x = 0; x < spritesToMerge[i].sprite.texture.width; x++) {
                for (int y = 0; y < spritesToMerge[i].sprite.texture.height; y++) {
                    var pixel = spritesToMerge[i].sprite.texture.GetPixel(x, y).a == 0 ? newTex.GetPixel(x, y) : spritesToMerge[i].sprite.texture.GetPixel(x, y);
                    if (spritesToMerge[i].sprite.texture.GetPixel(x, y).a == 0) pixel = newTex.GetPixel(x,y); 
                    else {
                        pixel = spritesToMerge[i].sprite.texture.GetPixel(x, y);
                        Color colorSprite = spritesToMerge[i].color;
                        Color colorPixel = pixel;
                        float r = (0.5f * ((float)System.Math.Pow(colorSprite.r, 2)))+ (0.5f * ((float)System.Math.Pow(colorPixel.r, 2)));
                        float g = (0.5f * ((float)System.Math.Pow(colorSprite.g, 2)))+ (0.5f * ((float)System.Math.Pow(colorPixel.g, 2)));
                        float b = (0.5f * ((float)System.Math.Pow(colorSprite.b, 2)))+ (0.5f * ((float)System.Math.Pow(colorPixel.b, 2)));
                        pixel = new Color(r, g, b, 1);
                    }
                    newTex.SetPixel(x, y, pixel);
                }
            }
        }

        newTex.Apply();
        finalSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(1f, 1f));
        finalSprite.name = "New Sprite"; 
        finalSpriteRenderer.sprite = finalSprite;
    }
}
