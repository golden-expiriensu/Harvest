using UnityEngine;

public class ObjectOnField : MonoBehaviour
{
    [HideInInspector] public Transform thisTransform;
    [HideInInspector] public Transform modelTransform;

    public bool needToRotateThenInstantinated = false;
    public bool destroyable;
    protected virtual void Awake()
    {
        thisTransform = transform;
        modelTransform = GetComponentsInChildren<Transform>()[1];
        if(needToRotateThenInstantinated)
            modelTransform.eulerAngles = new Vector3(0, 0 + 90 * Random.Range(1, 4), 0);
    }
}
