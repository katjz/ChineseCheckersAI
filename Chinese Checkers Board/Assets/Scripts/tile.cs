using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tile : MonoBehaviour {

    public GameObject highlighttile;
    public BoardManager bm;
    Vector3 pos;
    Vector3 posmouse; 
    public bool lastover = false; //true if it is the last tile the mouse hovers over
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
	}

    public void OnMouseEnter()
    {
        bm.lastpos = GetComponent<Transform>().position;
        bm.lastpos.y = 4;
        highlighttile.transform.position = bm.lastpos;
        bm.overboard += 1;
    }

    public void OnMouseExit()
    {
        bm.overboard = bm.overboard - 1;
    }
}
