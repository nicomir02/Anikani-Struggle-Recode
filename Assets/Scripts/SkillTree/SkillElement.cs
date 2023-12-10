using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillElement : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] public Transform endPos;

    [SerializeField] private List<SkillElement> nextSkills = new List<SkillElement>();

    public void createSkill(Material mat, Vector3 position, Transform ueberObject) {
        GameObject o = Instantiate(gameObject);
        o.transform.SetParent(ueberObject);

        gameObject.transform.position = position;

        if(nextSkills.Count > 0) {
            float f = (nextSkills.Count-1)*100+endPos.position.y;

            foreach(SkillElement skills in nextSkills) {
                skills.createSkill(mat, new Vector3(startPos.position.x+100, f, startPos.position.z), o.transform);
                f-=100;
                LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
                lineRenderer.startColor = Color.black;
                lineRenderer.endColor = Color.black;
                lineRenderer.startWidth = 0.05f;
                lineRenderer.endWidth = 0.05f;
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = true;    
                                
                //For drawing line in the world space, provide the x,y,z values
                lineRenderer.SetPosition(0, startPos.position); //x,y and z position of the starting point of the line
                lineRenderer.SetPosition(1, skills.endPos.position); //x,y and z position of the end point of the line
            }

            
        }
    }
}

