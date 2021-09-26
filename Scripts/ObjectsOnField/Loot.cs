using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : Demolishable
{
    public Types type;

    private bool needToWhirl = false;
    private bool increase;
    private Vector3 originalPosition;
    public float whirlSpeed = 107.61f;
    public float whirlThenCollectedSpeedFactor = 1.6f;
    public float whirlYSpeed = 0.39f;
    public float whirlHeight = 0.2f;
    public float whirlDeltaHeight = 0.15f;

    public enum Types
    {
        Petrol,
        RepairBox
    }

    protected override void Awake()
    {
        base.Awake();
        strength = 0;
        destroyable = true;
        needToWhirl = true;
        originalPosition = modelTransform.position;
        originalPosition.y += whirlHeight;
    }

    protected override void Update()
    {
        if (needToDestroy)
        {
            if (Vector3.Distance(modelTransform.localScale, Vector3.zero) < scaleToDestroy)
            {
                DeleteObject();
                needToDestroy = false;
            }
            if (whirlSpeed < 99999) whirlSpeed *= whirlThenCollectedSpeedFactor;
            modelTransform.localScale = Vector3.Lerp(modelTransform.localScale, Vector3.zero, scaleFactor * Time.deltaTime);
        }

        if (needToWhirl) DoWhirl();
    }

    public override void Demolish(Player player)
    {
        base.Demolish(player);

        player.GetLoot(type);
    }

    protected override void DeleteObject()
    {
        base.DeleteObject();
        needToWhirl = false;
    }

    private void DoWhirl()
    {
        modelTransform.eulerAngles = new Vector3(
            modelTransform.eulerAngles.x,
            modelTransform.eulerAngles.y + whirlSpeed * Time.deltaTime,
            modelTransform.eulerAngles.z);


        if (increase)
        {
            if (modelTransform.position.y < originalPosition.y)
            {
                modelTransform.position = originalPosition;
                increase = false;
            }
            Vector3 scale = Vector3.Lerp(
                new Vector3(originalPosition.x, originalPosition.y + whirlDeltaHeight, originalPosition.z),
                originalPosition,
                whirlYSpeed * Time.deltaTime);

            modelTransform.position += new Vector3(0, scale.y - (originalPosition.y + whirlDeltaHeight), 0);
        }
        else
        {
            if (modelTransform.position.y > originalPosition.y + whirlDeltaHeight)
            {
                modelTransform.position = new Vector3(originalPosition.x, originalPosition.y + whirlDeltaHeight, originalPosition.z);
                increase = true;
            }
            Vector3 scale = Vector3.Lerp(
                originalPosition,
                new Vector3(originalPosition.x, originalPosition.y + whirlDeltaHeight, originalPosition.z),
                whirlYSpeed * Time.deltaTime);

            modelTransform.position -= new Vector3(0, originalPosition.y - scale.y, 0);
        }
    }
}
