using UnityEngine;

public class Harvest : Demolishable
{
    public int benefinAmount = 1;

    protected override void Awake()
    {
        base.Awake();
        strength = 0;
    }

    protected override void Update()
    {
        if (needToDestroy)
        {
            if (Vector3.Distance(modelTransform.localScale, Vector3.zero) < scaleToDestroy)
            {
                Destroy(model);
                needToDestroy = false;
            }
            modelTransform.localScale = Vector3.Lerp(modelTransform.localScale, Vector3.zero, scaleFactor * Time.deltaTime);
        }
    }

    public override void Demolish(Player player)
    {
        base.Demolish(player);
        player.AddHarvestScore(benefinAmount);
    }
}
