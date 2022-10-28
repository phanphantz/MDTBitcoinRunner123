using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : ItemUIGen<ShopItemData>
{
    protected override void UpdateDisplay()
    {
        itemPriceText.text = (((float)data.price) / 100.0f).ToString("0.00");
		itemCountText.text = "x " + data.count; 
		if (data.delayTimeSpan.TotalSeconds > 0)
		{
			itemTitleText.text = data.title + " (Please Wait for " + data.delayTimeSpan.Hours.ToString() + " Hours)";
		}
		else
		{
			itemTitleText.text = data.title;
		}

		itemDescriptionText.text = data.description;
		itemIconImage.sprite = data.iconSprite;
    }
	
}
