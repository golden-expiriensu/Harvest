using UnityEngine;

public class Obstacle : ObjectOnField
{
    protected override void Awake()
    {
        base.Awake();
        destroyable = false;
    }
}
