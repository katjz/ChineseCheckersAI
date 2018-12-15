using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shaketiles : MonoBehaviour
{
    private float timer = 0.0f;
    //private float roundtime = 1.5f;
    private float shakespeed = 10.0f;
    private float shaketime = 0.5f;
    private float shakedist = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    /*void Update()
    {
        // Test shaking board
        timer += Time.deltaTime;
        //float x = Random.Range(shaketime, 2f*shaketime);
        //float y = Random.Range(2f*shaketime, 4f*shaketime);
        shakespeed = shakespeed*1.02f;
        //shaketime = shaketime / 1.02f;
        if (this.transform.position.x >= 14.8-shakedist)
        {
            this.transform.Translate(Vector3.left * shakespeed * Time.deltaTime);
        }
        else if (this.transform.position.x <= 14.8+shakedist)
        {
            this.transform.Translate(Vector3.right * shakespeed * Time.deltaTime);
        }
        else if (this.transform.position.x >= 14.8)
        {
            this.transform.Translate(Vector3.left * shakespeed * Time.deltaTime);
        }
        else
        {
            timer = 0;
        }
    }*/
}
