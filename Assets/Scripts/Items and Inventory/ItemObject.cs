using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] private Rigidbody2D rb;//�����ٶ�
    [SerializeField] private ItemData ItemData;
    [SerializeField] private Vector2 velocity;//�����ٶ�


    private void SetupVisuals()
    {
        if (ItemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = ItemData.icon;
        gameObject.name = ItemData.name;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)// ����ʵ������
    {
        ItemData = _itemData;
        rb.velocity = _velocity;//�����ٶ�
 
        SetupVisuals();
    }

    public void PickupItem()//ʰȡ�������
    {
        Inventory.instance.AddItem(ItemData);
        Destroy(gameObject);
    }
}