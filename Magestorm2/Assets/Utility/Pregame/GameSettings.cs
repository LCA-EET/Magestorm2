using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public static class GameSettings
{
    public const string Language = "language";
    public const string Shadows = "shadows";
    public const string ShadowType = "shadowtype";
    public const string MinimapLighting = "minimaplighting";
    public const string LitProjectiles = "litprojectiles";

    public static Dictionary<SettingKey, GameSetting> SettingsTable;

    public static LightShadowResolution LSR;
    public static void InitializeGraphicsSettings()
    {
        SettingsTable = new Dictionary<SettingKey, GameSetting>();
        SettingsTable.Add(SettingKey.Shadows, new GameSetting(SettingKey.Shadows, ControlCodes.Shadows_Medium, ApplyLightShadowSetting));
        SettingsTable.Add(SettingKey.MinimapLighting, new GameSetting(SettingKey.MinimapLighting, ControlCodes.MinimapLighting_Off,UpdateMinimapLightCulling));
        SettingsTable.Add(SettingKey.LitProjectiles, new GameSetting(SettingKey.LitProjectiles, ControlCodes.LitProjectiles_On, null));
        SettingsTable.Add(SettingKey.ShadowType, new GameSetting(SettingKey.ShadowType, ControlCodes.ShadowType_Soft, ApplyLightShadowSetting));
        ResetLSR();
        PlayerPrefs.Save();
    }
    public static byte GetSettingValue(SettingKey key)
    {
        return SettingsTable[key].Value;
    }
    public static void SetSettingValue(SettingKey key, byte newValue)
    {
        SettingsTable[key].SetValue(newValue);
    }
    private static Light[] GetLightsInScene()
    {
        return ComponentRegister.Scene.GetComponentsInChildren<Light>();
    }
    private static void UpdateMinimapLightCulling()
    {
        if (SettingsTable[SettingKey.MinimapLighting].Value == ControlCodes.MinimapLighting_Off)
        {
            ComponentRegister.Minimap.RemoveFromCullingMask(LayerManager.LightSourceMask);
        }
        else
        {
            ComponentRegister.Minimap.AddToCullingMask(LayerManager.LightSourceMask);
        }
    }
    private static void DisableShadows(Light light)
    {
        light.shadows = LightShadows.None;
    }

    public static void ApplyLightShadowSetting()
    {
        Debug.Log("ASLR");
        if(ComponentRegister.Scene != null)
        {
            Light[] inScene = GetLightsInScene();
            foreach (Light light in inScene)
            {
                ApplyLightShadowSetting(light);
            }
        }
    }
    public static void ApplyLightShadowSetting(Light light)
    {
        if(SettingsTable[SettingKey.ShadowType].Value == ControlCodes.ShadowType_Off)
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
        light.shadows = (SettingsTable[SettingKey.ShadowType].Value == ControlCodes.ShadowType_Soft) ? LightShadows.Soft : LightShadows.Hard;
        light.shadowResolution = LSR;
    }
    public static void ResetLSR()
    {
        LightShadowResolution resolution = LightShadowResolution.Low;
        switch (SettingsTable[SettingKey.Shadows].Value)
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

