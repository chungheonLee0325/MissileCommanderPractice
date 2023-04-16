﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleObject : MonoBehaviour
{
    protected bool isActived = false;
    protected Vector3 targetPosition;

    public Action<RecycleObject> Destroyed;
    public Action<RecycleObject> OutOfScreen;

    public virtual void Activate(Vector3 position)
    {
        isActived = true;
        transform.position = position;
    }
    public virtual void Activate(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;
        this.targetPosition = targetPosition;
        Vector3 dir = (targetPosition - startPosition).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, dir);
        isActived = true;
    }
}
