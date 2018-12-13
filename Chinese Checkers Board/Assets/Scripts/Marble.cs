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
    public bool click = false; //to check if the player just clicked the marble
    //public bool istarget = false; // true if this marble just jumped

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(click)
            targettoken.transform.position = GetComponent<Transform>().position;
    }

    // Moves the player to the specified location. Returns whether
    // the player was able to move there
    public bool TryMove(Vector2Int dBPos) //oops, Peng modified this
    {
        int dist = Mathf.Abs(dBPos.x) + Mathf.Abs(dBPos.y);
        if(dist<=4)
        {
            if (!bm.hasJumped && bm.IsFree(bPos + dBPos) && dist == 2)
            {
                RealizeMove(bPos + dBPos);
                bm.hasWalked = true;
                return true;
            }
            Vector2Int halfdBPos = new Vector2Int
            {
                x = dBPos.x / 2,
                y = dBPos.y / 2
            };
            if (dist == 4&&!bm.IsFree(bPos + halfdBPos) && bm.IsFree(bPos + dBPos))
            {
                RealizeMove(bPos + dBPos);
                bm.hasJumped = true;
                return true;
            }
        }
        return false;
    }

    // Assumes that newBPos is in the board, and vacant
    public void RealizeMove(Vector2Int newBPos)
    {
        bm.board[bPos.x, bPos.y] = null;
        bm.board[newBPos.x, newBPos.y] = this;
        bPos.x = newBPos.x;
        bPos.y = newBPos.y;
        SetLocation();
        bm.targetToken.transform.position = this.transform.position;
     }

    public void OnMouseOver()
    {
        //// istarget?
        //if (Input.GetMouseButtonDown(0)&&this.player == bm.curPlayer && !bm.hasJumped)
        //{
        //    click = true;
        //    bm.isSelectingTarget = false;
        //    SetLocation();
        //    bm.target = this;
        //}
        if (Input.GetMouseButtonDown(0) && this.player == bm.curPlayer)
        {
            if(!bm.hasJumped)
            {
                click = true;
                bm.isSelectingTarget = false;
                SetLocation();
                bm.target = this;
            }
        }
    }

    public void OnMouseEnter()
    {
        bm.overboard += 1;
    }

    public void OnMouseExit()
    {
        bm.overboard = bm.overboard - 1;
        click = false;
    }

    public void SetLocation()
    {
        transform.localPosition = bm.GetLocalLocation(bPos);
        transform.position += new Vector3(0, 1.5f, 0);
    }

    public bool IsInWinningSquares()
    {
        foreach (Vector2Int winningBPos in player.winningSquares)
            if (bPos == winningBPos)
                return true;
        return false;
    }
}
