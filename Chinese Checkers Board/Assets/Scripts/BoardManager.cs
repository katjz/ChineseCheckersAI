using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public int width = 17;
    public int height = 17;
    public Vector3 offset;
    public float scale = 1;

    [HideInInspector]
    public Marble[,] board;
    public Marble[] player1;
    public Marble[] player2;

    private Marble target;

    public GameObject spaceToken;

    public Material player1TargetMaterial;
    public Material player2TargetMaterial;
    public Material neutralTargetMaterial;
    public GameObject targetToken;
    private Vector2Int targetBPos;
    private int target1Index;
    private int target2Index;

    // an int that corresponds to the current turn number
    // use % to get whose turn it is.
    int playerTurn;
    // is true if the current player has just jumped.
    bool hasJumped;
    // true if the player is selecting a target
    // false if the player is actually moving it
    bool isSelectingTarget;

    // Use this for initialization
    void Start()
    {
        board = new Marble[width, height];

        //set up the board here:
        for (int i = 0; i < width; i++) for (int j = 0; j < height; j++)
            {
                Vector2Int vBPos = new Vector2Int(i, j);
                if (IsFree(vBPos))
                    GameObject.Instantiate(spaceToken, GetWorldLocation(vBPos), Quaternion.identity);
            }
        foreach (Marble m in player1)
            if (m != null)
            {
                m.bm = this;
                m.player = 0;
                board[m.bPos.x, m.bPos.y] = m;
                m.SetLocation();
            }
        foreach (Marble m in player2)
            if (m != null)
            {
                m.bm = this;
                m.player = 1;
                board[m.bPos.x, m.bPos.y] = m;
                m.SetLocation();
            }

        playerTurn = 0;
        hasJumped = false;
        isSelectingTarget = true;
        target = player1[0];
        SetTargetPosition(target.bPos);
        target1Index = 0;
        target2Index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Prev"))
        {
            if(playerTurn%2==0)
            {
                target1Index+=player1.Length-1;
                target1Index %= player1.Length;
                SetTargetPosition(player1[target1Index].bPos);
            }
            else
            {
                target2Index+=player1.Length-1;
                target2Index %= player2.Length;
                SetTargetPosition(player2[target2Index].bPos);
            }
        }
        if(Input.GetButtonDown("Next"))
        {
            if (playerTurn % 2 == 0)
            {
                target1Index++;
                target1Index %= player1.Length;
                SetTargetPosition(player1[target1Index].bPos);
            }
            else
            {
                target2Index++;
                target2Index %= player2.Length;
                SetTargetPosition(player2[target2Index].bPos);
            }
        }
        if (isSelectingTarget)
        {
            if (Input.GetButtonDown("End"))
            {
                Marble m = board[targetBPos.x, targetBPos.y];
                if (m!=null && ((m.player - playerTurn) % 2 == 0))
                {
                    isSelectingTarget = false;
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
            if (hasJumped && Input.GetButtonDown("End"))
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
            if (playerTurn % 2 == 0)
                foreach (Marble m in player2)
                    m.gameObject.SetActive(false);
            else
                foreach (Marble m in player1)
                    m.gameObject.SetActive(false);
        }
        playerTurn++;
        // reset the game state:
        hasJumped = false;
        isSelectingTarget = true;
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
        if (m != null && (m.player - playerTurn) % 2 == 0)
        {
            target = m;
            targetToken.GetComponent<MeshRenderer>().material =
                (playerTurn % 2 == 0) ? player1TargetMaterial : player2TargetMaterial;
        }
        else
            targetToken.GetComponent<MeshRenderer>().material =
                neutralTargetMaterial;
    }

    // returns whether (bx, by) is a valid point on the board
    // for a complete board, more rules are necessary
    public bool IsOnBoard(Vector2Int bPos)
    {
        // right coordinates
        if ((bPos.x + bPos.y) % 2 != 0)
            return false;
        // in the bounds of the array
        if (bPos.x < 0 || bPos.x >= width || bPos.y < 0 || bPos.y >= height)
            return false;
        // lower triangle
        if (bPos.y <= 8 && ((bPos.y < bPos.x - 8) || (bPos.y < -bPos.x + 8)))
            return false;
        // upper triangle
        if (bPos.y >= 8 && ((bPos.y > bPos.x + 8) || (bPos.y > -bPos.x + 24)))
            return false;
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

    //tests to see if all of the marbles are IN the end zone.
    // returns true iff the CURRENT PLAYER has won
    private bool GetIsWin()
    {
        if (playerTurn % 2 == 0)
        {
            foreach (Marble m in player1)
                if (m.bPos.y <= 12)
                    return false;
            return true;
        }
        else
        {
            foreach (Marble m in player2)
                if (m.bPos.y >= 4)
                    return false;
            return true;
        }
    }
}
