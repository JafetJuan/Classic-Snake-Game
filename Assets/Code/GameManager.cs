using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int maxHeight = 15;
    public int maxWidth = 17;

    public Color color1;
    public Color color2;
    public Color foodColor = Color.red;
    public Color playerColor = Color.black;

    public Transform cameraHolder;

    private GameObject playerObj;
    private GameObject foodObj;
    private Node playerNode;
    private Node foodNode;

    private GameObject mapObject;
    private SpriteRenderer mapRederer;

    private Node[,] grid;
    private List<Node> availableNodes = new List<Node>();

    private bool up, down, left, right;
    private float timer;
    public float moveRate = 0.5f;
    
    private Direction curDirection;
    
    public enum Direction
    {
        up,down,left,right
    }
    
    #region Init
    private void Start()
    {
        CreateMap();
        PlacePlayer();
        PlaceCamera();
        CreateFood();
        curDirection = Direction.right;
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
                availableNodes.Add(node);

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
        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
        mapRederer.sprite = sprite;
    }
    
    //Method to initialize the player
    private void PlacePlayer()
    {
        playerObj = new GameObject("playerColor");
        SpriteRenderer playerRenderer = playerObj.AddComponent<SpriteRenderer>();
        playerRenderer.sprite = CreateSprite(playerColor);
        playerRenderer.sortingOrder = 1;
        playerNode = GetNode(3, 3);
        playerObj.transform.position = playerNode.worldPosition;
    }

    private void PlaceCamera()
    {
        Node node = GetNode(maxWidth / 2, maxHeight / 2);
        Vector3 position = node.worldPosition;
        position += Vector3.one * .5f;
        cameraHolder.position = position;
    }

    private void CreateFood()
    {
        foodObj = new GameObject("Food");
        SpriteRenderer foodRenderer = foodObj.AddComponent<SpriteRenderer>();
        foodRenderer.sprite = CreateSprite(foodColor);
        foodRenderer.sortingOrder = 1;
        RandomlyPlaceFood();
    }
    #endregion

    #region Update
    private void Update()
    {
        GetInput();
        SetPlayerDirection();
        timer += Time.deltaTime;
        if (timer > moveRate)
        {
            timer = 0f;
            MovePlayer();
        }
    }
    
    private void GetInput()
    {
        up = Input.GetButtonDown("Up");
        down = Input.GetButtonDown("Down");
        left = Input.GetButtonDown("Left");
        right = Input.GetButtonDown("Right");
    }

    private void SetPlayerDirection()
    {
        if (up)
        {
            curDirection = Direction.up;
        }
        else if (down)
        {
            curDirection = Direction.down;
        }
        else if (left)
        {
            curDirection = Direction.left;
        }
        else if(right)
        {
            curDirection = Direction.right;
        }
    }

    private void MovePlayer()
    { 
        int x = 0;
        int y = 0;
        
        switch (curDirection)
        {
            case Direction.up:
                y = 1;
                break;
            case Direction.down:
                y = -1;
                break;
            case Direction.left:
                x = -1;
                break;
            case Direction.right:
                x = 1;
                break;
        }

        Node targetNode = GetNode(playerNode.x + x, playerNode.y + y);
        if (targetNode == null)
        {
            //Game Over
        }
        else
        {
            bool isScore = targetNode == foodNode;

            availableNodes.Remove(playerNode);
            playerObj.transform.position = targetNode.worldPosition;
            playerNode = targetNode;
            availableNodes.Add(playerNode);

            //Tail logic here
            
            if (isScore)
            {
                if (availableNodes.Count > 0)
                {
                    RandomlyPlaceFood();
                }
                else
                {
                    //Winner winner, apple dinner!
                }
            }
        }
    }
    #endregion
    
    #region Utils
    private void RandomlyPlaceFood()
    {
        int randomNodeNumber = Random.Range(0, availableNodes.Count);
        Node node = availableNodes[randomNodeNumber];
        foodObj.transform.position = node.worldPosition;
        foodNode = node;
    }
    // Returns a node from the requested position on the grid
    private Node GetNode(int x, int y)
    {
        if (x < 0 || x > maxWidth - 1 || y < 0 || y > maxHeight - 1)
            return null;

        return grid[x, y];
    }

    //Creates a color sprite and return it
    private Sprite CreateSprite(Color targetColor)
    {
        // New texture of a tile 1x1 with a color parameter
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, targetColor);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        Rect rect = new Rect(0, 0, 1, 1);
        return Sprite.Create(texture, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
    }
    #endregion
}