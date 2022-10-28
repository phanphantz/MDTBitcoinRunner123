using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class SkillItem : ItemUIGen<SkillItemData>
{
    protected override void UpdateDisplay()
    {
        
    }
}

public class SkillItemData
{
    public string skillName;
    public string description;
}
