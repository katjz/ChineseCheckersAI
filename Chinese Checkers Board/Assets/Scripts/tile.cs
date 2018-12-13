using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tile : MonoBehaviour {

    public GameObject highlighttile;
    public BoardManager bm;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseEnter()
    {
        bm.tileglobal = GetComponent<Transform>().position;
        bm.tileglobal.y = 4;
        highlighttile.transform.position = bm.tileglobal;
        bm.tilelocal.x = GetComponent<Transform>().localPosition.x;
        bm.tilelocal.y = GetComponent<Transform>().localPosition.z;
        bm.overboard += 1;
    }

    public void OnMouseOver()
    {
        if (!bm.isSelectingTarget && Input.GetMouseButtonDown(0))
            bm.clicktojump = true;
    }

    public void OnMouseExit()
    {
        bm.overboard = bm.overboard - 1;
    }
}
