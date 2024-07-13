using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] private Rigidbody2D rb;//设置速度
    [SerializeField] private ItemData ItemData;
    [SerializeField] private Vector2 velocity;//设置速度


    private void SetupVisuals()
    {
        if (ItemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = ItemData.icon;
        gameObject.name = ItemData.name;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)// 设置实例函数
    {
        ItemData = _itemData;
        rb.velocity = _velocity;//设置速度
 
        SetupVisuals();
    }

    public void PickupItem()//拾取函数打包
    {
        Inventory.instance.AddItem(ItemData);
        Destroy(gameObject);
    }
}