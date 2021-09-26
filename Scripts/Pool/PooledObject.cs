using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PooledObject : MonoBehaviour
{
    public RectTransform rectTransform;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public abstract void OnPooled();
}
