using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used for player-specific objects. Feel free to add any functions that would be called for each player
// e.g. in BoardManager, to make the code more readable.
public class Player : MonoBehaviour {

    public Marble[] pieces;
    // the material that will highlight your pieces when selected
    public Material targetMaterial;
    // the material that will highlight your pieces when you're ready to play
    public Material readyMaterial;
    // the index of the "target" when using next/prev keys
    public int targetIndex;
    // the squares that your pieces must be in in order to win
    public Vector2Int[] winningSquares;

	// Use this for initialization
	void Start () {
        targetIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void IncreaseTarget(int n)
    {
        targetIndex += pieces.Length + n;
        targetIndex %= pieces.Length;
    }
}
