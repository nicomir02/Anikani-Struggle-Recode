using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour {

    // Fill/drag these in from the editor

    // Choose the Unlit/Color shader in the Material Settings
    // You can change that color, to change the color of the connecting lines
    [SerializeField] private Material lineMat;
    [SerializeField] private SkillElement firstAllgemeinSkill;

    [SerializeField] private Transform menu;

    [SerializeField] private Transform start;

    public void Start() {
        openAllgemeinSkill();
    }

    public void openAllgemeinSkill() {
        SkillTree skillTree = new SkillTree(lineMat, firstAllgemeinSkill); 
        skillTree.createTree(menu, start);
    }
}