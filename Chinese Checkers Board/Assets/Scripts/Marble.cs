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
    public bool click = false; //If you click a marble, you set "click" to true

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (bm.isSelectingTarget == true&&Input.GetMouseButtonDown(0))
        {
            click = true;
            bm.isSelectingTarget = false;
        }
		
	}

    // Moves the player to the specified location. Returns whether
    // the player was able to move there
    public bool TryMove(Vector2Int dBPos, ref bool hasJumped)
    {
        if (!hasJumped && bm.IsFree(bPos + dBPos))
        {
            RealizeMove(bPos + dBPos);
            return true;
        }
        else if (!bm.IsFree(bPos + dBPos) && bm.IsFree(bPos + dBPos * 2))
        {
            RealizeMove(bPos + dBPos * 2);
            hasJumped = true;
            return true;
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
    }

    public void OnMouseOver()
    {
        targettoken.transform.position = GetComponent<Transform>().position;
    }

    public void OnMouseEnter()
    {
        bm.overboard += 1;
    }

    public void OnMouseExit()
    {
        bm.overboard = bm.overboard - 1;
    }

    public void SetLocation()
    {
        transform.localPosition = bm.GetWorldLocation(bPos);
    }

    public bool IsInWinningSquares()
    {
        foreach (Vector2Int winningBPos in player.winningSquares)
            if (bPos == winningBPos)
                return true;
        return false;
    }
}
