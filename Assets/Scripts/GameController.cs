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


    // Start is called before the first frame update
    void Start()
    {
       gameField.SetPixelController(pixelController);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerAction playerAction = ReadPlayerInput();
        gameField.HandlePlayerInput(playerAction);

      // if (speedTimer > currentTick)
      //  {
            gameField.Step();
      //  }
        //Update Graphics
        gameField.UpdateDisplay();
    }





    private PlayerAction ReadPlayerInput()
    {
        PlayerAction selectedAction = new PlayerAction();
        selectedAction = PlayerAction.none;

        if (Input.GetKeyDown("left"))
        {
            selectedAction = PlayerAction.moveLeft;
        }
        else if (Input.GetKeyDown("right"))
        {
            selectedAction = PlayerAction.moveRight;
        }
        else if (Input.GetKeyDown("down"))
        {
            selectedAction = PlayerAction.moveDown;
        }
        else if (Input.GetKeyDown("up"))
        {
            selectedAction = PlayerAction.drop;
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
        public Pixel(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    List<Pixel> fieldValue = new List<Pixel>();
    bool[,] currentPiecePosition = new bool[10, 20];
    List<Pixel> pixelsOfCurrentPiece = new List<Pixel>();

    public void Step()
    {


    }

    public void HandlePlayerInput(PlayerAction action)
    {
        if (!MovementAllowed(action))
        {
            return;
        }

        switch (action)
        {
            case PlayerAction.moveDown:
                foreach (Pixel pixel in pixelsOfCurrentPiece)
                {
                    pixel.y -= 1;
                }
                break;
            case PlayerAction.moveLeft:
                foreach (Pixel pixel in pixelsOfCurrentPiece)
                {
                    pixel.x -= 1;
                }
                break;
            case PlayerAction.moveRight:
                foreach (Pixel pixel in pixelsOfCurrentPiece)
                {
                    pixel.x += 1;
                }
                break;
            case PlayerAction.rotateLeft:
                Debug.Log("Rotation needs to be implemented!");
                break;
            case PlayerAction.rotateRight:
                Debug.Log("Rotation needs to be implemented!");
                break;
            default:
                break;
        }
    }

    private GamePiece GetNewRandomPiece()
    {
        int randomIndex = random.Next(0, 8);
        GamePiece newPiece = (GamePiece)randomIndex;
        return newPiece;
    }

    private bool[,] GetStartPositionOfPiece(GamePiece piece)
    {
        bool[,] newPiece = new bool[10, 20];


        //switch(piece)
        switch (GamePiece.l)
        {
            case GamePiece.l:
                newPiece[4, 19] = true;
                newPiece[4, 18] = true;
                newPiece[4, 16] = true;
                newPiece[4, 17] = true;
                break;
            default:
                break;
        }
        return newPiece;

    }

    private bool MovementAllowed(PlayerAction desiredAction)
    {
        return true;
    }

    public void UpdateDisplay()
    {
        /*
        foreach(Pixel pixel in fieldValue)
        {
            pixelController.SetPixel(pixel.x, pixel.y, Color.blue);
        }
        */
          pixelController.SetPixel(2, 2, Color.blue);
    }
}