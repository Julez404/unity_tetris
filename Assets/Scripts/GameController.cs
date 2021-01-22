using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //random function


public enum PlayerAction
{
    none,
    moveLeft,
    moveRight,
    moveDown,
    drop,
    rotateLeft,
    rotateRight,
    store
}


public class GameController : MonoBehaviour
{
    private GameField gameField = new GameField();
    public PixelController pixelController;
    private float lastTime;

    // Start is called before the first frame update
    void Start()
    {
        gameField.SetPixelController(pixelController);
        gameField.Init();
        InvokeRepeating("Cycle", 2.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerAction playerAction = ReadPlayerInput();
        gameField.HandlePlayerInput(playerAction);

        // if (speedTimer > currentTick)
        //  {
        //  }
        //Update Graphics
        gameField.UpdateDisplay();
    }

    void Cycle()
    {
        Debug.Log("Cycle");
        this.ingameField.Step();
    }

    void FixedUpdate()
    {
        if ((lastTime - Time.fixedDeltaTime) > 100000000)
        {
            lastTime = Time.fixedDeltaTime;
            Debug.Log("STEP");
            //gameField.Step();
        }
    }


    private PlayerAction ReadPlayerInput()
    {
        PlayerAction selectedAction = new PlayerAction();
        selectedAction = PlayerAction.none;

        if (Input.GetKeyDown("a"))
        {
            selectedAction = PlayerAction.moveLeft;
        }
        else if (Input.GetKeyDown("d"))
        {
            selectedAction = PlayerAction.moveRight;
        }
        else if (Input.GetKeyDown("s"))
        {
            selectedAction = PlayerAction.moveDown;
        }
        else if (Input.GetKeyDown("w"))
        {
            selectedAction = PlayerAction.drop;
        }
        else if (Input.GetKeyDown("q"))
        {
            selectedAction = PlayerAction.rotateLeft;
        }
        return selectedAction;
    }

}


public class GameField
{
    PixelController pixelController;


    public void SetPixelController(PixelController controller)
    {
        pixelController = controller;
    }

    public GameField()
    {

    }

    //Rendom generator for pieces.
    private System.Random random = new System.Random();


    public enum GamePiece
    {
        none,
        longI,
        s,
        mirroredS,
        l,
        mirroredL,
        square,
        t
    }

    public enum MovementResult
    {
        pieceMoved,
        pieceStored,

        gameOver
    }

    class Pixel
    {
        public int x, y;
        public GamePiece gamePiece = GamePiece.none;

        public Pixel(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Pixel(int x, int y, GamePiece gamePiece)
        {
            this.x = x;
            this.y = y;
            this.gamePiece = gamePiece;
        }
    }

    int[,] displayPixels = new int[10, 20];
    List<Pixel> currentPiecePixels = new List<Pixel>();


    public void Init()
    {
        LoadNewPiece(GamePiece.longI);
        CopyToDisplayBuffer();
    }

    private void CopyToDisplayBuffer()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
            displayPixels[pixel.x, pixel.y] = (int)pixel.gamePiece;
        }
    }


