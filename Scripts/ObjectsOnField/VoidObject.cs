using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidObject : Demolishable
{
    

    protected override void Awake()
    { 
        destroyable = true;
    }
    protected override void Update()
    { }

    public override void Demolish(Player player)
    {
        DeleteObject();
    }
}
