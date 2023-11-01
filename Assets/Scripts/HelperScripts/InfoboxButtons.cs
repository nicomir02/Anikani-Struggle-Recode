using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoboxButtons : MonoBehaviour
{
    private InfoboxManager im;
    // Start is called before the first frame update
    void Start()
    {
        im = GameObject.Find("___InGame___/----Manager----/GameManager").GetComponent<InfoboxManager>();
    }

    public void openAllgemeineInfos(){
        im.openAllgemeineInfos();
    }

    public void destroyObject(){
        im.destroyObject();
    }

    public void einheitenAusbildung(){
        im.einheitenAusbildung();
    }
    
}
