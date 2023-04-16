using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Building : MonoBehaviour
{
    BoxCollider2D box;

    public Action<Building> Destroyed;
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Missile>()!= null)
        {
            Destroyed?.Invoke(this);
        }
    }
}
