using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteMerger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spritesToMerge = null; 
    [SerializeField] private SpriteRenderer finalSpriteRenderer = null; 
    [SerializeField] private Sprite finalSprite = null; 

    void Start() 
    {
        Merge();
    }

    void Update()
    {
        Merge();
    }

    private void Merge() {
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
                    newTex.SetPixel(x, y, pixel);
                }
            }
        }

        newTex.Apply();
        finalSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
        finalSprite.name = "New Sprite"; 
        finalSpriteRenderer.sprite = finalSprite;
    }
}
