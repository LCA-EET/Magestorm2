using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class SharedFunctions
{
    public static bool CastDown(Transform origin, int layerMask, float distance, out RaycastHit hitInfo)
    {
        return Physics.Raycast(origin.position, origin.TransformDirection(Vector3.down), out hitInfo, distance);
    }
    public static bool CastDown(Transform origin, int layerMask, float distance)
    {
        RaycastHit hitInfo;
        return CastDown(origin, layerMask, distance, out hitInfo);
    }

    public static string PlayerClassToString(PlayerClass playerClass)
    {
        switch (playerClass)
        {
            case PlayerClass.Arcanist:
                return Language.GetBaseString(6);
            case PlayerClass.Cleric:
                return Language.GetBaseString(5);
            case PlayerClass.Magician:
                return Language.GetBaseString(7);
            case PlayerClass.Mentalist:
                return Language.GetBaseString(8);
        }
        return "Undefined";
    }
}
