using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tile : MonoBehaviour {

    public GameObject highlightTile;
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
        if (bm == null)
            return;
        //Vector3 position = GetComponent<Transform>().position;
        //highlightTile.transform.position = position;
        //bm.tileLocal.x = GetComponent<Transform>().localPosition.x;
        //bm.tileLocal.y = GetComponent<Transform>().localPosition.z;

        bm.SetHighlightLocation(GetComponent<Transform>().position);

        bm.overboard += 1;
    }

    public void OnMouseExit()
    {
        if (bm == null)
            return;
        bm.overboard = bm.overboard - 1;
    }

    public void OnMouseOver()
    {
        if (!bm.curPlayer.isManual)
            return;
        if (!bm.isSelectingTarget && Input.GetMouseButtonDown(0))
            bm.doingMove = true;
    }
}
