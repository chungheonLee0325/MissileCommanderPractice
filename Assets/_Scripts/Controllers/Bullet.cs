using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : RecycleObject
{
    [SerializeField]
    float moveSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActived)
            return;
        transform.position += transform.up * moveSpeed * Time.deltaTime;
        if(IsArrivedToTarget())
        {
            isActived = false;
            Destroyed?.Invoke(this);
        }
    }

    bool IsArrivedToTarget()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        return distance < 0.1f;
    }
}
