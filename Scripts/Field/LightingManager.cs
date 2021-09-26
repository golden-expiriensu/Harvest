using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public Transform lightTransform;

    private Light mainLight;
    private Light additionalLight;

    readonly Vector3[] lightDirections = new Vector3[] 
    { 
        new Vector3(60, 240, -60),
        new Vector3(130, 310, 90),
        new Vector3(30, 240, -60),
        new Vector3(30, -60, -90),
        new Vector3(60, 180, -90),
        new Vector3(60, 160, -90),
        new Vector3(60, 160, -90)
    };
    readonly string[] lightColors = new string[]
    {
        "#FFF0B9",
        "#FFC800",
        "#FFFF6F",
        "#73C8FF",
        "#FF5AFF",
        "#3AA2FF",
        "#A5BC96"
    };

    public int currentLighting;

    private void Awake()
    {
        Light[] lights = lightTransform.GetComponentsInChildren<Light>();
        mainLight = lights[0];
        additionalLight = lights[1];
    }

    public int SetRandomLighting()
    {
        int lightSeed = Random.Range(0, lightColors.Length);

        lightTransform.eulerAngles = lightDirections[lightSeed];

        if (ColorUtility.TryParseHtmlString(lightColors[lightSeed], out Color color))
        {
            mainLight.color = color;
            additionalLight.color = color;
        }
        currentLighting = lightSeed;
        return currentLighting;
    }

    public void SetLighting(int seed)
    {
        seed = 0;
        lightTransform.eulerAngles = lightDirections[seed];

        if (ColorUtility.TryParseHtmlString(lightColors[seed], out Color color))
        {
            mainLight.color = color;
            additionalLight.color = color;
            currentLighting = seed;
        }
    }
}
