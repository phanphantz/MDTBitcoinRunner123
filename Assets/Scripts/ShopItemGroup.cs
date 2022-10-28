using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ShopItemGroup : MonoBehaviour    
{
    [SerializeField] Transform shopItemParent;
    [SerializeField] ShopItem shopItemPrefab;
    [SerializeField] List<ShopItem> shopItemList = new List<ShopItem>();
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
        foreach(var data in shopItemDatas)
        {
            var shopUI = Instantiate(shopItemPrefab , shopItemParent, false);
            shopUI.SetShopItemData(data);
            shopUI.onBuy += HandleBuy;
            shopItemList.Add(shopUI);
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
