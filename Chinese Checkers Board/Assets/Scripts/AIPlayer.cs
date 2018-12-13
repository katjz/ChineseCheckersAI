using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS IS FOR A 2-PLAYER AI ONLY (the AI and the player must be the top and bottom positions).
// IN THE FOLLOWING CODE: player1 is the bottom player, and player2 is the top player.
public class AIPlayer : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // In this class goes all of the actual AI thinking and constants and stuff.
    public class VBoard {
        // This is the middle collumn of the game board - in the full board case, this is 12.
        private int middleCollumn = 12;
        // These are the width and height of the actual (full) game board
        private int width = 25;
        private int height = 17;

        bool[,] boardArr;
        Vector2Int[] player1Pieces;
        Vector2Int[] player2Pieces;
        int numPlayers = 10;
        int curPlayer;

        public VBoard(BoardManager gameBoard, int playerNumber)
        {
            boardArr = new bool[width, height];
            for (int i = 0; i < width; i++) for (int j = 0; j < height; j++)
                {

                }
            numPlayers = gameBoard.players.Length;
            curPlayer = gameBoard.playerTurn;
        }

        VBoard[] GetMoves(Vector2Int marblePos)
        {
            LinkedList<Vector2Int> possMoves = new LinkedList<Vector2Int>();
            while(possMoves.Dequeue)
            {
                
            }
        }

        // evaluates the board from player1's perspective
        float EvaluateBoard(int n)
        {

        }

        float EvaluateBoard()
        {
            return (0.3f) * (-GetVerticalVariance(player1Pieces) + GetVerticalVariance(player2Pieces))
                + (0.3f) * (-GetHorizontalVariance(player1Pieces) + GetHorizontalVariance(player2Pieces))
                + (0.2f) * (GetMaximumVertical(player1Pieces) + GetMinimumVertical(player2Pieces));
        }

        // the total vertical variance for one player
        float GetVerticalVariance(Vector2Int[] playerPieces)
        {
            float avg = 0;
            foreach (Vector2 piece in playerPieces)
                avg += piece.y;
            avg /= player1Pieces.Length;
            float ret = 0;
            foreach (Vector2 piece in playerPieces)
                ret += (piece.y - avg) * (piece.y - avg);
            return ret;
        }

        // the total horizontal variance for one player
        float GetHorizontalVariance(Vector2Int[] playerPieces)
        {
            float avg = 0;
            foreach (Vector2 piece in playerPieces)
                avg += piece.x;
            avg /= player1Pieces.Length;
            float ret = 0;
            foreach (Vector2 piece in playerPieces)
                ret += (piece.x - avg) * (piece.x - avg);
            return ret;
        }

        // the maximum y-coordinate for one player
        float GetMaximumVertical(Vector2Int[] playerPieces)
        {
            float ret = 0;
            foreach (Vector2Int piece in player1Pieces)
                if (piece.y > ret)
                    ret = piece.y;
            return ret;
        }

        // the minimum y-coordinate for one player
        float GetMinimumVertical(Vector2Int[] playerPieces)
        {
            float ret = height-1;
            foreach (Vector2Int piece in player1Pieces)
                if (piece.y < ret)
                    ret = piece.y;
            return ret;
        }
    }

}
