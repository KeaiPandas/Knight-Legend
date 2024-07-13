using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private ItemData itemData;
     [SerializeField] private Vector2 velocity;

    private void OnValidate()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            rb.velocity = velocity;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.itemName;
        itemData = _itemData;
        rb.velocity = _velocity;
    }


    public void PickUpItem()
    {
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
