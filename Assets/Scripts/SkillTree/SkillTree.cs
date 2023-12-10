using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree
{
    private SkillElement firstSkill;
    private Material mat;

    public SkillTree(Material m, SkillElement firstElement) {
        firstSkill = firstElement;
        mat = m;
    }

    public void createTree(Transform menu, Transform start) {
        firstSkill.createSkill(mat, start.position, menu);
    }
}
