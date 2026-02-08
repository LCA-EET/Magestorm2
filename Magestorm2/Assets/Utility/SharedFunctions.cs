using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class SharedFunctions
{
    private static System.Random _random = new System.Random();
    private static object[] _params;

    private static SpellDiscipline[] ArcanistDisciplines = new SpellDiscipline[] { 
        SpellDiscipline.ManaLaw, 
        SpellDiscipline.VoidLaw, 
        SpellDiscipline.Sigils, 
        SpellDiscipline.Shielding 
    };

    private static SpellDiscipline[] ClericDisciplines = new SpellDiscipline[] { 
        SpellDiscipline.Barriers, 
        SpellDiscipline.Healing, 
        SpellDiscipline.Smiting, 
        SpellDiscipline.SpiritLaw, 
        SpellDiscipline.Supplication 
    };
    
    private static SpellDiscipline[] MagicianDisciplines = new SpellDiscipline[] { 
        SpellDiscipline.EarthLaw,  
        SpellDiscipline.FireLaw, 
        SpellDiscipline.IceLaw,
        SpellDiscipline.Shielding
    };
    
    private static SpellDiscipline[] MentalistDisciplines = new SpellDiscipline[] { 
        SpellDiscipline.Brilliance,
        SpellDiscipline.Displacement,
        SpellDiscipline.Psionics,
        SpellDiscipline.Shielding
    };

    public static object[] Params {  
        get { return _params; } 
        set { _params = value; }
    }
    public static void FaceCamera(GameObject go)
    {
        go.transform.LookAt(Camera.main.transform.position);
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

    public static bool WasPlayerHit(Collider other)
    {
        return other.GetComponent<PC>() != null;
    }
    public static SpellDiscipline[] DisciplinesByClass(PlayerClass playerClass)
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
    public static int SpellDisciplineStringReference(SpellDiscipline spellDiscipline)
    {
        
        switch (spellDiscipline)
        {
            case SpellDiscipline.SpiritLaw:
                return 283;
            case SpellDiscipline.VoidLaw:
                return 239;
            case SpellDiscipline.FireLaw:
                return 229;
            case SpellDiscipline.IceLaw:
                return 230;
            case SpellDiscipline.ManaLaw:
                return 238;
            case SpellDiscipline.Barriers:
                return 284;
            case SpellDiscipline.Brilliance:
                return 232;
            case SpellDiscipline.Displacement:
                return 233;
            case SpellDiscipline.EarthLaw:
                return 231;
            case SpellDiscipline.Psionics:
                return 234;
            case SpellDiscipline.Smiting:
                return 237;
            case SpellDiscipline.Supplication:
                return 235;
            case SpellDiscipline.Sigils:
                return 240;
            case SpellDiscipline.Shielding:
                return 285;
            case SpellDiscipline.Healing:
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

    public static int DisciplineTableToInt(Dictionary<SpellDiscipline, byte> table)
    {
        bool[] skillsArray = new bool[32];
        foreach(SpellDiscipline key in table.Keys)
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
