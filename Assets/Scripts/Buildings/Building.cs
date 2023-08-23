using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    //ob Gebäude schon gesetzt
    private bool gesetzt = false;
    
    //Mouseposition
    TilemapHover position;

    // Start is called before the first frame update
    void Start()
    {
        //position = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
    }

    // Update is called once per frame
    private void Update()
    {
        /*if(!gesetzt){
            transform.position = position.getVec3Mouse();
        }

        if(Input.GetMouseButtonDown(0) && !gesetzt){
            transform.position = position.getVec3Mouse();
            gesetzt = true;
        }*/
    }

    
    private void transparencyChange(){

    }
}
