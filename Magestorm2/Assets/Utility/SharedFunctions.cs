using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class SharedFunctions
{
    private static Dictionary<byte, byte> _elementShieldEffects;
    private static Dictionary<byte, int> _spellTypeToStringReference;
    private static System.Random _random = new System.Random();
    private static object[] _params;

    public static object[] Params {  
        get { return _params; } 
        set { _params = value; }
    }

    public static int CompareVectors(Vector3 a, Vector3 b)
    {
        int toReturn = a.x.CompareTo(b.x);
        if(toReturn == 0)
        {
            toReturn = a.y.CompareTo(b.y);
            if(toReturn == 0)
            {
                toReturn = a.z.CompareTo(b.z);
            }
        }
        return toReturn;
    }
    public static void Initialize()
    {
        _spellTypeToStringReference = new Dictionary<byte, int>();
        _spellTypeToStringReference.Add(ControlCodes.SpellTypes_Sigil, 393);
        _spellTypeToStringReference.Add(ControlCodes.SpellTypes_NonSolidWall, 394);
        _spellTypeToStringReference.Add(ControlCodes.SpellTypes_SolidWall, 394);

        _elementShieldEffects = new Dictionary<byte, byte>();
        _elementShieldEffects.Add(ControlCodes.Element_Earth, ControlCodes.EffectCode_EarthShield);
        _elementShieldEffects.Add(ControlCodes.Element_Fire, ControlCodes.EffectCode_FireShield);
        _elementShieldEffects.Add(ControlCodes.Element_Ice, ControlCodes.EffectCode_IceShield);
        _elementShieldEffects.Add(ControlCodes.Element_Electric, ControlCodes.EffectCode_ElectricShield);
    }
    public static byte IsShieldedFromElement(byte element, Avatar toCheck)
    {
        if (_elementShieldEffects.ContainsKey(element))
        {
            byte shieldID = _elementShieldEffects[element];
            if (toCheck.IsEffectActive(shieldID))
            {
                return shieldID;
            }
        }
        return 0;
    }
    public static bool AdvanceSphereCast(Transform origin, float distanceToAdvance, int hitMask, float radius, out RaycastHit hitInfo)
    {
        if(Physics.SphereCast(origin.position, radius, origin.forward, out hitInfo, distanceToAdvance, hitMask))
        {
            return true;
        }
        return false;
    }
    public static bool AdvanceCast(Transform origin, float distanceToAdvance, int hitMask, out RaycastHit hitInfo)
    {
        if (Physics.Raycast(origin.position, origin.forward, out hitInfo, distanceToAdvance, hitMask))
        {
            return true;
        }
        return false;
    }
    public static float AngleBetween(Transform objectA, Transform objectB)
    {
        Vector3 direction = (objectB.position - objectA.position).normalized;
        return Vector3.SignedAngle(direction, objectA.forward, Vector3.up);
    }
    public static bool DirectionalSphereCast(Transform origin, float radius, int layerMask, float distance, Vector3 direction, out RaycastHit hitInfo)
    {
        return Physics.SphereCast(origin.position, radius, origin.TransformDirection(direction), out hitInfo, distance, layerMask);
    }
    public static bool DirectionalCast(Transform origin, int layerMask, float distance, Vector3 direction, out RaycastHit hitInfo)
    {
        return Physics.Raycast(origin.position, origin.TransformDirection(direction), out hitInfo, distance, layerMask);
    }
    public static bool IsPlayerInRadius(Vector3 origin, float radius)
    {        
        Vector3 playerPosition = ComponentRegister.PC.transform.position;
        float distance = Vector3.Distance(origin, playerPosition);
        if (distance <= radius)
        {
            Debug.Log("Player is within the blast radius.");
            Vector3 direction = DirectionVector(origin, playerPosition);
            RaycastHit hitInfo;
            bool obstructed = Physics.Raycast(origin, direction, out hitInfo, distance, LayerManager.AoEObstructionMask);
            if (obstructed)
            {
                Debug.Log("Player is obstructed by " + hitInfo.collider.gameObject.name);
            }
            return !obstructed;
        }
        else
        {
            Debug.Log("Player is outside of blast radius. Distance = " + distance + ", radius = " + radius);
        }
        return false;        
    }
    public static void CameraShake(SpellData spellData)
    {
        byte playerStrength = PlayerAccount.SelectedCharacter.GetStat(ControlCodes.PlayerStats_Strength);
        if(playerStrength < spellData.ShakePrevention)
        {
            ComponentRegister.MainCamera.Shake();
        }
    }
    public static void FillCameraDirectionBytes(ref byte[] toFill, int index)
    {
        byte[] cameraDirectionBytes = GetCameraDirectionBytes();
        //Debug.Log("Direction1: " + Camera.main.transform.forward.ToString());
        cameraDirectionBytes.CopyTo(toFill, index);
    }
    public static bool IsPlayerAvatar(Avatar toCheck)
    {
        return toCheck.PlayerID == MatchParams.IDinMatch;
    }
    public static byte[] GetCameraPositionBytes(float forwardDistance)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition += Camera.main.transform.forward * forwardDistance;
        return ByteUtils.Vector3ToBytes(cameraPosition);
    }
    public static byte[] GetCameraDirectionBytes()
    {
        return ByteUtils.Vector3ToBytes(Camera.main.transform.forward);
    }

    public static byte GetPlayerInSphereCast(Vector3 origin, float range, float radius, TeamSelectionCode tsc, int obstructionMask)
    {
        GameObjectDistanceComparer comparer = new GameObjectDistanceComparer(Game.PCAvatar.transform.position);
        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, Camera.main.transform.forward, range, LayerManager.RemotePlayerLayerMask);
        Debug.Log("Number hits in spherecast: " + hits.Length);
        if(hits.Length > 0)
        {
            List<GameObject> toProcess = new List<GameObject>();
            for (int i = 0; i < hits.Length; i++)
            {
                toProcess.Add(hits[i].collider.gameObject);
            }
            toProcess.Sort(comparer);
            for (int i = 0; i < toProcess.Count; i++)
            {
                GameObject toCheck = toProcess[i];
                if (!IsObstructed(origin, toCheck.transform.position, obstructionMask))
                {
                    Avatar unobstructed = toCheck.GetComponentInChildren<Avatar>();
                    
                    if (tsc == TeamSelectionCode.Friendly)
                    {
                        if(unobstructed.PlayerTeam == MatchParams.MatchTeam)
                        {
                            return unobstructed.PlayerID;
                        }
                    }
                    else if (tsc == TeamSelectionCode.Enemy)
                    {
                        if (unobstructed.PlayerTeam != MatchParams.MatchTeam || MatchParams.MatchTeam == Team.Neutral)
                        {
                            return unobstructed.PlayerID;
                        }
                    }
                    else
                    {
                        return unobstructed.PlayerID;
                    }
                }
            }
        }
        return 0;
    }
    public static byte[] GetPlayerIDsInRadius(Vector3 origin, float radius, bool livingPlayers)
    {
        List<byte> playerIDs = new List<byte>();
        List<GameObject> toProcess = GetObjectsInRadius(origin, radius, livingPlayers ? LayerManager.PlayerLayerMask : LayerManager.DeadPlayerLayerMask, LayerManager.AoEObstructionMask);
        foreach (GameObject obj in toProcess)
        {
            Avatar player = obj.GetComponent<Avatar>();
            if(player != null)
            {
                playerIDs.Add(player.PlayerID);
            }
        }
        return playerIDs.ToArray();
    }
    public static bool IsObstructed(Vector3 origin, Vector3 terminus, int obstructionLayerMask)
    {
        RaycastHit hitInfo;
        float distance = Vector3.Distance(terminus, origin);
        Vector3 directionVector = DirectionVector(origin, terminus);
        return Physics.Raycast(origin, directionVector, out hitInfo, distance, obstructionLayerMask);
    }
    public static List<GameObject> GetObjectsInRadius(Vector3 origin, float radius, int objectLayerMask, int obstructionLayerMask)
    {
        List<GameObject> inRadius = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(origin, radius, objectLayerMask);
        foreach (Collider collider in colliders)
        {
            GameObject go = collider.gameObject;
            
            if(obstructionLayerMask != 0)
            {   
                if(!IsObstructed(origin, go.transform.position, obstructionLayerMask))
                {
                    inRadius.Add(go);
                }
            }
            else
            {
                inRadius.Add(go);
            }
        }
        return inRadius;
    }
    public static Vector3 DirectionVector(Vector3 start, Vector3 end)
    {
        return (end - start).normalized;
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
    public static bool SphereCastForward(Transform origin, int layerMask, float radius, float distance, out RaycastHit hitInfo)
    {
        return DirectionalSphereCast(origin, radius, layerMask, distance, Vector3.forward, out hitInfo);
    }
    public static bool CastForward(Transform origin, int layerMask, float distance, out RaycastHit hitInfo)
    {
        return DirectionalCast(origin, layerMask, distance, Vector3.forward, out hitInfo);
    }
    
     public static bool WasPCHit(Collider other)
    {
        return other.GetComponent<PC>() != null && Game.PCAvatar.IsAlive;
    }
    public static bool WasRemoteHit(Collider other, out Avatar remote)
    {
        remote = other.GetComponent<Avatar>();
        return remote != null;
    }
    public static bool WasWallHit(Collider other, out Wall wall)
    {
        wall = other.GetComponent<Wall>();
        return wall != null;
    }
    public static void SetLayerRecursive(GameObject gameObject, LayerMask newLayer)
    {
        gameObject.layer = newLayer;
        foreach(Transform child in gameObject.transform)
        {
            SetLayerRecursive(child.gameObject, newLayer);
        }
    }

    public static void RotateToCamera(Transform toRotate)
    {
        toRotate.LookAt(Camera.main.transform.position);
        toRotate.Rotate(0, 180, 0);
    }
    public static byte GetMaxSkillPointsForLevel(byte characterLevel)
    {
        return (byte)(3 + Math.Floor(characterLevel / 8.0));
    }

    public static string MatchTypeString(byte matchType)
    {
        
        switch (matchType)
        {
            case ControlCodes.MatchTypes_DeathMatch:
                return Language.GetBaseString(104); //
            case ControlCodes.MatchTypes_CaptureTheFlag:
                return Language.GetBaseString(106); //
            case ControlCodes.MatchTypes_FreeForAll:
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
    public static Vector3 CalculateVector3Lerp(ref float elapsed, float lerpPeriod, Vector3 starting, Vector3 ending)
    {
        elapsed += Time.deltaTime;
        float percentComplete = elapsed / lerpPeriod;
        if (percentComplete >= 1.0f)
        {
            percentComplete = 1.0f;
            elapsed = 0.0f;
        }
        return Vector3.Lerp(starting, ending, percentComplete);
    }
    public static void ApplyVector3Lerp(Vector3 result, Transform mover, bool local, bool position)
    {
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
    }
    public static bool ProcessVector3Lerp(ref float elapsed, float lerpPeriod, Vector3 starting, Vector3 ending, Transform mover, bool local, bool position)
    {
        Vector3 result = CalculateVector3Lerp(ref elapsed, lerpPeriod, starting, ending);
        ApplyVector3Lerp(result, mover, local, position);
        return elapsed == 0.0f;
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

    public static int SpellTypeStrRef(byte spellType)
    {
        return _spellTypeToStringReference[spellType];
    }
}
