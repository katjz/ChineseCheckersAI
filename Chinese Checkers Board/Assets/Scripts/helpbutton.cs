using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class helpbutton : MonoBehaviour {
    public BoardManager bm;
    public Text text;

    public void OnMouseDown()
    {
        Behaviour halo = (Behaviour)this.GetComponent("Halo");
        if (Input.GetMouseButtonDown(0))
        {
            halo.enabled = !halo.enabled ? true : false;
            text.enabled = halo.enabled ? true : false;
        }
    }
}