    private bool CanMoveDown()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
            //Negative Check
            if (pixel.y == 0)
            {
                return false;
            }
            if (displayPixels[pixel.x, pixel.y - 1] != 0)
            {
                return false;
            }
        }
        return true;
    }


    public void Step()
    {
        Debug.Log("I am a Step");
        //Downmovement Possible?
        if (CanMoveDown())
        {
            RemovePixelsFromDisplay(currentPiecePixels);
            foreach (Pixel pixel in currentPiecePixels)
            {
                pixel.y -= 1;
            }
        }
        else
        {
            bool isGameOver = false;
            if (isGameOver)
            {
                //GameOverEvent
            }
            else
            {
                //Store position
                CopyToDisplayBuffer();
                LoadNewPiece(GetRandomPiece());
            }
        }
    }

    private void RemovePixelsFromDisplay(List<Pixel> pixels)
    {
        foreach (Pixel pixel in pixels)
        {
            displayPixels[pixel.x, pixel.y] = 0;
        }
    }

    private bool CanMoveRight()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
            //Negative Check
            if (pixel.x == 9)
            {
                return false;
            }
            if (displayPixels[pixel.x + 1, pixel.y] != 0)
            {
                return false;
            }
        }
        return true;
    }

    private bool CanMoveLeft()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
            //Negative Check
            if (pixel.x == 0)
            {
                return false;
            }
            if (displayPixels[pixel.x - 1, pixel.y] != 0)
            {
                return false;
            }
        }
        return true;
    }


    public void HandlePlayerInput(PlayerAction action)
    {
        Debug.Log($"HandlePlayerInput: {action}");
        if (!MovementAllowed(action))
        {
            return;
        }

        RemovePixelsFromDisplay(currentPiecePixels);

        switch (action)
        {
            case PlayerAction.moveDown:
                Step();
                break;
            case PlayerAction.moveLeft:
                if (CanMoveLeft())
                {
                    foreach (Pixel pixel in currentPiecePixels)
                    {
                        Debug.Log("Move left");
                        pixel.x -= 1;
                    }
                }
                break;
            case PlayerAction.moveRight:
                if (CanMoveRight())
                {
                    foreach (Pixel pixel in currentPiecePixels)
                    {
                        Debug.Log("Move right");
                        pixel.x = pixel.x + 1;
                    }
                }
                break;
            case PlayerAction.rotateLeft:
                Step();
                Debug.Log("Rotation needs to be implemented!");
                break;
            case PlayerAction.rotateRight:
                Debug.Log("Rotation needs to be implemented!");
                break;
            default:
                break;
        }

        CopyToDisplayBuffer();
    }

    private GamePiece GetRandomPiece()
    {
        int randomIndex = random.Next(0, 8);
        GamePiece newPiece = (GamePiece)randomIndex;
        return newPiece;
    }

    private void LoadNewPiece(GamePiece piece)
    {
        currentPiecePixels = new List<Pixel>();

        //switch(piece)
        switch (piece)
        {
            case GamePiece.longI:
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(5, 17, piece));
                currentPiecePixels.Add(new Pixel(5, 16, piece));
                break;
            case GamePiece.square:
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(4, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(4, 18, piece));
                break;
            case GamePiece.t:
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(4, 18, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(6, 18, piece));
                break;
            case GamePiece.l:
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(5, 17, piece));
                currentPiecePixels.Add(new Pixel(6, 17, piece));
                break;
            case GamePiece.mirroredL:
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(5, 17, piece));
                currentPiecePixels.Add(new Pixel(4, 17, piece));
                break;
            case GamePiece.s:
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(6, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(4, 18, piece));
                break;
            case GamePiece.mirroredS:
                currentPiecePixels.Add(new Pixel(4, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(6, 18, piece));
                break;
            default:
                break;
        }
    }

    private bool MovementAllowed(PlayerAction desiredAction)
    {
        return true;
    }

    public void UpdateDisplay()
    {
        for (int col = 0; col < 10; col++)
        {
            for (int row = 0; row < 20; row++)
            {
                pixelController.SetPixel(col, row, GetColorByPiece((GamePiece)displayPixels[col, row]));
            }
        }
    }

    private Color GetColorByPiece(GamePiece gamePiece)
    {
        switch (gamePiece)
        {
            case GamePiece.none:
                return Color.grey;
            case GamePiece.l:
                return Color.blue;
            case GamePiece.longI:
                return Color.cyan;
            case GamePiece.mirroredL:
                return new Color(1, 0.65f, 0, 1);
            case GamePiece.t:
                return new Color(0.5f, 0, 1, 1);
            case GamePiece.mirroredS:
                return Color.green;
            case GamePiece.s:
                return Color.red;
            default:
                return Color.white;
        }
    }
}
