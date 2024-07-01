using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar_UI : MonoBehaviour
{
    private Entity entity;

    private void Start()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void FlipUI()
    {
        Debug.Log("Entity is flipped");
    }
}
