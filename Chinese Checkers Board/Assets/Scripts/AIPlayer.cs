using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS IS FOR A 2-PLAYER AI ONLY (the AI and the player must be the top and bottom positions).
// IN THE FOLLOWING CODE: player1 is the bottom player, and player2 is the top player.
public class AIPlayer : Player {
    public int numFutureTurnsCalculated;
    [HideInInspector]
    public bool isPlayer1;

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
        Vector3Int bestMove = internalVBoard.PickMove(numFutureTurnsCalculated);
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
        // true iff curPlayer started out as player1
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
                foreach (Vector2Int direction in directionSet)
                    if (BoardManager.IsOnBoard(marblePos + direction * 2) && 
                           boardArr[marblePos.x+direction.x, marblePos.y+direction.y]==true &&
                           boardArr[marblePos.x + 2 * direction.x, marblePos.y + 2 * direction.y] == false)
                        if (!jumpMoves.Contains(marblePos + direction * 2))
                        {
                            jumpMoves.Add(marblePos + direction * 2);
                            validMoves.Add(marblePos + direction * 2);
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
        public Vector3Int PickMove(int n)
        {
            Vector3Int ret = new Vector3Int(-1, -1, -1);
            if (n == 0)
                return ret;
            float optimalValue = curPlayer ? Mathf.NegativeInfinity : Mathf.Infinity;
            bool doesWin = false;
            Vector2Int[] myPieces = curPlayer ? player1Pieces : player2Pieces;
            for (int i = 0; i < myPieces.Length; i++)
            {
                if (doesWin == true)
                    break;
                Vector2Int piece = myPieces[i];
                foreach (Vector2Int moveToPos in GetMoves(piece))
                {
                    if (doesWin == true)
                        break;
                    VBoard newBoard = new VBoard(this);
                    newBoard.boardArr[piece.x, piece.y] = false;
                    newBoard.boardArr[moveToPos.x, moveToPos.y] = true;
                    (curPlayer ? newBoard.player1Pieces : newBoard.player2Pieces)[i] = moveToPos;
                    float newValue = newBoard.EvaluateBoard(n, ref doesWin);
                    if (doesWin || (curPlayer ? (newValue > optimalValue) : (newValue < optimalValue)))
                    {
                        ret = new Vector3Int(i, moveToPos.x, moveToPos.y);
                        optimalValue = newValue;
                    }
                }
            }
            Debug.Log("Optimal Value: " + optimalValue + " won? " + doesWin);
            return ret;
        }

        // evaluates the board from player1's perspective
        // we maximize iff curPlayer is true
        // doesn't work if n<0!
        // TODO: ADDD DOESWIN HERE!!!!!!!!!!!!
        float EvaluateBoard(int n, ref bool doesWin)
        {
            if (n == 0)
                return EvaluateBoard(ref doesWin);
            float ret = curPlayer ? Mathf.NegativeInfinity : Mathf.Infinity;
            Vector2Int[] myPieces = curPlayer ? player1Pieces : player2Pieces;
            for(int i=0;i<myPieces.Length;i++)
            {
                if (doesWin == true)
                    break;
                Vector2Int piece = myPieces[i];
                foreach (Vector2Int moveToPos in GetMoves(piece))
                {
                    if (doesWin == true)
                        break;
                    VBoard newBoard = new VBoard(this);
                    newBoard.boardArr[piece.x, piece.y] = false;
                    newBoard.boardArr[moveToPos.x, moveToPos.y] = true;
                    (curPlayer ? newBoard.player1Pieces : newBoard.player2Pieces)[i] = moveToPos;
                    float newValue = newBoard.EvaluateBoard(n - 1, ref doesWin);
                    if (doesWin || (curPlayer ? (newValue > ret) : (newValue < ret)))
                        ret = newValue;
                }
            }
            return ret;
        }

        // remember: a high score means that Player1 has a good board.
        // don't set doesWin = false. Ever.
        float EvaluateBoard(ref bool doesWin)
        {
            // set doesWin to true iff the original player wins.
            if (originalPlayer)
            {
                bool isWinNow = false;
                foreach (Vector2Int piece in player1Pieces)
                    if (piece.y < 13)
                    {
                        isWinNow = false;
                        break;
                    }
                if(isWinNow)
                    doesWin = true;
            }
            else
            {
                bool isWinNow = false;
                foreach (Vector2Int piece in player1Pieces)
                    if (piece.y > 3)
                    {
                        isWinNow = false;
                        break;
                    }
                if (isWinNow)
                    doesWin = true;
            }

            return (0.00f) * (-Mathf.Sqrt(GetVerticalVariance(player1Pieces)) + Mathf.Sqrt(GetVerticalVariance(player2Pieces)))
                + (4.0f) * (GetAverageVertical(player1Pieces) + GetAverageVertical(player2Pieces))
                + (0.01f) * (-GetHorizontalVariance(player1Pieces) + GetHorizontalVariance(player2Pieces))
                + (0.0f) * (GetMaximumVertical(player1Pieces) + GetMinimumVertical(player2Pieces))
                + (0.0f) * (-GetMinimumVertical(player1Pieces) - GetMaximumVertical(player2Pieces));
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
    }

}
