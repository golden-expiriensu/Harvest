using UnityEngine;

public class Tractor : ObjectOnField
{
    private int maxFuel = 50;
    public float СurrentFuel { get; private set; }

    private int maxHealth = 5;
    public int СurrentHealth { get; private set; }



    public Player player;
    public Transform CurrentModelTransform;
    public GameObject TractorModel;
    public GameObject BrokenTractorModel;
    private GameObject brokenModel;


    private float dotFactor = 0.96f;
    private float rotateSpeed = 10f;
    private bool needToRotate = false;
    private Vector3 currentDirection;

    public float distanceFactor = 0.01f;
    public float moveSpeed = 4.47f;

    public Vector3 currentMove;
    public Vector3 startPosition;


    private bool increase = false;
    private Vector3 originalScale;
    public float chyhChyhScale = 0.83f;
    public float chyhChyhIDLEScale = 0.92f;
    public float chyhChyhSpeed = 7.2f;
    public float chyhChyhIDLESpeed = 4f;

    private FlyingNumbersManager flyingNumbersManager;


    public bool isMoving = false;

    protected override void Awake()
    {
        base.Awake();
        СurrentFuel = maxFuel;
        СurrentHealth = maxHealth;

        originalScale = CurrentModelTransform.localScale;

        flyingNumbersManager = FlyingNumbersManager.Instance;
    }

    private void Update()
    {
        if (!player.IsGameOver)
        {
            if (CanMove()) TractorGoChyhChyh();
            else
            {
                player.SetGameOver();
                player.IsGameOver = true;
            }
        }
        if (isMoving) DoCurrentMove();
        if (needToRotate) RotateToCurrentRotation();
    }

    public void SetMaxFuel(float value)
    {
        maxFuel = Mathf.RoundToInt(value);
        СurrentFuel = maxFuel;
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = Mathf.RoundToInt(value);
        СurrentHealth = maxHealth;
    }

    public void SetСurrentFuel(float value)
    {
        СurrentFuel = value;
        player.UIManager.SetTextFuelSlider((float)СurrentFuel / maxFuel);
    }

    public void SetСurrentHealth(float value)
    {
        СurrentHealth = (int)value;
        player.UIManager.SetTextHealthSlider((float)СurrentHealth / maxHealth);
    }

    public void ReduceFuel(float value)
    {
        СurrentFuel -= value;
        if (СurrentFuel < 0) СurrentFuel = 0;
        player.UIManager.SetTextFuelSlider((float)СurrentFuel / maxFuel);
    }

    public void AddFuel(float value)
    {
        СurrentFuel += value;
        if (СurrentFuel > maxFuel) СurrentFuel = maxFuel;
        player.UIManager.SetTextFuelSlider((float)СurrentFuel / maxFuel);
    }

    public void ReduceHealth(int strength)
    {
        СurrentHealth -= strength;
        if (СurrentHealth < 0) СurrentHealth = 0;
        player.UIManager.SetTextHealthSlider((float)СurrentHealth / maxHealth);
    }

    public void AddHealth(int value)
    {
        СurrentHealth += value;
        if (СurrentHealth > maxHealth) СurrentHealth = maxHealth;
        player.UIManager.SetTextHealthSlider((float)СurrentHealth / maxHealth);
    }

    public bool CanMove()
    {
        return (СurrentFuel > 0 && СurrentHealth > 0);
    }

    public void Broke()
    {
        brokenModel = Instantiate(BrokenTractorModel, CurrentModelTransform);
        player.Tractor.CurrentModelTransform.localScale = originalScale;
        TractorModel.SetActive(false);
    }

    public void Recover()
    {
        TractorModel.SetActive(true);
        Destroy(brokenModel);
        player.IsGameOver = false;
        СurrentFuel = maxFuel;
        СurrentHealth = maxHealth;
    }

