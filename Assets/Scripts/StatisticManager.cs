using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GamePiece;

namespace TetrisGame
{
    public class StatisticManager
    {
        // Line clears in total
        int clearedLines = 0;
        // Lines cleared with Tetris
        int tetrisClears = 0;
        // Lines cleared without tetris, since last tetris.
        int burnCount = 0;
        // List of all pieces in that game.
        List<GamePiece> gamePieceList = new List<GamePiece>();

        public StatisticManager() { }

        //---------------
        // Incomming Data
        //---------------
        public void AddPieceToStatistic(GamePiece newPiece)
        {
            gamePieceList.Add(newPiece);
        }

        public void AddLineClear(int linecount)
        {
            clearedLines += linecount;
            if (linecount == 4)
            {
                tetrisClears += 4;
                burnCount = 0;
            }
            else
            {
                burnCount += linecount;
            }
        }

        //---------------
        // Outgoing Data
        //---------------
        public float GetTetrisRate()
        {
            if (clearedLines == 0)
            {
                return 100;
            }
            else
            {
                return 100 * tetrisClears / clearedLines;
            }
        }

        public float GetIPieceCount()
        {
            int i_count = 0;
            foreach (var piece in gamePieceList)
            {
                if (piece == GamePiece.i)
                {
                    i_count++;
                }
            }
            return i_count;
        }

        public int GetClearedLines()
        {
            return clearedLines;
        }

        public int GetCountSinceLastI()
        {
            int count = 0;
            foreach (var piece in gamePieceList)
            {
                count++;
                if (piece == GamePiece.i)
                {
                    count = 0;
                }
            }
            return count;
        }

        public int GetBurnCount()
        {
            return burnCount;
        }
    }
}