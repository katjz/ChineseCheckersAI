using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour {

    public int bx, by;
    [HideInInspector]
    public BoardManager bm;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Moves the player to the specified location. Returns whether
    // the player was able to move there
    public bool TryMove(int dx, int dy)
    {
        if(bm.IsFree(bx+dx,by+dy))
        {
            RealizeMove(bx + dx, by + dy);
            return true;
        }
        else if(bm.IsFree(bx+2*dx, by+2*dy))
        {
            RealizeMove(bx + 2 * dx, by + 2 * dy);
            return true;
        }
        return false;
    }

    // Assumes that (newBX, newBY) is in the board, and vacant
    public void RealizeMove(int newBX, int newBY)
    {
        bm.board[bx, by] = null;
        bm.board[newBX, newBY] = this;
        bx = newBX;
        by = newBY;
        SetLocation();
    }

    public void SetLocation()
    {
        transform.localPosition = bm.getWorldLocation(bx, by);
    }
}
