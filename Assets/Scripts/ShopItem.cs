using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
	public Text itemCountText;
	public Text itemPriceText;
	[SerializeField] ShopItemData itemData;

	void Start()
	{
		UpdateShopItem();
	}

	void UpdateShopItem()
	{
		// Display Data
		itemPriceText.text = (((float)itemData.price) / 100.0f).ToString("0.00");
		itemCountText.text = "x " + itemData.count; //Get Data
		//PreferencesManager.Instance.GetPowerup(itemName)
	}

	public void BuyPowerup()
	{
		if (PreferencesManager.Instance.GetCoins() >= itemData.price)
		{
			PreferencesManager.Instance.AddCoins(-itemData.price);
			PreferencesManager.Instance.ModifyPowerup(itemData.itemName, 1); //Modify Data
			UpdateShopItem();
			UIManager.Instance.UpdateMenuCoinCounts();
			FirebaseEventManager.Instance.SendSpendVirtualCurrency();
		}
	}
}
