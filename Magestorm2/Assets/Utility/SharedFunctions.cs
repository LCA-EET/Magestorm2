using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public static class SharedFunctions
{
    private static object[] _params;
    public static object[] Params {  
        get { return _params; } 
        set { _params = value; }
    }

    public static int GameServerPort
    {
        get; set;
    }
    public static bool DirectionalCast(Transform origin, int layerMask, float distance, Vector3 direction, out RaycastHit hitInfo)
    {
        return Physics.Raycast(origin.position, origin.TransformDirection(direction), out hitInfo, distance, layerMask);
    }
    public static bool CastDown(Transform origin, int layerMask, float distance, out RaycastHit hitInfo)
    {
        return DirectionalCast(origin, layerMask, distance, Vector3.down, out hitInfo);
    }
    public static bool CastDown(Transform origin, int layerMask, float distance)
    {
        RaycastHit hitInfo;
        return CastDown(origin, layerMask, distance, out hitInfo);
    }
    public static bool CastForward(Transform origin, int layerMask, float distance, out RaycastHit hitInfo)
    {
        return DirectionalCast(origin, layerMask, distance, Vector3.forward, out hitInfo);
    }
    public static string PlayerClassToString(PlayerClass playerClass)
    {
        switch (playerClass)
        {
            case PlayerClass.Arcanist:
                return Language.GetBaseString(7); //
            case PlayerClass.Cleric:
                return Language.GetBaseString(6); //
            case PlayerClass.Magician:
                return Language.GetBaseString(8); // 
            case PlayerClass.Mentalist:
                return Language.GetBaseString(9); //
        }
        return "Undefined";
    }

    public static string ClassAbbreviation(PlayerClass playerClass)
    {
        switch (playerClass)
        {
            case PlayerClass.Arcanist:
                return "Ar";
            case PlayerClass.Cleric:
                return "Cl";
            case PlayerClass.Magician:
                return "Ma";
            case PlayerClass.Mentalist:
                return "Me";
        }
        return "";
    }
    public static string MatchTypeString(MatchTypes matchType)
    {
        
        switch (matchType)
        {
            case MatchTypes.Deathmatch:
                return Language.GetBaseString(104); //
            case MatchTypes.CaptureTheFlag:
                return Language.GetBaseString(106); //
            case MatchTypes.FreeForAll:
                return Language.GetBaseString(105); //
        }
        return "";
    }
    public static void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer; // Set the layer for the current object
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer); // Recursively set the layer for children
        }
    }

    public static bool ProcessFloatLerp(ref float elapsed, float lerpPeriod, float startValue, float endValue, ref float value)
    {
        elapsed += Time.deltaTime;
        float percentComplete = elapsed / lerpPeriod;
        if(percentComplete >= 1.0f)
        {
            percentComplete = 1.0f;
            elapsed -= lerpPeriod;
        }
        value = Mathf.Lerp(startValue, endValue, percentComplete);
        return percentComplete == 1.0f;
    }
    public static bool ProcessVector3Lerp(ref float elapsed, float lerpPeriod, Vector3 startingPosition, Vector3 endingPosition, Transform mover)
    {
        elapsed += Time.deltaTime;
        float percentComplete = elapsed / lerpPeriod;
        if (percentComplete >= 1.0f)
        {
            percentComplete = 1.0f;
            elapsed = 0.0f;
        }
        mover.position = Vector3.Lerp(startingPosition, endingPosition, percentComplete);
        return percentComplete == 1.0f;
    }
}
