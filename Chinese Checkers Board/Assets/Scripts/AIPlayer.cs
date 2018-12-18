using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS IS FOR A 2-PLAYER AI ONLY (the AI and the player must be the top and bottom positions).
// IN THE FOLLOWING CODE: player1 is the bottom player, and player2 is the top player.
public class AIPlayer : Player {
    public int numFutureTurnsCalculated;
    public float priorWeight; // a number between 0.0 and 1.0 that determines the weight assigned to first moves. Vanilla version - set to 1.0

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // This gets called it "manual" is false. It's where the AI is actually given the reigns.
    public override void DoMove()
    {
        VBoard internalVBoard = new VBoard(bm, playerNumber);
        Vector3Int bestMove = new Vector3Int(-1, -1, -1);
        bool doesWin = false;
        bestMove = internalVBoard.PickMove(numFutureTurnsCalculated, priorWeight, ref doesWin);
        // now, if the player does win, we have to make them win in the least number of possible moves.
        Debug.Log("Does Win? " + doesWin);
        if(!doesWin)
        {
            for (int i = numFutureTurnsCalculated-1; i >= 0; i--)
            {
                doesWin = false;
                Vector3Int newBestMove = internalVBoard.PickMove(i, priorWeight, ref doesWin);
                if (!doesWin)
                    break;
                else
                    bestMove = newBestMove;
            }
        }
        if (bestMove.x < 0)
            Debug.Log("Error with the AI - couldn't find a valid move!");
        pieces[bestMove.x].RealizeMove(new Vector2Int(bestMove.y, bestMove.z));
        bm.hasFinishedMove = true;
    }

    // In this class goes all of the actual AI thinking and constants and stuff.
    public class VBoard {
        // These are the width and height of the actual (full) game board
        private int width = 25;
        private int height = 17;
        // This is the list of possible moves:
        private static Vector2Int[] directionSet = {
            new Vector2Int(-2,0),
            new Vector2Int(-1,1),
            new Vector2Int(1,1),
            new Vector2Int(2,0),
            new Vector2Int(1,-1),
            new Vector2Int(-1,-1)
        };

        bool[,] boardArr;
        Vector2Int[] player1Pieces;
        Vector2Int[] player2Pieces;
        // true iff curPlayer is player1
        bool curPlayer;
        bool originalPlayer;

        public VBoard(BoardManager bm, int playerNumber)
        {
            boardArr = new bool[width, height];
            for (int i = 0; i < width; i++) for (int j = 0; j < height; j++)
                    if (bm.board[i, j] != null)
                        boardArr[i, j] = true;
            player1Pieces = new Vector2Int[bm.players[0].pieces.Length];
            for (int i=0;i<bm.players[0].pieces.Length;i++)
                player1Pieces[i] = bm.players[0].pieces[i].bPos;
            player2Pieces = new Vector2Int[bm.players[1].pieces.Length];
            for (int i = 0; i < bm.players[1].pieces.Length; i++)
                player2Pieces[i] = bm.players[1].pieces[i].bPos;

            curPlayer = (playerNumber==0);
            originalPlayer = (playerNumber==0);
        }

        // use this to copy a VBoard, e.g. to test out certain moves:
        public VBoard(VBoard original)
        {
            boardArr = new bool[width, height];
            for (int i = 0; i < width; i++) for (int j = 0; j < height; j++)
                    boardArr[i, j] = original.boardArr[i, j];
            player1Pieces = new Vector2Int[original.player1Pieces.Length];
            for (int i = 0; i < original.player1Pieces.Length; i++)
                player1Pieces[i] = original.player1Pieces[i];
            player2Pieces = new Vector2Int[original.player2Pieces.Length];
            for (int i = 0; i < original.player2Pieces.Length; i++)
                player2Pieces[i] = original.player2Pieces[i];

            curPlayer = !original.curPlayer;
            originalPlayer = original.originalPlayer;
        }

