using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemGroup : MonoBehaviour    
{
    [SerializeField] Text shopItemCountText;
    [SerializeField] Text playerItemCountText;
    [SerializeField] Transform shopItemParent;
    [SerializeField] ShopItem shopItemPrefab;
    [SerializeField] List<ShopItem> shopItemList = new List<ShopItem>();
    [SerializeField] ShopItemData[] shopItemDatas;

    void Start()
    {
        shopItemPrefab.gameObject.SetActive(false);
        Refresh();
    }

    private void Refresh()
    {
        PrepareShopItemDatas();
        SetupUIs(shopItemDatas);
    }

    void PrepareShopItemDatas()
    {
        foreach(var data in shopItemDatas)
            data.count = PreferencesManager.Instance.GetPowerup(data.itemName);
    }

    void SetupUIs(ShopItemData[] datas)
    {
        DestroyAndClearAllUIs();
        CreateUIs(datas);

        shopItemCountText.text = "Shop Item Count : " + shopItemDatas.Length + " Highest Price : " + shopItemDatas.Max(data => data.price);

        var playerItemCount = shopItemDatas.Sum(data => PreferencesManager.Instance.GetPowerup(data.itemName));
        playerItemCountText.text = "Player Item Count : " + playerItemCount;
    }

    void DestroyAndClearAllUIs()
    {
        foreach (var ui in shopItemList)
            Destroy(ui.gameObject);

        shopItemList.Clear();
    }

    void CreateUIs(ShopItemData[] datas)
    {
        foreach (var data in datas)
        {
            var shopUI = Instantiate(shopItemPrefab, shopItemParent, false);
            shopUI.gameObject.SetActive(true);
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
        Refresh();
    }

    public void SortByLowestPrice()
    {
        ShopItemData[] sortedShopItemDatas = shopItemDatas.OrderBy(itemData => itemData.price).ToArray();
        SetupUIs(sortedShopItemDatas);
    }

    public void SortByHighestPrice()
    {
        ShopItemData[] sortedShopItemDatas = shopItemDatas.OrderByDescending(itemData => itemData.price).ToArray();
        SetupUIs(sortedShopItemDatas);
    }

    public void SortByAToZ()
    {
        ShopItemData[] sortedShopItemDatas = shopItemDatas
        .OrderBy(itemData => itemData.title)
        .ThenBy(itemData => itemData.type)
        .ToArray();

        SetupUIs(sortedShopItemDatas);
    }

    public void FilterBuffOnly()
    {
        ShopItemData[] sortedShopItemDatas = shopItemDatas.Where(itemData => itemData.type == "Buff").ToArray();
        SetupUIs(sortedShopItemDatas);
    }

    public void FilterAbilityOnly()
    {
        ShopItemData[] sortedShopItemDatas = shopItemDatas.Where(itemData => itemData.type == "Ability").ToArray();
        SetupUIs(sortedShopItemDatas);
    }

}
