using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ShopItemGroup : MonoBehaviour    
{
    [SerializeField] ShopItem[] shopItems;
    [SerializeField] ShopItemData[] shopItemDatas;

    void Start() 
    {
        PrepareShopItemDatas();
        SetupUIs();
    }

    void PrepareShopItemDatas()
    {
        foreach(var data in shopItemDatas)
            data.count = PreferencesManager.Instance.GetPowerup(data.itemName);
    }

    void SetupUIs()
    {
        var index = 0;
        foreach(var item in shopItems)
        {
            item.SetShopItemData(shopItemDatas[index]);
            item.onBuy += HandleBuy;
            index++;
        }
    }

    void HandleBuy(ShopItemData itemData)
    {
        if (PreferencesManager.Instance.GetCoins() < itemData.price)
            return;

		PreferencesManager.Instance.AddCoins(-itemData.price);
		PreferencesManager.Instance.ModifyPowerup(itemData.itemName, 1);
		UIManager.Instance.UpdateMenuCoinCounts();
		FirebaseEventManager.Instance.SendSpendVirtualCurrency();
        PrepareShopItemDatas();
        SetupUIs();
    }

}
