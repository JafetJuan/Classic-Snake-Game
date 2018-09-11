using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int maxHeight = 15;
    public int maxWidth = 17;

    public Color color1;
    public Color color2;

    private GameObject mapObject;
    private SpriteRenderer mapRederer;
    
    private void Start()
    {
        CreateMap();
    }

    //Method that create the hole world for our snake
    private void CreateMap()
    {
        // Create the new map game object and assign his renderer
        mapObject = new GameObject("Map");
        mapRederer = mapObject.AddComponent<SpriteRenderer>();
        
        // New texture of all wold
        Texture2D texture = new Texture2D(maxWidth, maxHeight);
        
        // Iterate on all pixels, and put in the first color or second color
        #region Map
        for (int x = 0; x < maxWidth; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                if (x % 2 != 0)
                {
                    if (y % 2 != 0)
                    {
                        texture.SetPixel(x, y, color1);
                    }
                    else
                    {
                        texture.SetPixel(x, y, color2);
                    }
                }
                else
                {
                    if (y % 2 != 0)
                    {
                        texture.SetPixel(x, y, color2);
                    }
                    else
                    {
                        texture.SetPixel(x, y, color1);
                    }
                } 
            }
        }
        #endregion
        
        // Set the filter mode for prevent blur texture    
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        
        // Create a new rect, sprite with our texture and assign to the renderer of the game object of the map
        Rect rect = new Rect(0, 0, maxWidth, maxHeight);
        Sprite sprite = Sprite.Create(texture, rect, Vector2.one * 0.5f, 1, 0, SpriteMeshType.FullRect);
        mapRederer.sprite = sprite;
    }
}
