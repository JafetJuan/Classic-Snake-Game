using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private GameObject tailParent;
    private Node playerNode;
    private Node previousPlayerNode;
    private Node foodNode;
    private Sprite playerSprite;

    private GameObject mapObject;
    private SpriteRenderer mapRederer;

    private Node[,] grid;
    private List<Node> availableNodes = new List<Node>();
    private List<SpecialNode> tail = new List<SpecialNode>();

    private bool up, down, left, right;
    private float timer;
    public float moveRate = 0.5f;
    
    private Direction targetDirection;
    private Direction currentDirection;
    
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
        targetDirection = Direction.right;
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
        playerSprite = CreateSprite(playerColor);
        playerRenderer.sprite = playerSprite;
        playerRenderer.sortingOrder = 1;
        playerNode = GetNode(3, 3);
        
        PlacePlayerObject(playerObj, playerNode.worldPosition);
        playerObj.transform.localScale = Vector3.one * 1.2f;
        
        tailParent = new GameObject("tailParent");
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
            timer = 0;
            currentDirection = targetDirection;
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
            SetDirection(Direction.up);
        }
        else if (down)
        {
            SetDirection(Direction.down);
        }
        else if (left)
        {
            SetDirection(Direction.left);
        }
        else if(right)
        {
            SetDirection(Direction.right);
        }
    }

    private void SetDirection(Direction dir)
    {
        if (!isOpposite(dir))
        {
            targetDirection = dir;
        }   
    }
    
    private void MovePlayer()
    { 
        int x = 0;
        int y = 0;
        
        switch (currentDirection)
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
            if (isTailNode(targetNode))
            {
                //Game Over
            }
            else
            {

                bool isScore = targetNode == foodNode;

                Node previousNode = playerNode;
                availableNodes.Add(previousNode);

                if (isScore)
                {
                    tail.Add(CreateTailNode(previousNode.x, previousNode.y));
                    availableNodes.Remove(previousNode);
                }

                MoveTail();

                PlacePlayerObject(playerObj, targetNode.worldPosition);
                playerNode = targetNode;
                availableNodes.Remove(playerNode);

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
    }

    private void MoveTail()
    {
        Node prevNode = null;

        for (int i = 0; i < tail.Count; i++)
        {
            SpecialNode spNode = tail[i];
            availableNodes.Add(spNode.node);

            if (i == 0)
            {
                prevNode = spNode.node;
                spNode.node = playerNode;
            }
            else
            {
                Node prev = spNode.node;
                spNode.node = prevNode;
                prevNode = prev;
            }

            availableNodes.Remove(spNode.node);
            PlacePlayerObject(spNode.obj, spNode.node.worldPosition);
        }
        
    }
    #endregion
    
    #region Utils

    private bool isOpposite(Direction dir)
    {
        switch (dir)
        {
            default:
            case Direction.up:
                if (currentDirection == Direction.down)
                    return true;
                else
                    return false;
            case Direction.down:
                if (currentDirection == Direction.up)
                    return true;
                else
                    return false;
            case Direction.left:
                if (currentDirection == Direction.right)
                    return true;
                else
                    return false;
            case Direction.right:
                if (currentDirection == Direction.left)
                    return true;
                else
                    return false;
        }
    }

    private bool isTailNode(Node targetNode)
    {
        for (int i = 0; i < tail.Count; i++)
        {
            if (tail[i].node == targetNode)
            {
                return true;
            }
        }

        return false;
    }
    
    private void PlacePlayerObject(GameObject obj, Vector3 pos)
    {
        pos += Vector3.one * .5f;
        obj.transform.position = pos;
    }
    
    private void RandomlyPlaceFood()
    {
        int randomNodeNumber = Random.Range(0, availableNodes.Count);
        Node node = availableNodes[randomNodeNumber];
        PlacePlayerObject(foodObj, node.worldPosition);
        foodNode = node;
    }
    
    // Returns a node from the requested position on the grid
    private Node GetNode(int x, int y)
    {
        if (x < 0 || x > maxWidth - 1 || y < 0 || y > maxHeight - 1)
            return null;

        return grid[x, y];
    }

    SpecialNode CreateTailNode(int x, int y)
    {
        SpecialNode spNode = new SpecialNode();
        spNode.node = GetNode(x, y);
        spNode.obj = new GameObject();
        spNode.obj.transform.parent = tailParent.transform;
        spNode.obj.transform.position = spNode.node.worldPosition;
        spNode.obj.transform.localScale = Vector3.one * .95f;
        SpriteRenderer renderer = spNode.obj.AddComponent<SpriteRenderer>();
        renderer.sprite = playerSprite;
        renderer.sortingOrder = 1;

        return spNode;
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
        return Sprite.Create(texture, rect, Vector2.one*.5f, 1, 0, SpriteMeshType.FullRect);
    }
    #endregion
}