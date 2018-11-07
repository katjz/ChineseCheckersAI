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

    // an int that corresponds to the current turn number
    // use % to get whose turn it is.
    int playerTurn;
    // is true if the player has just jumped.
    bool hasJumped;

    // Use this for initialization
    void Start()
    {
        board = new Marble[width, height];

        //set up the board here:
        for (int i = 0; i < width; i++) for (int j = 0; j < height; j++)
            {
                if (IsFree(i, j))
                    GameObject.Instantiate(spaceToken, getWorldLocation(i, j), Quaternion.identity);
            }
        foreach (Marble m in player1)
            if (m != null)
            {
                Debug.Log(m);
                m.bm = this;
                board[m.bx, m.by] = m;
                m.SetLocation();
            }
        foreach (Marble m in player2)
            if (m != null)
            {
                m.bm = this;
                board[m.bx, m.by] = m;
                m.SetLocation();
            }

        playerTurn = 0;

        if (player1.Length > 0)
            target = player1[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (Input.GetButtonDown("UpL"))
            {
                if (target.TryMove(-1, 1))
                    EndMove();
            }
            else if (Input.GetButtonDown("UpR"))
            {
                if (target.TryMove(1, 1))
                    EndMove();
            }
            else if (Input.GetButtonDown("Left"))
            {
                if (target.TryMove(-2, 0))
                    EndMove();
            }
            else if (Input.GetButtonDown("Right"))
            {
                if (target.TryMove(2, 0))
                    EndMove();
            }
            else if (Input.GetButtonDown("DownL"))
            {
                if (target.TryMove(-1, -1))
                    EndMove();
            }
            else if (Input.GetButtonDown("DownR"))
            {
                if (target.TryMove(1, -1))
                    EndMove();
            }
            else if(Input.GetButtonDown("Cent"))
            {
                EndMove();
            }
        }
    }

    void EndMove()
    {
        //TODO: write this function for selecting targets, etc.
        playerTurn++;
        if (playerTurn % 2 == 0)
            target = player1[0];
        else
            target = player2[0];
    }

    // returns whether (bx, by) is a valid point on the board
    // for a complete board, more rules are necessary
    public bool IsOnBoard(int bx, int by)
    {
        // right coordinates
        if ((bx + by) % 2 != 0)
            return false;
        // in the bounds of the array
        if (bx < 0 || bx >= width || by < 0 || by >= height)
            return false;
        // lower triangle
        if (by <= 8 && ((by < bx - 8) || (by < -bx + 8)))
            return false;
        // upper triangle
        if (by >= 8 && ((by > bx + 8) || (by > -bx + 24)))
            return false;
        return true;
    }

    // This will return whether (bx, by) is unoccupied and in bounds.
    public bool IsFree (int bx, int by)
    {
        if (!IsOnBoard(bx, by))
            return false;
        return board[bx, by] == null;
    }

    // returns the world location of a specific (bx, by) coordinate.
    public Vector3 getWorldLocation(int bx, int by)
    {
        return new Vector3((float)bx / 2, 0, (float)(by) * Mathf.Sqrt(3) / 2) * scale + offset;
    }
}
