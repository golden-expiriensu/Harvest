using UnityEngine;

public class Demolishable : ObjectOnField
{
    public GameObject model;

    protected bool needToDestroy = false;
    protected float scaleFactor = 3.5f;
    protected float scaleToDestroy = 0.045f;

    public int strength = 1;
    public bool isBrokenModelJustCompressedModel = false;

    public GameObject ostovPrefab;

    public ObjectPooler.PoolObjectId id;
    public string flyingNumberText;
    private FlyingNumbersManager flyingNumbersManager;
    public float flyingTextScaleFactor = 1f;
    public int flyingTextOriginal = 40;

    protected override void Awake()
    {
        base.Awake();
        flyingNumbersManager = FlyingNumbersManager.Instance;
    }

    protected virtual void Update()
    {
        if (needToDestroy)
        {
            if (modelTransform.localScale.y < scaleToDestroy) DeleteObject();

            modelTransform.localScale = new Vector3(
                modelTransform.localScale.x,
                Mathf.Lerp(modelTransform.localScale.y, 0, scaleFactor * Time.deltaTime),
                modelTransform.localScale.z);
        }
    }

    protected virtual void DeleteObject()
    {
        if(!isBrokenModelJustCompressedModel) Destroy(model);
        needToDestroy = false;
    }

    public virtual void Demolish(Player player)
    {
        player.Tractor.ReduceHealth(strength);
        flyingNumbersManager.SpawnNumber(id, flyingNumberText, flyingTextOriginal, flyingTextScaleFactor);

        player.Field.RemoveObjectFromFieldMemory(new Vector2Int((int)thisTransform.position.x, (int)thisTransform.position.z));
        if(!isBrokenModelJustCompressedModel) 
            player.Field.AddObjectInCell(ostovPrefab, new Vector2Int((int)thisTransform.position.x, (int)thisTransform.position.z));
        needToDestroy = true;
    }
}
