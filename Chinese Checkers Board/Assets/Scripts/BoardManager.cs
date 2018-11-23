using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    private int width = 25;
    private int height = 17;
    public Vector3 offset;
    public float scale = 1;

    [HideInInspector]
    public Marble[,] board;
    public Player[] players;
    // an int that corresponds to the current turn number
    // use % to get whose turn it is.
    int playerTurn;
    // exists just for convenience
    Player curPlayer;

    private Marble target;

    public Material neutralTargetMaterial;
    public GameObject targetToken;
    private Vector2Int targetBPos;

    // is true if the current player has just jumped.
    bool hasJumped;
    // true if the player is selecting a target
    // false if the player is actually moving it
    bool isSelectingTarget;

    // Use this for initialization
    void Start()
    {
        board = new Marble[width, height];

        foreach (Player player in players)
            foreach (Marble m in player.pieces)
                if(m!=null)
                {
                    m.bm = this;
                    m.player = player;
                    board[m.bPos.x, m.bPos.y] = m;
                    m.SetLocation();
                }

        playerTurn = 0;
        curPlayer = players[0];
        hasJumped = false;
        isSelectingTarget = true;
        target = curPlayer.pieces[curPlayer.targetIndex];
        SetTargetPosition(target.bPos);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Prev"))
        {
            curPlayer.IncreaseTarget(-1);
            SetTargetPosition(curPlayer.pieces[curPlayer.targetIndex].bPos);
        }
        if(Input.GetButtonDown("Next"))
        {
            curPlayer.IncreaseTarget(+1);
            SetTargetPosition(curPlayer.pieces[curPlayer.targetIndex].bPos);
        }
        if (isSelectingTarget)
        {
            if (Input.GetButtonDown("End"))
            {
                Marble m = board[targetBPos.x, targetBPos.y];
                if (m!=null && (m.player == curPlayer))
                {
                    isSelectingTarget = false;
                    SetTargetMaterial(curPlayer.readyMaterial);
                }
                else
                {
                    Debug.Log("You're playing the wrong thing!");
                }
            }
            else
            {
                Vector2Int direction = GetDirectionFromInput();
                if (direction != Vector2Int.zero && IsOnBoard(targetBPos + direction))
                    SetTargetPosition(targetBPos + direction);
            }
        }
        else
        {
            if (!hasJumped && Input.GetButtonDown("End"))
            {
                isSelectingTarget = true;
                SetTargetMaterial(curPlayer.targetMaterial);
            }
            else if (hasJumped && Input.GetButtonDown("End"))
                EndMove();
            else
            {
                Vector2Int direction = GetDirectionFromInput();
                if (direction != Vector2Int.zero)
                    if (target.TryMove(direction, ref hasJumped))
                    {
                        if (hasJumped)
                            SetTargetPosition(target.bPos);
                        else
                          EndMove();
                    }
            }
        }
    }

    private Vector2Int GetDirectionFromInput()
    {
        if (Input.GetButtonDown("UpL"))
            return new Vector2Int(-1, 1);
        else if (Input.GetButtonDown("UpR"))
            return new Vector2Int(1, 1);
        else if (Input.GetButtonDown("Left"))
            return new Vector2Int(-2, 0);
        else if (Input.GetButtonDown("Right"))
            return new Vector2Int(2, 0);
        else if (Input.GetButtonDown("DownL"))
            return new Vector2Int(-1, -1);
        else if (Input.GetButtonDown("DownR"))
            return new Vector2Int(1,-1);
        else if (Input.GetButtonDown("Cent"))
            return new Vector2Int(0, 0);
        return Vector2Int.zero;
    }

    private void EndMove()
    {
        // TODO: have a better winning animation
        if (GetIsWin())
        {
            foreach (Player player in players)
                if (player != curPlayer)
                    foreach (Marble m in player.pieces)
                        m.gameObject.SetActive(false);
        }
        playerTurn++;
        curPlayer = players[playerTurn % players.Length];
        // reset the game state:
        hasJumped = false;
        isSelectingTarget = true;
        targetBPos = curPlayer.pieces[curPlayer.targetIndex].bPos;
        SetTargetPosition(targetBPos);
    }

    // Call this to move the target -- also changes its color and stuff!
    private void SetTargetPosition(Vector2Int newPos)
    {
        if (!IsOnBoard(newPos))
            return;
        targetBPos = newPos;
        targetToken.transform.SetPositionAndRotation(GetWorldLocation(targetBPos), Quaternion.identity);
        Marble m = board[targetBPos.x, targetBPos.y];
        if (m != null && m.player==curPlayer)
        {
            target = m;
            SetTargetMaterial(curPlayer.targetMaterial);
        }
        else
            SetTargetMaterial(neutralTargetMaterial);
    }

    private void SetTargetMaterial(Material newMaterial)
    {
        targetToken.GetComponent<Renderer>().material = newMaterial;
    }

    // returns whether (bx, by) is a valid point on the board
    // for a complete board, more rules are necessary
    // may have to modify for different arrangements of players
    // TODO: do the complete board
    public bool IsOnBoard(Vector2Int bPos)
    {
        // right coordinates
        if ((bPos.x + bPos.y) % 2 != 0)
            return false;
        // in the bounds of the array
        if (bPos.x < 0 || bPos.x >= width || bPos.y < 0 || bPos.y >= height)
            return false;

        //down-triangle
        if ((bPos.y >= 4) && (bPos.y <= bPos.x + 4) && (bPos.y <= -bPos.x + 28))
            return true;
        //up-triangle
        if ((bPos.y <= 12) && (bPos.y >= bPos.x - 12) && (bPos.y >= -bPos.x + 12))
            return true;
        return false;

        // lower triangle
        if (bPos.y <= 8 && ((bPos.y < bPos.x - 8) || (bPos.y < -bPos.x + 8)))
            return false;
        // upper triangle
        if (bPos.y >= 8 && ((bPos.y > bPos.x + 8) || (bPos.y > -bPos.x + 24)))
            return false;
        // lower-left triangle
        // lower-right triangle
        // upper-left triangle
        // upper-right triangle
        return true;
    }

    // This will return whether bPos is unoccupied and in bounds.
    public bool IsFree (Vector2Int bPos)
    {
        if (!IsOnBoard(bPos))
            return false;
        return board[bPos.x, bPos.y] == null;
    }

    // returns the world location of a specific bPos.
    public Vector3 GetWorldLocation(Vector2Int bPos)
    {
        return new Vector3((float)bPos.x / 2, 0, (float)(bPos.y) * Mathf.Sqrt(3) / 2) * scale + offset;
    }

    // tests to see if all of the marbles are IN the end zone.
    // returns true iff the CURRENT PLAYER has won
    // TODO: update this for multiple players
    private bool GetIsWin()
    {
        foreach (Marble m in curPlayer.pieces)
            if(!m.IsInWinningSquares())
              return false;
        return true;
        /**
        if (playerTurn % 2 == 0)
        {
            foreach (Marble m in players[0].pieces)
                if (m.bPos.y <= 12)
                    return false;
            return true;
        }
        else
        {
            foreach (Marble m in players[1].pieces)
                if (m.bPos.y >= 4)
                    return false;
            return true;
        }
        */
    }
}
