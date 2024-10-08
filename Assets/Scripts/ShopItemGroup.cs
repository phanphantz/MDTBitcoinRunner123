using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemGroup : ShopGroupGen<ShopItemData>  
{
    private void Update() 
    {
        var index = 0;
        foreach(var data in datas)
        {
            if (data.delayTimeSpan.TotalSeconds > 0)
                data.delayTimeSpan -= TimeSpan.FromSeconds(Time.deltaTime);

            uiItemList[index].SetItemData(data);
            index++;
        }
         
    }

    protected override void Refresh()
    {
        PrepareShopItemDatas();
        base.Refresh();
    }

    void PrepareShopItemDatas()
    {
        foreach(var data in datas)
            data.count = PreferencesManager.Instance.GetPowerup(data.itemName);
    }

    protected override void SetupUIs(ShopItemData[] datas)
    {
        base.SetupUIs(datas);

        shopItemCountText.text = "Shop Item Count : " + datas.Length + " Highest Price : " + datas.Max(data => data.price);

        var playerItemCount = datas.Sum(data => PreferencesManager.Instance.GetPowerup(data.itemName));
        playerItemCountText.text = "Player Item Count : " + playerItemCount;
    }
 
    protected override void HandleBuy(ShopItemData itemData)
    {
        if (itemData.delayTimeSpan.TotalSeconds > 0)
            return;

        if (PreferencesManager.Instance.GetCoins() < itemData.price)
            return;

        itemData.AddDelay(new TimeSpan( 1, 0 , 0 ,0));
		PreferencesManager.Instance.AddCoins(-itemData.price);
		PreferencesManager.Instance.ModifyPowerup(itemData.itemName, 1);
		UIManager.Instance.UpdateMenuCoinCounts();
		FirebaseEventManager.Instance.SendSpendVirtualCurrency();
        Refresh();
    }

    public void SortByLowestPrice()
    {
        ShopItemData[] sortedShopItemDatas = datas.OrderBy(itemData => itemData.price).ToArray();
        SetupUIs(sortedShopItemDatas);
    }

    public void SortByHighestPrice()
    {
        ShopItemData[] sortedShopItemDatas = datas.OrderByDescending(itemData => itemData.price).ToArray();
        SetupUIs(sortedShopItemDatas);
    }

    public void SortByAToZ()
    {
        ShopItemData[] sortedShopItemDatas = datas
        .OrderBy(itemData => itemData.title)
        .ThenBy(itemData => itemData.type)
        .ToArray();

        SetupUIs(sortedShopItemDatas);
    }

    public void FilterBuffOnly()
    {
        ShopItemData[] sortedShopItemDatas = datas.Where(itemData => itemData.type == "Buff").ToArray();
        SetupUIs(sortedShopItemDatas);
    }

    public void FilterAbilityOnly()
    {
        ShopItemData[] sortedShopItemDatas = datas.Where(itemData => itemData.type == "Ability").ToArray();
        SetupUIs(sortedShopItemDatas);
    }

}
