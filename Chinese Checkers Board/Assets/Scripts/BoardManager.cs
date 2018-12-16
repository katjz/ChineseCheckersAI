using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {
    private int width = 25;
    private int height = 17;

    public Vector3 offset;
    public float scale = 1;

	public CameraController camera;

    [HideInInspector]
    public Marble[,] board;
    public Player[] players;
    // an int that corresponds to the current turn number
    // use % to get whose turn it is.
    [HideInInspector]
    public int playerTurn;
    // exists just for convenience
    [HideInInspector]
    public Player curPlayer;

    // the marble that is currently being manipulated
    [HideInInspector]
    public Marble target;
    
    public Material neutralTargetMaterial;
    public Material miscellaneousMaterial;
    public GameObject targetToken;
    public bool displayTargetToken;
    //private Vector2Int targetBPos;


    //[HideInInspector]
    //public Vector2 tileLocal; //stores the local position of the last tile the mouse hovers over
    //[HideInInspector]
    //public Vector3 tileGlobal; //stores the global position of the last tile the mouse hovers over
    [HideInInspector]
    public Vector2Int tileBPos;

    public Text playerTurnText;
    private string playerTurnString = "Player's Turn: ";

    // is true if the current player has just jumped.
    [HideInInspector]
    public bool hasJumped;
    // true if the current player has walked (and so cannot do other moves). Since re-named to more accurately describe its role with the AI.
    [HideInInspector]
    public bool hasFinishedMove;
    //public bool hasWalked
    // true if the player is selecting a target ; false if the player is actually moving it
    [HideInInspector]
    public bool isSelectingTarget;
    // the outline for the tile the mouse is on.
    public GameObject outlineTile;
    // the highlight for the target's tile
    public GameObject highlightTile;
    [HideInInspector]
    public int overboard; // a count that is used to keep the mouse correct.
    [HideInInspector]
    public bool doingMove; // true if the user has clicked the tile he wants the piece to jump onto, and the move has not been completed
    [HideInInspector]
    public bool gameEnded; // true if the game has ended

    // Use this for initialization
    void Start()
    {
        board = new Marble[width, height];

        int playerNum = 0;
        foreach (Player player in players)
        {
            player.SetBM(this, playerNum);
            foreach (Marble m in player.pieces)
                if (m != null)
                {
                    m.bm = this;
                    m.player = player;
                    board[m.bPos.x, m.bPos.y] = m;
                    m.SetLocation();
                }
            playerNum++;
        }

        overboard = 0;
        playerTurn = 0;
        doingMove = false;
        targetToken.SetActive(displayTargetToken);
        playerTurnText.enabled = true;
        ResetNewTurn();
    }

    private void ResetNewTurn()
    {
        //rotate camera

		curPlayer = players[playerTurn % players.Length];
        // reset the game state:
        hasJumped = false;
        highlightTile.transform.position = new Vector3(100, 4, 50);
        hasFinishedMove = false;
        isSelectingTarget = true;
        highlightTile.GetComponent<Renderer>().material = curPlayer.targetMaterial;
        SetTargetTokenPosition(new Vector2Int(-10, -10));
        target = null;
        playerTurnText.color = curPlayer.targetMaterial.color;
        playerTurnText.text = playerTurnString + (playerTurn % players.Length);
    }

    // Update is called once per frame
    void Update()
    {
        // NOTE Peng: I changed this so that a player can skip their turn if they choose.
        // Also, so that after an AI ends their turn, an Update() must be called before a new turn.
        //if ((hasjumped && input.getbuttondown("end"))||haswalked)
        if(Input.GetButtonDown("End") || hasFinishedMove)
            EndMove();
        else if (doingMove)
        {
            doingMove = false;
            Vector2Int direction = tileBPos - target.bPos;//GetDirectionFromTile();
            target.TryMove(direction);
            //if (direction != Vector2Int.zero)
            //{
            //    target.TryMove(direction);
            //    //if (target.TryMove(direction))
            //    //{
            //    //    //if(hasJumped)
            //    //    //{
            //    //    //    target.istarget = true;
            //    //    //}
            //    //}
            //}
        }
        // if the mouse is not on the board, get rid of the outlineTile by moving it.
        if (overboard < 1)
        {
            SetOutlineLocation(new Vector3(100, 4, 17));
            //highlightTile.transform.position = new Vector3(100, 4, 17);
        }
    }

    // returns the vector from the highlighted tile to the target marble
    //private Vector2Int GetDirectionFromTile()
    //{
    //    Vector2Int dir = new Vector2Int
    //    {
    //        x =(int)tileLocal.x - target.bPos.x+12,
    //        y =(int)(tileLocal.y/1.5f - target.bPos.y+8)
    //    };
    //    return dir;
    //}

    // called by tile : sets the position of the highlightTile as well as tileGlobal
    // also does color and stuff
    public void SetOutlineLocation(Vector3 globalLocation)
    {
        //tileLocal = new Vector2(localLocation.x, localLocation.z);
        //highlightTile.transform.localPosition = new Vector3(localLocation.x, 4, localLocation.z);
        //tileGlobal = globalLocation;
        outlineTile.transform.position = new Vector3(globalLocation.x, 4, globalLocation.z);
        tileBPos = GetVirtualLocation(outlineTile.transform.position);
        if(IsOnBoard(tileBPos))
        {
            Marble m = board[tileBPos.x, tileBPos.y];
            if (m != null && m.player == curPlayer && curPlayer.isManual)
                SetOutlineMaterial(curPlayer.targetMaterial);
            else
                SetOutlineMaterial(neutralTargetMaterial);
        }
        else
            SetOutlineMaterial(neutralTargetMaterial);
    }

    private void EndMove()
    {
        if (GetIsWin())
        {
            curPlayer.hasWon = true;
            gameEnded = true;
            curPlayer.winText.enabled = true;
            //foreach (Player player in players)
            //    if (player != curPlayer)
            //        foreach (Marble m in player.pieces)
            //            //m.gameObject.SetActive(false);
            //            m.gameObject.transform.position += new Vector3(0, 1.0f, 0);
        }
        if(!gameEnded)
            playerTurn++;

        ResetNewTurn();

        if (!gameEnded&&!curPlayer.isManual)
            curPlayer.DoMove();
    }

    //// Call this to move the target -- also changes its color and stuff!
    //private void SetTargetPosition(Vector2Int newPos)
    //{
    //    if (!IsOnBoard(newPos))
    //        return;
    //    target.bPos = newPos;
    //    targetToken.transform.SetPositionAndRotation(GetLocalLocation(target.bPos), Quaternion.identity);
    //    Marble m = board[target.bPos.x, target.bPos.y];
    //    if (m != null && m.player==curPlayer)
    //    {
    //        target = m;
    //        SetTargetMaterial(isSelectingTarget ? m.player.targetMaterial : m.player.readyMaterial);
    //    }
    //    else
    //        SetTargetMaterial(neutralTargetMaterial);
    //}

    // Call this guy instead to move the target token -- also changes its color and stuff!
    public void SetTargetTokenPosition(Vector2Int newPos)
    {
        targetToken.transform.SetPositionAndRotation(GetWorldLocation(newPos), Quaternion.identity);
        if (IsOnBoard(newPos))
        {
            //Marble m = board[target.bPos.x, target.bPos.y];
            Marble m = board[newPos.x, newPos.y];
            if (m != null && m.player == curPlayer)
            {
                target = m;
                SetTargetTokenMaterial(m.player.targetMaterial);
                highlightTile.transform.position = outlineTile.transform.position;
            }
            else
                SetTargetTokenMaterial(neutralTargetMaterial);
        }
        else
            SetTargetTokenMaterial(neutralTargetMaterial);
    }

    private void SetTargetTokenMaterial(Material newMaterial)
    {
        targetToken.GetComponent<Renderer>().material = newMaterial;
    }
    private void SetOutlineMaterial(Material newMaterial)
    {
        outlineTile.GetComponent<Renderer>().material = newMaterial;
    }

    // returns whether bPos is a valid point on the board.
    // is a static method so that VBoard can use it.
    public static bool IsOnBoard(Vector2Int bPos)
    {
        // right coordinates
        if ((bPos.x + bPos.y) % 2 != 0)
            return false;
        // in the bounds of the array
        if (bPos.x < 0 || bPos.x >= 25 || bPos.y < 0 || bPos.y >= 17)
            return false;

        //down-triangle
        if ((bPos.y >= 4) && (bPos.y <= bPos.x + 4) && (bPos.y <= -bPos.x + 28))
            return true;
        //up-triangle
        if ((bPos.y <= 12) && (bPos.y >= bPos.x - 12) && (bPos.y >= -bPos.x + 12))
            return true;
        return false;
    }

    // This will return whether bPos is unoccupied and in bounds.
    public bool IsFree (Vector2Int bPos)
    {
        if (!IsOnBoard(bPos))
            return false;
        return board[bPos.x, bPos.y] == null;
    }

    // returns the (local) world location of a specific bPos.
    public Vector3 GetWorldLocation(Vector2Int bPos)
    {
        return new Vector3((float)(bPos.x) / 2, 0, (float)(bPos.y) * Mathf.Sqrt(3) / 2) * scale + offset;
    }
    // returns the bPos for a specific (local) world location
    public Vector2Int GetVirtualLocation(Vector3 pos)
    {
        Vector3 scaledPos = (pos - offset) / scale;
        return new Vector2Int(Mathf.RoundToInt(scaledPos.x * 2), Mathf.RoundToInt(scaledPos.z * 2 / Mathf.Sqrt(3)));
    }

    // tests to see if all of the CURRENT PLAYER's marbles are IN the end zone.
    // returns true iff the CURRENT PLAYER has won
    private bool GetIsWin()
    {
        foreach (Marble m in curPlayer.pieces)
            if(!m.IsInWinningSquares())
              return false;
        return true;
    }
}