        Vector2Int[] GetMoves(Vector2Int marblePos)
        {
            // NOTE: when we add to a "List<>" object, it automatically adds to the end, so when we loop through the jumpMoves, we can do so while adding elements.
            // Unlike the player, there is no ability to pass your turn.
            List<Vector2Int> validMoves = new List<Vector2Int>();
            // if we are "walking" the marble
            foreach(Vector2Int direction in directionSet)
                if (BoardManager.IsOnBoard(marblePos + direction) && boardArr[marblePos.x + direction.x, marblePos.y + direction.y] == false)
                    validMoves.Add(marblePos + direction);

            // if we are "jumping" the marble:
            List<Vector2Int> jumpMoves = new List<Vector2Int> { marblePos };
            for (int i = 0; i < jumpMoves.Count; i++)
            {
                Vector2Int curMarblePos = jumpMoves[i];
                foreach (Vector2Int direction in directionSet)
                    if (BoardManager.IsOnBoard(curMarblePos + direction * 2) &&
                           boardArr[curMarblePos.x + direction.x, curMarblePos.y + direction.y] == true &&
                           boardArr[curMarblePos.x + 2 * direction.x, curMarblePos.y + 2 * direction.y] == false)
                        if (!jumpMoves.Contains(curMarblePos + direction * 2))
                        {
                            jumpMoves.Add(curMarblePos + direction * 2);
                            validMoves.Add(curMarblePos + direction * 2);
                        }
            }

            Vector2Int[] ret = new Vector2Int[validMoves.Count];
            validMoves.CopyTo(ret);
            return ret;
        }

        // HERE we finally produce an optimal move for the AI.
        // The Vector3Int is really just a hack - the first coordinate is the 
        // id of the marble to move, and the second and third coordinates are the new position.
        // if the first coordinate is negative, an error has occured.
        // just like with EvaluateBoard(int), we maximize iff we are player1
        public Vector3Int PickMove(int n, float priorWeight, ref bool doesWin)
        {
            Vector3Int ret = new Vector3Int(-1, -1, -1);
            if (n == 0)
                return ret;
            float optimalValue = curPlayer ? Mathf.NegativeInfinity : Mathf.Infinity;
            Vector2Int[] myPieces = curPlayer ? player1Pieces : player2Pieces;
            for (int i = 0; i < myPieces.Length; i++)
            {
                if (doesWin)
                    break;
                Vector2Int piece = myPieces[i];
                foreach (Vector2Int moveToPos in GetMoves(piece))
                {
                    if (doesWin)
                        break;
                    VBoard newBoard = new VBoard(this);
                    newBoard.boardArr[piece.x, piece.y] = false;
                    newBoard.boardArr[moveToPos.x, moveToPos.y] = true;
                    (curPlayer ? newBoard.player1Pieces : newBoard.player2Pieces)[i] = moveToPos;
                    float newValue = newBoard.EvaluateBoard(n-1, priorWeight, ref doesWin);
                    if (doesWin || (curPlayer ? (newValue > optimalValue) : (newValue < optimalValue)))
                    {
                        ret = new Vector3Int(i, moveToPos.x, moveToPos.y);
                        optimalValue = newValue;
                    }
                }
            }

            Debug.Log("Optimal Value: " + optimalValue);
            return ret;
        }

