using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniSpriteMerger : MonoBehaviour 
{
    [SerializeField] private Sprite[] staticSprites;
    [SerializeField] private Sprite[] colorSprites;
    private List<Sprite> coloredSprites = new List<Sprite>();
    
    void Start()
    {
        aniMerge();
    }

    public void aniMerge() {
        if (staticSprites.Length != colorSprites.Length) {
            Debug.Log("Different number of sprites");
            return;
        }

        if (staticSprites.Length <= 0 || colorSprites.Length <= 0) {
            Debug.Log("No sprites");
            return;
        }
        
        coloredSprites.Clear();
        Color color = transform.parent.transform.GetChild(3).GetComponent<SpriteRenderer>().color;

        Sprite tmp;
        for (int i = 0; i < staticSprites.Length; i++) {
            tmp = concreteMerge(staticSprites[i], colorSprites[i], color, i);
            if (tmp != null) coloredSprites.Add(tmp);
        }
    }

    public List<Sprite> GetList() {
        return coloredSprites;
    }

    public Sprite concreteMerge(Sprite staticSprite, Sprite colorSprite, Color teamColor, int i) {
        int height = staticSprite.texture.height;
        int width = staticSprite.texture.width;
        bool quad = false;
        if (width == height) quad = true;
        if (!quad && width*2 != height) return null;

        teamColor.a = 1;
        var newTex = new Texture2D(width, height); 
        for (int x = 0; x < newTex.width; x++) {
            for (int y = 0; y < newTex.height; y++) {
                newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
        }

        Color pixel;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (staticSprite.texture.GetPixel(x, y).a == 0) pixel = newTex.GetPixel(x,y); 
                else pixel = staticSprite.texture.GetPixel(x, y);
                newTex.SetPixel(x, y, pixel);
            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (colorSprite.texture.GetPixel(x, y).a == 0) pixel = newTex.GetPixel(x,y); 
                else {
                    pixel = colorSprite.texture.GetPixel(x, y);
                    Color cp = pixel;
                    float r = (0.5f * ((float)System.Math.Pow(teamColor.r, 2)))+ (0.5f * ((float)System.Math.Pow(cp.r, 2)));
                    float g = (0.5f * ((float)System.Math.Pow(teamColor.g, 2)))+ (0.5f * ((float)System.Math.Pow(cp.g, 2)));
                    float b = (0.5f * ((float)System.Math.Pow(teamColor.b, 2)))+ (0.5f * ((float)System.Math.Pow(cp.b, 2)));
                    float a = (0.5f * ((float)System.Math.Pow(teamColor.a, 2)))+ (0.5f * ((float)System.Math.Pow(cp.a, 2)));
                    pixel = new Color(r, g, b, a); 
                }
                newTex.SetPixel(x, y, pixel);
            }
        }

        newTex.Apply();
        newTex.filterMode = FilterMode.Point; 
        newTex.Apply();
        Vector2 piv; 
        if (quad) piv = new Vector2(0.5f, 0.125f); 
        else piv = new Vector2(0.5f, 0.625f); 
        Sprite final = Sprite.Create(newTex, new Rect(0, 0, width, height), piv, 20f, 0, SpriteMeshType.FullRect);
        string name = "NewSprite" + i;
        final.name = name; 
        return final;
    }
}
