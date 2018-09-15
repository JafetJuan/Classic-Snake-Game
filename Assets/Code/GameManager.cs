using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public int maxHeight = 15;
    public int maxWidth = 17;

    public Color color1;
    public Color color2;
    public Color playerColor = Color.black;

    private GameObject playerObj;

    private GameObject mapObject;
    private SpriteRenderer mapRederer;

    private Node[,] grid;

    private void Start()
    {
        CreateMap();
        PlacePlayer();
    }

    //Method that create the hole world for our snake
    private void CreateMap()
    {
        // Create the new map game object and assign his renderer
        mapObject = new GameObject("Map");
        mapRederer = mapObject.AddComponent<SpriteRenderer>();

        // Initialize the grid
        grid = new Node[maxWidth, maxHeight];

        // New texture of all wold
        Texture2D texture = new Texture2D(maxWidth, maxHeight);

        // Iterate on all positions (pixels), and put in the first color or second color

        #region Map Iteration

        for (int x = 0; x < maxWidth; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                Vector3 targetPosition = Vector3.zero;
                targetPosition.x = x;
                targetPosition.y = y;

                Node node = new Node()
                {
                    x = x,
                    y = y,
                    worldPosition = targetPosition
                };

                // Assign the new created node to de grid
                grid[x, y] = node;

                #region Draw pixels

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

                #endregion
            }
        }

        #endregion

        // Set the filter mode for prevent blur texture    
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        // Create a new rect, a sprite with our texture and assign to the renderer of the game object of the map
        Rect rect = new Rect(0, 0, maxWidth, maxHeight);
        Sprite sprite = Sprite.Create(texture, rect, Vector2.one * 0.5f, 1, 0, SpriteMeshType.FullRect);
        mapRederer.sprite = sprite;
    }

    private void PlacePlayer()
    {
        playerObj = new GameObject("playerColor");
        SpriteRenderer playerRenderer = playerObj.AddComponent<SpriteRenderer>();
        playerRenderer.sprite = CreateSprite(playerColor);
        playerRenderer.sortingOrder = 1;

        playerObj.transform.position = GetNode(3, 3).worldPosition;
    }

    private Node GetNode(int x, int y)
    {
        if (x < 0 || x > maxWidth - 1 || y < 0 || y > maxHeight - 1)
            return null;

        return grid[x, y];
    }

    private Sprite CreateSprite(Color targetColor)
    {
        // New texture of a tile 1x1 with a color parameter
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, targetColor);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        Rect rect = new Rect(0, 0, 1, 1);
        return Sprite.Create(texture, rect, Vector2.one * 0.5f, 1, 0, SpriteMeshType.FullRect);
    }
}