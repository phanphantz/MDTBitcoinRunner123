using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
	public Text itemCountText;
	public Text itemPriceText;
	ShopItemData itemData;

	public Action<ShopItemData> onBuy;

	public void SetShopItemData(ShopItemData data)
	{
		itemData = data;
		UpdateDisplay();
	}

	void UpdateDisplay()
	{
		itemPriceText.text = (((float)itemData.price) / 100.0f).ToString("0.00");
		itemCountText.text = "x " + itemData.count; 
	}

	public void BuyPowerup()
	{
		onBuy?.Invoke(itemData);
	}

}
