using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used for player-specific objects. Feel free to add any functions that would be called for each player
// e.g. in BoardManager, to make the code more readable.
public class Player : MonoBehaviour {

    // this tells the rest of the game whether it can continue with how we've programmed it, or it should wait for the AI to finish its move first.
    public bool isManual;
    public Marble[] pieces;
    // the material that will highlight your pieces when selected
    public Material targetMaterial;
    // the material that will highlight your pieces when you're ready to play
    public int targetIndex;
    // the squares that your pieces must be in in order to win
    public Vector2Int[] winningSquares;
    protected BoardManager bm;
    // this is useful for an AI:
    protected int playerNumber;

    // Use this for initialization
    void Start()
    {
        targetIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetBM(BoardManager bm, int playerNumber)
    {
        this.bm = bm;
        this.playerNumber = playerNumber;
    }

    public void IncreaseTarget(int n)
    {
        targetIndex += pieces.Length + n;
        targetIndex %= pieces.Length;
    }

    // This gets called if "manual" is false.
    public virtual void DoMove()
    {
        Debug.Log("This is a regular player object that doesn't have an automatic setting.");
    }
}
