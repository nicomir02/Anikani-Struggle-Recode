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
        position = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<TilemapHover>();
        transparencyChange(0.5f);
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 newPos = position.getVec3Mouse();
        newPos.z += 1;
        if(!gesetzt){
            transform.position = newPos;
        }

        if(Input.GetMouseButtonDown(0) && !gesetzt && setzbar()){
            transform.position = newPos;
            gesetzt = true;
            transparencyChange(1f);
        }
        if(!setzbar()) Debug.Log("nicht setzbar");
    }

    
    private void transparencyChange(float f){
        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = f;
        GetComponent<SpriteRenderer>().color = tmp;
    }

    private bool setzbar(){
        RadiusNeighborCheck radiusNeighborCheck = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<RadiusNeighborCheck>(); 
        int radius = 3; // irgendwie allgemein formulieren für alle? vielleicht aus spezifischen gebäude skript diese methode mit radius 3 als parameter aufrufen?
        Vector3Int neuPos = position.getMousePosition();
        neuPos.z += 1;
        return radiusNeighborCheck.checkRadius(radius, neuPos, position.tilemaps);  
    }
}
