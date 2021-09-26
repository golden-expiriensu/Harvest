
using UnityEngine;

public class FlyingNumbersManager : MonoBehaviour
{
    [SerializeField] RectTransform numbersStartPosition;
    public RectTransform numbersCanvasRectTr;
    public float xResolutionMoveFactor = 80f;
    public float yResolutionMoveFactor = 50f;
    ObjectPooler objectPooler;

    public static FlyingNumbersManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    public void SpawnNumber(ObjectPooler.PoolObjectId id, string flyingNumberText, int flyingTextOriginalSize, float flyingTextScaleFactor)
    {
        GameObject obj = objectPooler.GetObjectFromPool(id);
        PooledFlyingNumber number = obj.GetComponent<PooledFlyingNumber>();
        number.rectTransform.position = numbersStartPosition.position;
        number.textTMPro.text = flyingNumberText;
        number.textTMPro.fontSize = flyingTextOriginalSize * flyingTextScaleFactor;
        number.OnPooled();
    }
}