    private void DoCurrentMove()
    {
        if(player.Tractor.thisTransform.position == currentMove)
        {
            player.Tractor.isMoving = false;
            return;
        }

        Vector3 position = Vector3.Lerp(startPosition, currentMove, moveSpeed * Time.deltaTime);

        if (startPosition.x == currentMove.x)
        {
            float min = currentMove.z < startPosition.z ? currentMove.z : startPosition.z;
            float max = currentMove.z > startPosition.z ? currentMove.z : startPosition.z;
            float incrementZ = position.z - startPosition.z;
            
            player.Tractor.thisTransform.position = new Vector3(
                player.Tractor.thisTransform.position.x,
                player.Tractor.thisTransform.position.y,
                Mathf.Clamp(player.Tractor.thisTransform.position.z + incrementZ, min, max));

            //move flying numbers canvas
            Vector3 numbPos = flyingNumbersManager.numbersCanvasRectTr.position;
            flyingNumbersManager.numbersCanvasRectTr.position = 
                new Vector3(numbPos.x, numbPos.y + (-incrementZ * flyingNumbersManager.yResolutionMoveFactor), 0);
            //
        }
        else
        {
            float min = currentMove.x < startPosition.x ? currentMove.x : startPosition.x;
            float max = currentMove.x > startPosition.x ? currentMove.x : startPosition.x;
            float incrementX = position.x - startPosition.x;

            player.Tractor.thisTransform.position = new Vector3(
                Mathf.Clamp(player.Tractor.thisTransform.position.x + incrementX, min, max),
                player.Tractor.thisTransform.position.y,
                player.Tractor.thisTransform.position.z);

            //move flying numbers canvas
            Vector3 numbPos = flyingNumbersManager.numbersCanvasRectTr.position;
            flyingNumbersManager.numbersCanvasRectTr.position = 
                new Vector3(numbPos.x + (-incrementX * flyingNumbersManager.xResolutionMoveFactor), numbPos.y, 0);
            //
        }
    }

    public void SetRotation(Vector3 rotate)
    {
        currentDirection = -rotate;
        needToRotate = true;
    }

    private void RotateToCurrentRotation()
    {
        if (!player.IsGameOver)
        {
            Quaternion rotation = Quaternion.LookRotation(currentDirection);

            if (Quaternion.Dot(rotation, player.Tractor.CurrentModelTransform.rotation) > dotFactor)
            {
                player.Tractor.CurrentModelTransform.rotation = rotation;
                needToRotate = false;
                return;
            }

            player.Tractor.CurrentModelTransform.rotation = Quaternion.Lerp(player.Tractor.CurrentModelTransform.rotation, rotation, rotateSpeed * Time.deltaTime); 
        }
    }

    public void TractorGoChyhChyh()
    {
        float currentChyhChyhFactor = isMoving ? chyhChyhSpeed : chyhChyhIDLESpeed;
        float currentChyhChyhScale = isMoving ? chyhChyhScale : chyhChyhIDLEScale;
        if (increase)
        {
            if (player.Tractor.CurrentModelTransform.localScale.y > originalScale.y)
            {
                player.Tractor.CurrentModelTransform.localScale = originalScale;
                increase = false;
            }
            Vector3 scale = Vector3.Lerp(
                new Vector3(originalScale.x, originalScale.y * currentChyhChyhScale, originalScale.z),
                originalScale,
                currentChyhChyhFactor * Time.deltaTime);

            player.Tractor.CurrentModelTransform.localScale += new Vector3(0, scale.y - originalScale.y * currentChyhChyhScale, 0);
        }
        else
        {
            if (player.Tractor.CurrentModelTransform.localScale.y < originalScale.y * currentChyhChyhScale)
            {
                player.Tractor.CurrentModelTransform.localScale = new Vector3(originalScale.x, originalScale.y * currentChyhChyhScale, originalScale.z);
                increase = true;
            }
            Vector3 scale = Vector3.Lerp(
                originalScale,
                new Vector3(originalScale.x, originalScale.y * currentChyhChyhScale, originalScale.z),
                currentChyhChyhFactor * Time.deltaTime);

            player.Tractor.CurrentModelTransform.localScale -= new Vector3(0, originalScale.y - scale.y, 0);
        }
    }
}
