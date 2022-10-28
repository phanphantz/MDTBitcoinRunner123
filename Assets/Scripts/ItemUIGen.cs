using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemUIGen<T> : MonoBehaviour 
{
    public Text itemCountText;
    public Text itemPriceText;
    public Text itemTitleText;
    public Text itemDescriptionText;
    public Image itemIconImage;

    protected T data;

    public Action<T> onBuy;

    public void SetItemData(T data)
    {
        this.data = data;
        UpdateDisplay();
    }

    protected abstract void UpdateDisplay();

    public void BuyPowerup()
    {
        onBuy?.Invoke(data);
    }

}
