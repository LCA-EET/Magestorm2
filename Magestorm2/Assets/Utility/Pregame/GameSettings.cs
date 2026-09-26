using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
public static class GameSettings
{
    public const string Language = "language";
    public const string Shadows = "shadows";
    public static LightShadowResolution LSR;
    public static void InitializeGraphicsSettings()
    {
        bool changed = false;
        Debug.Log("Graphics");
        int shadows = PlayerPrefs.GetInt(Shadows);
        if(shadows == 0)
        {
            PlayerPrefs.SetInt(Shadows, ControlCodes.Shadows_Medium);
            changed = true;
        }
        ResetLSR();
        if (changed)
        {
            PlayerPrefs.Save();
        }
    }
    private static Light[] GetLightsInScene()
    {
        return ComponentRegister.Scene.GetComponentsInChildren<Light>();
    }
    private static void DisableShadows(Light light)
    {
        light.shadows = LightShadows.None;
    }

    public static void ApplyLightShadowSetting()
    {
        Light[] inScene = GetLightsInScene();
        foreach (Light light in inScene)
        {
            ApplyLightShadowSetting(light);
        }
    }
    public static void ApplyLightShadowSetting(Light light)
    {
        if(PlayerPrefs.GetInt(Shadows) == ControlCodes.Shadows_Off)
        {
            DisableShadows(light);
        }
        else
        {
            UpdateShadowResolution(light);
        }
    }
    private static void UpdateShadowResolution(Light light)
    {
        light.shadows = LightShadows.Soft;
        light.shadowResolution = LSR;
    }
    public static void ResetLSR()
    {
        LightShadowResolution resolution = LightShadowResolution.Low;
        switch (PlayerPrefs.GetInt(Shadows))
        {
            case ControlCodes.Shadows_Medium:
                resolution = LightShadowResolution.Medium;
                Debug.Log("Medium Resolution");
                break;
            case ControlCodes.Shadows_High:
                resolution = LightShadowResolution.High;
                Debug.Log("High Resolution");
                break;
        }
        LSR = resolution;
    }
}

