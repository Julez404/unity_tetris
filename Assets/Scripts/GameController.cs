using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public GameField gameField;

    public Text storedPieceText;
    public Text scoreText;

    public PixelController pixelController;
    PlayerAction currentPlayerAction = PlayerAction.none;
    PlayerAction lastPlayerAction = PlayerAction.none;

    // Handling player Input
    bool isSameActionAsBefore = false;
    ActionTimeCounter inputDelayTimer = new ActionTimeCounter(0.1f);
    ActionTimeCounter inputCycleTimer = new ActionTimeCounter(0.05f);

    // Handling game speed
    ActionTimeCounter pushDownTimer = new ActionTimeCounter(1f);

    // Start is called before the first frame update
    void Start()
    {
        gameField.SetPixelController(pixelController);
        gameField.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameField.isGameOver)
        {
            currentPlayerAction = ReadPlayerInput();

            if (currentPlayerAction != lastPlayerAction)
            {
                isSameActionAsBefore = false;
            }

            //Increase timers.
            pushDownTimer.AddTime(Time.deltaTime);
            inputDelayTimer.AddTime(Time.deltaTime);
            inputCycleTimer.AddTime(Time.deltaTime);


            if (isSameActionAsBefore)
            {
                if (inputDelayTimer.IsReached())
                {
                    if (inputCycleTimer.IsReached())
                    {
                        inputCycleTimer.Clear();
                        gameField.HandlePlayerInput(currentPlayerAction);
                    }
                }
                else
                {
                    inputCycleTimer.Clear();
                }
            }
            else
            {
                gameField.HandlePlayerInput(currentPlayerAction);
                inputDelayTimer.Clear();
                isSameActionAsBefore = true;
            }

            if (pushDownTimer.IsReached())
            {
                pushDownTimer.Clear();
                gameField.Push();
            }

            gameField.CalculatePreviewPixels();
            gameField.UpdateDisplay();

            lastPlayerAction = currentPlayerAction;
        }
        else
        {
            gameField.SetGameOverScreen();
        }
    }

    void FixedUpdate()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        //Hold
        storedPieceText.text = gameField.storedPiece.ToString();

        //Score
        scoreText.text = gameField.score.ToString();
    }

    private PlayerAction ReadPlayerInput()
    {
        PlayerAction selectedAction = new PlayerAction();
        selectedAction = PlayerAction.none;

        if (Input.GetKey("a"))
        {
            selectedAction = PlayerAction.moveLeft;
        }
        else if (Input.GetKey("d"))
        {
            selectedAction = PlayerAction.moveRight;
        }
        else if (Input.GetKey("s"))
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

    private class ActionTimeCounter
    {
        float timer;
        float threshold = 0;

        public ActionTimeCounter(float threshold)
        {
            this.threshold = threshold;
        }

        public void AddTime(float time)
        {
            timer += time;
        }

        public void SetThreshold(float time)
        {
            threshold = time;
        }

        public bool IsReached()
        {
            return timer > threshold;
        }

        public void Clear()
        {
            timer = 0f;
        }
    }
}



