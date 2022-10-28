using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

public class CharacterItem : ItemUIGen<CharacterItemData>
{
    protected override void UpdateDisplay()
    {
        itemTitleText.text = data.name;
        itemDescriptionText.text = "HP : " + data.hp + " ATK : " + data.attack;
    }
}
