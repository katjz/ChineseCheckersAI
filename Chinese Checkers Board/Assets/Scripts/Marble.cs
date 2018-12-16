using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour {

    public Vector2Int bPos;
    [HideInInspector]
    public BoardManager bm;
    // equal to the numeric player minus one (e.g. player 1 would have player = 0)
    [HideInInspector]
    public Player player;
    public GameObject targettoken;
    [HideInInspector]
    public bool click = false; // to check if the player just clicked the marble
    
	// Use this for initialization
	void Start () {
        //GetComponent<Rigidbody>().isKinematic = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (click)
            bm.SetTargetTokenPosition(bPos);
            //targettoken.transform.position = GetComponent<Transform>().position;
    }

    // Moves the marble to the specified location. Returns whether
    // the player was able to move there
    public bool TryMove(Vector2Int dBPos)
    {
        // First: we see if where the player wants to move is on the board and unoccupied
        if (!bm.IsFree(bPos + dBPos))
            return false;
        // NOTE: using dist <= 4, dist==2 doesn't work. I know this is ugly, but I don't know a better way.
        //if ((Mathf.Abs(dBPos.x) == 1 && Mathf.Abs(dBPos.y) == 1) || (Mathf.Abs(dBPos.x) == 1 && Mathf.Abs(dBPos.y) == 0))
        if ((Mathf.Abs(dBPos.x) == 1 && Mathf.Abs(dBPos.y) == 1) || (Mathf.Abs(dBPos.x) == 2 && Mathf.Abs(dBPos.y) == 0))
        {
            if (bm.hasJumped)
                return false;
            RealizeMove(bPos + dBPos);
            bm.hasFinishedMove = true;
            return true;
        }
        else if((Mathf.Abs(dBPos.x) == 2 && Mathf.Abs(dBPos.y) == 2) || (Mathf.Abs(dBPos.x) == 4 && Mathf.Abs(dBPos.y) == 0))
        {
            Vector2Int halfDBPos = new Vector2Int(dBPos.x / 2, dBPos.y / 2);
            if (bm.IsFree(bPos + halfDBPos))
                return false;
            RealizeMove(bPos + dBPos);
            bm.hasJumped = true;
            return true;
        }
        return false;

        //int dist = Mathf.Abs(dBPos.x) + Mathf.Abs(dBPos.y);
        //if(dist<=4)
        //{
        //    if (!bm.hasJumped && bm.IsFree(bPos + dBPos) && dist == 2)
        //    {
        //        RealizeMove(bPos + dBPos);
        //        bm.hasWalked = true;
        //        return true;
        //    }
        //    Vector2Int halfdBPos = new Vector2Int
        //    {
        //        x = dBPos.x / 2,
        //        y = dBPos.y / 2
        //    };
        //    if (dist == 4&&!bm.IsFree(bPos + halfdBPos) && bm.IsFree(bPos + dBPos))
        //    {
        //        RealizeMove(bPos + dBPos);
        //        bm.hasJumped = true;
        //        return true;
        //    }
        //}
        //return false;
    }

    // Assumes that newBPos is in the board, and vacant
    // This is the function that actually moves the marble around.
    public void RealizeMove(Vector2Int newBPos)
    {
        bm.board[bPos.x, bPos.y] = null;
        bm.board[newBPos.x, newBPos.y] = this;
        bPos.x = newBPos.x;
        bPos.y = newBPos.y;
        SetLocation();
        //bm.targetToken.transform.position = this.transform.position;
        bm.SetTargetTokenPosition(bPos);
    }

    //destroys the tiles beneath as a player loses the game
    // TODO: gives lots of errors
    public void OnCollisionEnter(Collision collision)
    {
        if (player == null || bm == null)
            return;
        if(bm.gameEnded&&!player.hasWon)
            Destroy(collision.gameObject);
    }

    //public void OnMouseOver()
    //{
    //    if (!bm.curPlayer.isManual)
    //        return;
    //    if (Input.GetMouseButtonDown(0)&&this.player == bm.curPlayer && bm.isSelectingTarget)//!bm.hasJumped)
    //    {
    //        click = true;
    //        bm.isSelectingTarget = false;
    //        SetLocation();
    //bm.target = this;
    //    }
//}

public void OnMouseDown()
    {
        if (!bm.curPlayer.isManual)
            return;
        if (Input.GetMouseButtonDown(0) && this.player == bm.curPlayer && bm.isSelectingTarget)//!bm.hasJumped)
        {
            click = true;
            bm.isSelectingTarget = false;
            SetLocation();
            bm.SetTargetTokenPosition(bPos);
            //bm.highlightTile.transform.position = bm.outlineTile.transform.position; // moved to BoardManager
        }
    }

    public void OnMouseEnter()
    {
        if (bm == null)
            return;
        bm.overboard += 1;
       //bm.highlightTile.transform.position = bm.GetWorldLocation(bPos);
    }

    public void OnMouseExit()
    {
        if (bm==null)
            return;
        bm.overboard = bm.overboard - 1;
        click = false;
    }

    public void SetLocation()
    {
        transform.localPosition = bm.GetWorldLocation(bPos);
        transform.position += new Vector3(0, 2.0f, 0);
    }

    public bool IsInWinningSquares()
    {
        foreach (Vector2Int winningBPos in player.winningSquares)
            if (bPos == winningBPos)
                return true;
        return false;
    }
}
