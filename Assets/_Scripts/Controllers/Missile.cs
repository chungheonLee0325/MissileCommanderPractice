using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Missile : RecycleObject
{
    BoxCollider2D box;
    Rigidbody2D body;

    [SerializeField]
    float moveSpeed = 3f;

    float bottomY;

    void Start()
    {
        Vector3 bottomPosition = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        bottomY = bottomPosition.y - box.size.y;
    }
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();

        body.bodyType = RigidbodyType2D.Kinematic;
        box.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActived)
            return;

        transform.position += transform.up * moveSpeed * Time.deltaTime;
        CheckOutOfScreen();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Building>() != null)
        {
            OutOfScreen?.Invoke(this);
            //DestroySelf();
            return;
        }
        if(collision.GetComponent<Explosion>()!=null)
        {
            DestroySelf();
            return;
        }
    }
    void DestroySelf()
    {
        isActived = false;
        Destroyed?.Invoke(this);
    }
    void CheckOutOfScreen()
    {
        if(transform.position.y < bottomY)
        {
            isActived = false;
            OutOfScreen?.Invoke(this);
        }
    }
}
