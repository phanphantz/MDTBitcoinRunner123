using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShopGroupGen<T> : MonoBehaviour 
{
    [SerializeField] protected Text shopItemCountText;
    [SerializeField] protected Text playerItemCountText;
    [SerializeField] protected Transform uiItemParent;
    [SerializeField] protected ItemUIGen<T> uiPrefab;
    [SerializeField] protected List<ItemUIGen<T>> uiItemList = new List<ItemUIGen<T>>();
    [SerializeField] protected T[] datas;

    void Start()
    {
        uiPrefab.gameObject.SetActive(false);
        Refresh();
    }

    protected virtual void Refresh()
    {
        SetupUIs(datas);
    }

    protected virtual void SetupUIs(T[] datas)
    {
        DestroyAndClearAllUIs();
        CreateUIs(datas);
    }

    void DestroyAndClearAllUIs()
    {
        foreach (var ui in uiItemList)
            Destroy(ui.gameObject);

        uiItemList.Clear();
    }

    void CreateUIs(T[] datas)
    {
        foreach (var data in datas)
        {
            var ui = Instantiate(uiPrefab, uiItemParent, false);
            ui.gameObject.SetActive(true);
            ui.SetItemData(data);
            ui.onBuy += HandleBuy;
            uiItemList.Add(ui);
        }
    }

    protected virtual void HandleBuy(T itemData)
    {
        // Buy Character Logic
        Refresh();
    }

}
