using UnityEngine;

public class SkyManager : MonoBehaviour
{
    public Material daySky;
    public Material nightSky;

    public enum SkyMode
    {
        Day,
        Night
    }

    public SkyMode currentSky;

    void Start()
    {
        ChangeSky();
    }

    public void ChangeSky()
    {
        if (currentSky == SkyMode.Day)
        {
            RenderSettings.skybox = daySky;
        }
        else
        {
            RenderSettings.skybox = nightSky;
        }

        DynamicGI.UpdateEnvironment();
    }
}