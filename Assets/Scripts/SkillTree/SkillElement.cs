using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillElement : MonoBehaviour
{
    [SerializeField] public Transform startPos;
    [SerializeField] private Transform endPos;

    [SerializeField] private List<SkillElement> nextSkills = new List<SkillElement>();

    public void createSkill(Material mat, Vector3 position, Transform ueberObject, SkillElement last=null) {
        GameObject o = Instantiate(gameObject);
        o.transform.SetParent(ueberObject, false);

        gameObject.transform.position = position;

        Transform canvasTransform = GameObject.Find("Canvas").GetComponent<Transform>();

        if(nextSkills.Count > 0) {
            float f = (nextSkills.Count-1)*100+endPos.position.y;

            foreach(SkillElement skills in nextSkills) {
                skills.createSkill(mat, new Vector3(startPos.position.x+200, f, startPos.position.z), ueberObject, this);
                f-=100;
            }
        }

        if(last!=null) {
            LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
            lineRenderer.transform.SetParent(ueberObject);
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = false;    
                            
            //For drawing line in the world space, provide the x,y,z values
            lineRenderer.SetPosition(0, last.startPos.position); //x,y and z position of the starting point of the line
            lineRenderer.SetPosition(1, endPos.position); //x,y and z position of the end point of the line
        }
        
    }
}