        // evaluates the board from player1's perspective
        // we maximize iff curPlayer is true
        // doesn't work if n<0!
        float EvaluateBoard(int n, float priorWeight, ref bool doesWin)
        {
            if (n == 0)
                return EvaluateBoard(ref doesWin);
            CheckIfWin(ref doesWin);
            if (doesWin == true)
                return 0;
            // short-circuit if you are going to win in fewer moves!
            float optimalValue = curPlayer ? Mathf.NegativeInfinity : Mathf.Infinity;
            Vector2Int[] myPieces = curPlayer ? player1Pieces : player2Pieces;
            for(int i=0;i<myPieces.Length;i++)
            {
                if (doesWin)
                    break;
                Vector2Int piece = myPieces[i];
                foreach (Vector2Int moveToPos in GetMoves(piece))
                {
                    if (doesWin)
                        break;
                    VBoard newBoard = new VBoard(this);
                    newBoard.boardArr[piece.x, piece.y] = false;
                    newBoard.boardArr[moveToPos.x, moveToPos.y] = true;
                    (curPlayer ? newBoard.player1Pieces : newBoard.player2Pieces)[i] = moveToPos;
                    float newValue = newBoard.EvaluateBoard(n - 1, priorWeight, ref doesWin);
                    if (doesWin || (curPlayer ? (newValue > optimalValue) : (newValue < optimalValue)))
                        optimalValue = newValue;
                }
            }
            float currentValue = EvaluateBoard(ref doesWin);
            return currentValue * priorWeight + optimalValue * (1-priorWeight);
        }

        // remember: a high score means that Player1 has a good board.
        // NEVER set doesWin to false.
        float EvaluateBoard(ref bool doesWin)
        {
            CheckIfWin(ref doesWin);
            return
                +(0.04f) * (-Mathf.Sqrt(GetVerticalVariance(player1Pieces)) + Mathf.Sqrt(GetVerticalVariance(player2Pieces)))
                +(4.0f) * (GetAverageVertical(player1Pieces) + GetAverageVertical(player2Pieces))
                + (0.3f) * (GetHorizontalDeviation(player2Pieces) - GetHorizontalDeviation(player1Pieces))
                +(1.0f) * (GetProblemSpot(player2Pieces, false) - GetProblemSpot(player1Pieces, true));
            //+ (0.01f) * (-GetHorizontalVariance(player1Pieces) + GetHorizontalVariance(player2Pieces))
            //+ (0.0f) * (GetMaximumVertical(player1Pieces) + GetMinimumVertical(player2Pieces))
            //+ (0.0f) * (-GetMinimumVertical(player1Pieces) - GetMaximumVertical(player2Pieces));
        }

        void CheckIfWin(ref bool doesWin)
        {
            if (originalPlayer)
            {
                bool thisIsWin = true;
                foreach (Vector2Int piece in player1Pieces)
                    if (piece.y < 13)
                    {
                        thisIsWin = false;
                        break;
                    }
                if (thisIsWin)
                    doesWin = true;
            }
            else
            {
                bool thisIsWin = true;
                foreach (Vector2Int piece in player2Pieces)
                    if (piece.y > 3)
                    {
                        thisIsWin = false;
                        break;
                    }
                if (thisIsWin)
                {
                    doesWin = true;
                    Debug.Log("meh");
                }
            }
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
            float avg = GetAverageVertical(playerPieces);
            float ret = 0;
            foreach (Vector2 piece in playerPieces)
                ret += (piece.x - avg) * (piece.x - avg);
            return ret;
        }

        // the total horizontal deviation for one player
        float GetHorizontalDeviation(Vector2Int[] playerPieces)
        {
            float avg = 12;
            float ret = 0;
            foreach (Vector2 piece in playerPieces)
                ret += Mathf.Abs(piece.x - avg);
            return ret;
        }

        // the average y-coordinate for one player
        float GetAverageVertical(Vector2Int[] playerPieces)
        {
            float avg = 0;
            foreach (Vector2Int piece in playerPieces)
                avg += piece.y;
            avg /= player1Pieces.Length;
            return avg;
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
        
        // returns 1 if there is a piece at the one bad spot of the board - 0 otherwise.
        float GetProblemSpot(Vector2Int[] playerPieces, bool player)
        {
            Vector2Int problemSpot = player ? new Vector2Int(12, 12) : new Vector2Int(12, 4);
            foreach (Vector2Int piece in playerPieces)
            {
                if (piece == problemSpot)
                    return 1;
            }
            return 0;
        }
    }

}
