using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public static class GameSettings
{

    public static Dictionary<SettingKey, GameSetting> SettingsTable;

    public static LightShadowResolution LSR;
    public static void InitializeSettings()
    {
        SettingsTable = new Dictionary<SettingKey, GameSetting>();
        SettingsTable.Add(SettingKey.Shadows, new GameSetting(SettingKey.Shadows, ControlCodes.Shadows_Medium, ApplyLightShadowSetting));
        SettingsTable.Add(SettingKey.MinimapLighting, new GameSetting(SettingKey.MinimapLighting, ControlCodes.MinimapLighting_Off,UpdateMinimapLightCulling));
        SettingsTable.Add(SettingKey.LitProjectiles, new GameSetting(SettingKey.LitProjectiles, ControlCodes.LitProjectiles_On, null));
        SettingsTable.Add(SettingKey.ShadowType, new GameSetting(SettingKey.ShadowType, ControlCodes.ShadowType_Soft, ApplyLightShadowSetting));
        SettingsTable.Add(SettingKey.AAMode, new GameSetting(SettingKey.AAMode, ControlCodes.AntiAliasing_FXAA, ApplyAntiAliasModeSetting));
        SettingsTable.Add(SettingKey.AAQuality, new GameSetting(SettingKey.AAQuality, ControlCodes.AAQuality_Medium, ApplyAntiAliasQualitySetting));
        SettingsTable.Add(SettingKey.Language, new GameSetting(SettingKey.Language, ControlCodes.Language_English, ApplyLanguageSetting));
        if (!PlayerPrefs.HasKey("server"))
        {
            PlayerPrefs.SetString("server", "www.fosiemods.net:6000");
        }
        ResetLSR();
        PlayerPrefs.Save();
    }
    public static void ApplyLanguageSetting()
    {
        Language.SetLanguage(GetSettingValue(SettingKey.Language));
    }
    public static byte GetSettingValue(SettingKey key)
    {
        byte toReturn = SettingsTable[key].Value;
        Debug.Log("GetSettingValue " + key + ", " + toReturn);
        return toReturn;
    }
    public static void SetSettingValue(SettingKey key, byte newValue)
    {
        SettingsTable[key].SetValue(newValue);
    }
    public static void WriteSettings()
    {
        foreach (GameSetting setting in SettingsTable.Values)
        {
            setting.WriteSetting();
        }
        PlayerPrefs.Save();
    }
    private static Light[] GetLightsInScene()
    {
        return ComponentRegister.Scene.GetComponentsInChildren<Light>();
    }
    private static void UpdateMinimapLightCulling()
    {
        if(ComponentRegister.Minimap != null)
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
    }
    private static void DisableShadows(Light light)
    {
        light.shadows = LightShadows.None;
    }
    public static void ApplyAntiAliasQualitySetting()
    {
        if (Camera.main != null)
        {
            UniversalAdditionalCameraData data = Camera.main.GetComponent<UniversalAdditionalCameraData>();
            byte setting = SettingsTable[SettingKey.AAQuality].Value;
            switch (setting)
            {
                case ControlCodes.AAQuality_Low:
                    data.antialiasingQuality = AntialiasingQuality.Low;
                    break;
                case ControlCodes.AAQuality_Medium:
                    data.antialiasingQuality = AntialiasingQuality.Medium;
                    break;
                case ControlCodes.AAQuality_High:
                    data.antialiasingQuality = AntialiasingQuality.High;
                    break;
            }
        }
    }
    public static void ApplyAntiAliasModeSetting()
    {
        if (Camera.main != null)
        {
            UniversalAdditionalCameraData data = Camera.main.GetComponent<UniversalAdditionalCameraData>();
            byte setting = SettingsTable[SettingKey.AAMode].Value;
            switch (setting)
            {
                case ControlCodes.AntiAliasing_Off:
                    data.antialiasing = AntialiasingMode.None;
                    break;
                case ControlCodes.AntiAliasing_FXAA:
                    data.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                    break;
                case ControlCodes.AntiAliasing_TAA:
                    data.antialiasing = AntialiasingMode.TemporalAntiAliasing;
                    break;
                case ControlCodes.AntiAliasing_SMAA:
                    data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    break;
            }
        }
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
        if (SettingsTable != null)
        {
            if (SettingsTable[SettingKey.ShadowType].Value == ControlCodes.ShadowType_Off)
            {
                DisableShadows(light);
            }
            else
            {
                UpdateShadowResolution(light);
            }
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

