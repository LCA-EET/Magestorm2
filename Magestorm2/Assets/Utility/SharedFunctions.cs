using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Xml.Linq;

public static class SharedFunctions
{
    private static System.Random _random = new System.Random();
    private static object[] _params;

    private static byte[] ArcanistDisciplines = new byte[] { 
        ControlCodes.SpellDiscipline_ManaLaw, 
        ControlCodes.SpellDiscipline_VoidLaw, 
        ControlCodes.SpellDiscipline_Sigils, 
        ControlCodes.SpellDiscipline_Shielding 
    };

    private static byte[] ClericDisciplines = new byte[] { 
        ControlCodes.SpellDiscipline_Barriers, 
        ControlCodes.SpellDiscipline_Healing, 
        ControlCodes.SpellDiscipline_Smiting, 
        ControlCodes.SpellDiscipline_SpiritLaw, 
        ControlCodes.SpellDiscipline_Supplication 
    };
    
    private static byte[] MagicianDisciplines = new byte[] { 
        ControlCodes.SpellDiscipline_EarthLaw,  
        ControlCodes.SpellDiscipline_FireLaw, 
        ControlCodes.SpellDiscipline_IceLaw,
        ControlCodes.SpellDiscipline_Shielding
    };
    
    private static byte[] MentalistDisciplines = new byte[] { 
        ControlCodes.SpellDiscipline_Brilliance,
        ControlCodes.SpellDiscipline_Displacement,
        ControlCodes.SpellDiscipline_Psionics,
        ControlCodes.SpellDiscipline_Shielding
    };

    public static object[] Params {  
        get { return _params; } 
        set { _params = value; }
    }
    public static void FaceCamera(GameObject go)
    {
        go.transform.LookAt(Camera.main.transform.position);
    }
    public static float AngleBetween(Transform objectA, Transform objectB)
    {
        Vector3 direction = (objectB.position - objectA.position).normalized;
        return Vector3.SignedAngle(direction, objectA.forward, Vector3.up);
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

    public static bool BoxCast(Transform origin, int layerMask, float distance, Vector3 direction, out RaycastHit hitInfo)
    {
        return Physics.BoxCast(origin.position, Vector3.one, direction, out hitInfo);
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

    public static bool WasPCHit(Collider other)
    {
        return other.GetComponent<PC>() != null;
    }
    public static bool WasRemoteHit(Collider other, out Avatar remote)
    {
        remote = other.GetComponent<Avatar>();
        return remote != null;
    }
    public static void SetLayerRecursive(GameObject gameObject, LayerMask newLayer)
    {
        gameObject.layer = newLayer;
        foreach(Transform child in gameObject.transform)
        {
            SetLayerRecursive(child.gameObject, newLayer);
        }
    }
    public static byte[] DisciplinesByClass(PlayerClass playerClass)
    {
        switch (playerClass)
        {
            case PlayerClass.Arcanist:
                return ArcanistDisciplines;
            case PlayerClass.Cleric:
                return ClericDisciplines;
            case PlayerClass.Magician:
                return MagicianDisciplines;
            case PlayerClass.Mentalist:
                return MentalistDisciplines;
        }
        return null;
    }
    public static int SpellDisciplineStringReference(byte spellDiscipline)
    {
        
        switch (spellDiscipline)
        {
            case ControlCodes.SpellDiscipline_SpiritLaw:
                return 283;
            case ControlCodes.SpellDiscipline_VoidLaw:
                return 239;
            case ControlCodes.SpellDiscipline_FireLaw:
                return 229;
            case ControlCodes.SpellDiscipline_IceLaw:
                return 230;
            case ControlCodes.SpellDiscipline_ManaLaw:
                return 238;
            case ControlCodes.SpellDiscipline_Barriers:
                return 284;
            case ControlCodes.SpellDiscipline_Brilliance:
                return 232;
            case ControlCodes.SpellDiscipline_Displacement:
                return 233;
            case ControlCodes.SpellDiscipline_EarthLaw:
                return 231;
            case ControlCodes.SpellDiscipline_Psionics:
                return 234;
            case ControlCodes.SpellDiscipline_Smiting:
                return 237;
            case ControlCodes.SpellDiscipline_Supplication:
                return 235;
            case ControlCodes.SpellDiscipline_Sigils:
                return 240;
            case ControlCodes.SpellDiscipline_Shielding:
                return 285;
            case ControlCodes.SpellDiscipline_Healing:
                return 236;
        }
        return 0;
    }
    public static byte GetMaxSkillPointsForLevel(byte characterLevel)
    {
        return (byte)(3 + Math.Floor(characterLevel / 7.0));
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
    public static bool ProcessVector3Lerp(ref float elapsed, float lerpPeriod, Vector3 starting, Vector3 ending, Transform mover, bool local, bool position)
    {
        elapsed += Time.deltaTime;
        float percentComplete = elapsed / lerpPeriod;
        if (percentComplete >= 1.0f)
        {
            percentComplete = 1.0f;
            elapsed = 0.0f;
        }
        Vector3 result = Vector3.Lerp(starting, ending, percentComplete);
        
        if (local)
        {
            if (position)
            {
                mover.localPosition = result;
            }
            else
            {
                mover.localEulerAngles = result;
            }
        }
        else
        {
            if (position)
            {
                mover.position = result;
            }
            else
            {
                mover.eulerAngles = result;
            }
        }
        if(percentComplete == 1.0f)
        {
            return true;
        }
        return false;
    }
    public static bool ProcessRotation(float rotationAmount, Transform toRotate, ref float elapsed, float lerpPeriod)
    {
        float delta = Time.deltaTime;
        if((elapsed + delta) >= lerpPeriod)
        {
            delta = lerpPeriod - elapsed;
            elapsed = lerpPeriod;
        }
        else
        {
            elapsed += delta;
        }
        float frameRotation = rotationAmount * (delta / lerpPeriod);
        //Debug.Log("Frame Rotation: " + frameRotation);
        toRotate.Rotate(0, frameRotation, 0);
        if (elapsed == lerpPeriod)
        {
            elapsed = 0.0f;
            return true;
        }
        return false;
    }
    public static bool GetPHPString(string function, out string contents)
    {
        contents = "";
        using (HttpClient client = new HttpClient())
        {
            try
            {
                client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true
                };
                Task<string> t = client.GetStringAsync("https://www.fosiemods.net/ms2.php?func="+ function + "&appid=ms2");
                contents = t.Result;
                return true;
            }
            catch(System.Exception e)
            {

            }
        }
        return false;
    }

    public static int DisciplineTableToInt(Dictionary<byte, byte> table)
    {
        bool[] skillsArray = new bool[32];
        foreach(byte key in table.Keys)
        {
            ByteUtils.FillBooleanArray(ref skillsArray, table[key], ((byte)key) * 2);
        }
        int skillsInteger = 0;
        for(int i = 0; i < skillsArray.Length; i++)
        {
            if (skillsArray[i])
            {
                skillsInteger += (int)Math.Pow(2, i);
            }
        }
        return skillsInteger;
    }

    public static int RandomInt(int min, int max)
    {
        return _random.Next(min, max);
    }
}
