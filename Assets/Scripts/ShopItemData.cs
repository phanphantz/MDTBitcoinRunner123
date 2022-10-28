using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ShopItemData
{
    public string itemName;
    public string title;
    public string description;
    public Sprite iconSprite;
    public int price;
    public int count;
    public string type;
    public TimeSpan delayTimeSpan;

    public void AddDelay(TimeSpan delay)
    {
        delayTimeSpan = delay;
    }

}
