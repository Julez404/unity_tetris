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

    private float newTime;

    // Start is called before the first frame update
    void Start()
    {
        gameField.SetPixelController(pixelController);
        gameField.Init();
        //InvokeRepeating("Cycle", 2.0f, 1.0f);
        newTime = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameField.isGameOver)
        {

            PlayerAction playerAction = ReadPlayerInput();
            gameField.HandlePlayerInput(playerAction);

            newTime += Time.deltaTime;
            if (newTime > 1)
            {
                newTime = 0;
                gameField.Push();
            }
            //Update Graphics
            gameField.UpdateDisplay();
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
        else if (Input.GetKeyDown("e"))
        {
            selectedAction = PlayerAction.rotateRight;
        }
        else if (Input.GetKeyDown("space"))
        {
            selectedAction = PlayerAction.store;
        }
        return selectedAction;
    }

}


public class GameField
{
    PixelController pixelController;

    int score = 0;

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

        public Pixel(Pixel pixel)
        {
            this.x = pixel.x;
            this.y = pixel.y;
            this.gamePiece = pixel.gamePiece;
        }

        public override string ToString()
        {
            return $"({x}|{y}) --> {gamePiece}";
        }
    }

    int[,] displayPixels = new int[10, 25];
    List<Pixel> currentPiecePixels = new List<Pixel>();
    Pixel rotationPoint;
    public bool isGameOver = false;

    public void Init()
    {
        LoadNewPiece(GetRandomPiece());
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

    public void Push()
    {
        RemovePixelsFromDisplay(currentPiecePixels);
        Step();
        CopyToDisplayBuffer();
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
            rotationPoint.y -= 1;
        }
        else
        {
            if (IsGameOver())
            {
                RemoveOutOfBouncePixels(currentPiecePixels);
                isGameOver = true;
            }
            else
            {
                //Store position
                CopyToDisplayBuffer();
                RemoveCurrentPiece();
                HandleFullLineDetection();
                LoadNewPiece(GetRandomPiece());
            }
        }
    }

    private bool IsGameOver()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
            if (pixel.y > 19)
            {
                return true;
            }
        }
        return false;
    }

    private void RemoveOutOfBouncePixels(List<Pixel> pixelList)
    {
        List<Pixel> newList = new List<Pixel>();
        foreach (Pixel pixel in pixelList)
        {
            if (pixel.y < 20)
            {
                newList.Add(pixel);
            }
        }
        pixelList = newList;
    }

    private void RemoveCurrentPiece()
    {
        currentPiecePixels = new List<Pixel>();
    }

    private void HandleFullLineDetection()
    {
        //Determine Full Lines
        List<int> fullLines = new List<int>();
        for (int row = 0; row < 20; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                if (displayPixels[col, row] == 0)
                {
                    break;
                }
                if (col == 9)
                {
                    fullLines.Add(row);
                }
            }
        }

        //Remove Lines
        int count = 0;
        foreach (var toDel in fullLines)
        {
            for (int row = toDel - count; row < 20; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    if (row != 19)
                    {
                        displayPixels[col, row] = displayPixels[col, row + 1];
                    }
                    else
                    {
                        displayPixels[col, row] = 0;
                    }
                }
            }
            count++;
        }

        CopyToDisplayBuffer();
        //

        AddValueToScore(0);


        //1-40
        //2-100
        //3-300
        //4-1200
        //for(
    }

    private void AddValueToScore(int linecount)
    {
        Debug.Log("Score calculation not impemented!");
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
        //Debug.Log($"HandlePlayerInput: {action}");

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
                    rotationPoint.x = rotationPoint.x - 1;
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
                    rotationPoint.x = rotationPoint.x + 1;
                }
                break;
            case PlayerAction.rotateLeft:
                RotateLeft();
                break;
            case PlayerAction.rotateRight:
                RotateRight();
                break;
            case PlayerAction.drop:
                Drop();
                break;
            default:
                break;
        }

        CopyToDisplayBuffer();
    }

    private GamePiece GetRandomPiece()
    {
        int randomIndex = random.Next(1, 8);
        GamePiece newPiece = (GamePiece)randomIndex;
        Debug.Log($"New Piece is {newPiece}");
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
                rotationPoint = new Pixel(5, 17);
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
                rotationPoint = new Pixel(5, 19);
                break;
            case GamePiece.l:
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(5, 17, piece));
                currentPiecePixels.Add(new Pixel(6, 17, piece));
                rotationPoint = new Pixel(5, 18);
                break;
            case GamePiece.mirroredL:
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(5, 17, piece));
                currentPiecePixels.Add(new Pixel(4, 17, piece));
                rotationPoint = new Pixel(5, 18);
                break;
            case GamePiece.s:
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(6, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(4, 18, piece));
                rotationPoint = new Pixel(5, 18);
                break;
            case GamePiece.mirroredS:
                currentPiecePixels.Add(new Pixel(4, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 19, piece));
                currentPiecePixels.Add(new Pixel(5, 18, piece));
                currentPiecePixels.Add(new Pixel(6, 18, piece));
                rotationPoint = new Pixel(5, 18);
                break;
            default:
                break;
        }
        bool collision = false;
        do
        {
            collision = false;
            foreach (Pixel pixel in currentPiecePixels)
            {
                if (displayPixels[pixel.x, pixel.y] != 0)
                {
                    collision = true;
                    break;
                }
            }
            if (collision)
            {
                TransformPixelList(0, 1, currentPiecePixels);
                rotationPoint.y += 1;
            }
        } while (collision);
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
            case GamePiece.square:
                return Color.yellow;
            default:
                return Color.white;
        }
    }

    private bool AllAboveRotationPoint(List<Pixel> pixelList)
    {
        foreach (Pixel pixel in pixelList)
        {
            if (pixel.y <= rotationPoint.y)
            {
                return false;
            }
        }
        return true;
    }

    private bool AllRightOfRotationPoint(List<Pixel> pixels)
    {
        foreach (Pixel pixel in pixels)
        {
            if (pixel.x <= rotationPoint.x)
            {
                return false;
            }
        }
        return true;
    }

    private bool TwoAreRightOfRotationPoint(List<Pixel> pixels)
    {
        int count = 0;
        foreach (Pixel pixel in pixels)
        {
            if (pixel.x > rotationPoint.x)
            {
                count++;
            }
        }
        if (count >= 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool TwoAreAboveOfRotationPoint(List<Pixel> pixels)
    {
        int count = 0;
        foreach (Pixel pixel in pixels)
        {
            if (pixel.y > rotationPoint.y)
            {
                count++;
            }
        }
        if (count >= 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void RotateRight()
    {
        List<Pixel> bufferList = new List<Pixel>();

        GamePiece piece = currentPiecePixels[0].gamePiece;

        if (piece == GamePiece.square)
        {
            // Piece does not change on rotation
            return;
        }
        else if (piece == GamePiece.longI)
        {
            if (AllAboveRotationPoint(currentPiecePixels))
            {
                // 3
                List<Pixel> pixelList = new List<Pixel>();
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y + 2, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y - 1, GamePiece.longI));
                currentPiecePixels = pixelList;
            }
            else if (AllRightOfRotationPoint(currentPiecePixels))
            {
                // 4
                List<Pixel> pixelList = new List<Pixel>();
                pixelList.Add(new Pixel(rotationPoint.x - 1, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 2, rotationPoint.y, GamePiece.longI));
                currentPiecePixels = pixelList;
            }
            else if (TwoAreAboveOfRotationPoint(currentPiecePixels))
            {
                // 2
                List<Pixel> pixelList = new List<Pixel>();
                pixelList.Add(new Pixel(rotationPoint.x - 1, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 2, rotationPoint.y + 1, GamePiece.longI));
                currentPiecePixels = pixelList;
            }
            else if (TwoAreRightOfRotationPoint(currentPiecePixels))
            {
                // 1
                List<Pixel> pixelList = new List<Pixel>();
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y + 2, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y - 1, GamePiece.longI));
                currentPiecePixels = pixelList;
            }
            else
            {
                foreach (var pixle in currentPiecePixels)
                {
                    Debug.Log(pixle);
                }
                Debug.Log("Rotate: " + rotationPoint);
                throw new Exception("I fucked Up");
            }
            BoundryCorrection();
            CopyToDisplayBuffer();
            return;
        }
        else
        {
            //Direct Neighbours
            //rotationPoint
            foreach (Pixel pixel in currentPiecePixels)
            {
                //Left to Up
                if (IsHorizontalToRotationPoint(pixel) && IsLeftFromRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(1, 1, pixel));
                }

                //Up to Right
                else if (IsVerticalToRotationPoint(pixel) && IsAboveRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(1, -1, pixel));
                }

                //Right to Down
                else if (IsHorizontalToRotationPoint(pixel) && IsRightRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(-1, -1, pixel));
                }

                //Down To Left
                else if (IsVerticalToRotationPoint(pixel) && IsBelowRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(-1, 1, pixel));
                }

                //TopLeft To TopRight
                else if (IsAboveRotationPoint(pixel) && IsLeftFromRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(2, 0, pixel));
                }

                //TopRight to BottomRight
                else if (IsAboveRotationPoint(pixel) && IsRightRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(0, -2, pixel));
                }

                //BottomRight to BottomLeft
                else if (IsBelowRotationPoint(pixel) && IsRightRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(-2, 0, pixel));
                }

                //BottomLeft to TopLeft
                else if (IsBelowRotationPoint(pixel) && IsLeftFromRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(0, 2, pixel));
                }

                else if (IsSamePositionAsRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(0, 0, pixel));
                }
                else
                {
                    Debug.Log($"No condition for {pixel}");
                    Debug.Log($"Rotation Pixel is {rotationPoint}");
                }
            }
            currentPiecePixels = new List<Pixel>(bufferList);

            BoundryCorrection();

            CopyToDisplayBuffer();
        }
    }


    private void TransformPixelList(int x, int y, List<Pixel> pixelList)
    {
        foreach (var pixel in pixelList)
        {
            pixel.x += x;
            pixel.y += y;
        }
    }


    private bool BoundryCheck()
    {
        if (RightBoundryOverflow() || LeftBoundryOverflow() || CollisionWithMap())
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool RightBoundryOverflow()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
            Debug.Log($"{pixel}");
            if (pixel.x > 9)
            {
                return true;
            }
        }
        return false;
    }

    private bool LeftBoundryOverflow()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
            if (pixel.x < 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool UpperBoundryOverflow()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
            if (pixel.y > 19)
            {
                return true;
            }
        }
        return false;
    }

    private bool BottomBoundryOverflow()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
            if (pixel.y < 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool CollisionWithMap()
    {
        foreach (Pixel pixel in currentPiecePixels)
        {
        }
        return false;
    }

    private void RotateLeft()
    {
        List<Pixel> bufferList = new List<Pixel>();

        GamePiece piece = currentPiecePixels[0].gamePiece;

        if (piece == GamePiece.square)
        {
            // Piece does not change on rotation
            return;
        }
        else if (piece == GamePiece.longI)
        {
            if (AllAboveRotationPoint(currentPiecePixels))
            {
                // 1
                List<Pixel> pixelList = new List<Pixel>();
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y + 2, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y - 1, GamePiece.longI));
                currentPiecePixels = pixelList;
            }
            else if (AllRightOfRotationPoint(currentPiecePixels))
            {
                // 2
                List<Pixel> pixelList = new List<Pixel>();
                pixelList.Add(new Pixel(rotationPoint.x - 1, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 2, rotationPoint.y + 1, GamePiece.longI));
                currentPiecePixels = pixelList;
            }
            else if (TwoAreAboveOfRotationPoint(currentPiecePixels))
            {
                // 4
                List<Pixel> pixelList = new List<Pixel>();
                pixelList.Add(new Pixel(rotationPoint.x - 1, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 2, rotationPoint.y, GamePiece.longI));
                currentPiecePixels = pixelList;
            }
            else if (TwoAreRightOfRotationPoint(currentPiecePixels))
            {
                // 3
                List<Pixel> pixelList = new List<Pixel>();
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y + 2, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y + 1, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y, GamePiece.longI));
                pixelList.Add(new Pixel(rotationPoint.x + 1, rotationPoint.y - 1, GamePiece.longI));
                currentPiecePixels = pixelList;
            }
            return;
        }
        else
        {
            //Direct Neighbours
            //rotationPoint
            foreach (Pixel pixel in currentPiecePixels)
            {
                //Left to Down
                if (IsHorizontalToRotationPoint(pixel) && IsLeftFromRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(1, -1, pixel));
                }

                //Down to Right
                else if (IsVerticalToRotationPoint(pixel) && IsBelowRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(1, 1, pixel));
                }

                //Right to Up
                else if (IsHorizontalToRotationPoint(pixel) && IsRightRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(-1, 1, pixel));
                }

                //Up to Left
                else if (IsVerticalToRotationPoint(pixel) && IsAboveRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(-1, -1, pixel));
                }

                //TopLeft To BottomLeft
                else if (IsAboveRotationPoint(pixel) && IsLeftFromRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(0, -2, pixel));
                }

                //BottomLeft to BottomRight
                else if (IsBelowRotationPoint(pixel) && IsLeftFromRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(2, 0, pixel));
                }

                //BottomRight to TopRight
                else if (IsBelowRotationPoint(pixel) && IsRightRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(0, 2, pixel));
                }

                //TopRight to TopLeft
                else if (IsAboveRotationPoint(pixel) && IsRightRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(-2, 0, pixel));
                }

                else if (IsSamePositionAsRotationPoint(pixel))
                {
                    bufferList.Add(TransformPixel(0, 0, pixel));
                }
                else
                {
                    Debug.Log($"No condition for {pixel}");
                    Debug.Log($"Rotation Pixel is {rotationPoint}");
                }
            }
            currentPiecePixels = new List<Pixel>(bufferList);

            BoundryCorrection();

            CopyToDisplayBuffer();
        }
    }

    private void BoundryCorrection()
    {
        Debug.Log("Validate new Position");
        if (RightBoundryOverflow())
        {
            Debug.Log("Right Boundry Overflow");
            TransformPixelList(-1, 0, currentPiecePixels);
            rotationPoint.x -= 1;
        }
        else if (LeftBoundryOverflow())
        {
            Debug.Log("Left Boundry Overflow");
            TransformPixelList(1, 0, currentPiecePixels);
            rotationPoint.x += 1;
        }
        else if (UpperBoundryOverflow())
        {
            Debug.Log("Upper Boundry Overflow");
            TransformPixelList(0, -1, currentPiecePixels);
            rotationPoint.y -= 1;
        }
        else if (BottomBoundryOverflow())
        {
            Debug.Log("Bottom Boundry Overflow");
            TransformPixelList(0, 1, currentPiecePixels);
            rotationPoint.y += 1;
        }
        Debug.Log("Check Done");
    }

    /// Return new Pixel based on given X/Y Offset
    private Pixel TransformPixel(int x, int y, Pixel pixel)
    {
        return new Pixel(pixel.x + x, pixel.y + y, pixel.gamePiece);
    }

    private bool IsHorizontalToRotationPoint(Pixel pixel)
    {
        if (pixel.y == rotationPoint.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsVerticalToRotationPoint(Pixel pixel)
    {
        if (pixel.x == rotationPoint.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsAboveRotationPoint(Pixel pixel)
    {
        if (pixel.y > rotationPoint.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsBelowRotationPoint(Pixel pixel)
    {
        if (pixel.y < rotationPoint.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsLeftFromRotationPoint(Pixel pixel)
    {
        if (pixel.x < rotationPoint.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsRightRotationPoint(Pixel pixel)
    {
        if (pixel.x > rotationPoint.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsSamePositionAsRotationPoint(Pixel pixel)
    {
        if (pixel.x == rotationPoint.x && pixel.y == rotationPoint.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Drop()
    {
        List<Pixel> bottomPixels = GetGroundFacingPixels(currentPiecePixels);
        int distanceToGroundPiece = GetSmallestDistanceToGround(bottomPixels);

        RemovePixelsFromDisplay(currentPiecePixels);
        TransformPixelList(0, -distanceToGroundPiece, currentPiecePixels);
        rotationPoint.y -= distanceToGroundPiece;
        CopyToDisplayBuffer();
        Step();
    }

    private List<Pixel> GetGroundFacingPixels(List<Pixel> pixelList)
    {
        Dictionary<int, int> usedColumns = new Dictionary<int, int>();

        //Get used Columns
        foreach (Pixel pixel in pixelList)
        {
            if (usedColumns.ContainsKey(pixel.x))
            {
                if (usedColumns[pixel.x] > pixel.y)
                {
                    usedColumns[pixel.x] = pixel.y;
                }
            }
            else
            {
                usedColumns.Add(pixel.x, pixel.y);
            }
        }

        List<Pixel> returnList = new List<Pixel>();
        foreach (var entry in usedColumns)
        {
            returnList.Add(new Pixel(entry.Key, entry.Value));
        }
        return returnList;
    }


    private int GetSmallestDistanceToGround(List<Pixel> pixelList)
    {
        int distance = 20;
        foreach (Pixel pixel in pixelList)
        {
            int pixelDistance = 0;
            for (int row = pixel.y - 1; row >= 0; row--)
            {
                if (displayPixels[pixel.x, row] == 0)
                {
                    pixelDistance++;
                }
                else
                {
                    break;
                }
            }

            if (pixelDistance < distance)
            {
                distance = pixelDistance;
            }
        }

        if (distance == 20)
        {
            return 0;
        }
        else
        {
            return distance;
        }
    }
}
