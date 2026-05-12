using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject AvatarPrefab;
    public GameObject DeadbodyPrefab;
    public GameObject Marker;
    private Dictionary<byte, SpellSpawner> _spellPrefabs;
    private Dictionary<short, SpawnedSpell> _spellRegistry;
    private Dictionary<byte, VFX> _vfxTable;
    private Dictionary<byte, AppliedEffect> _appliedEffects;
    private PeriodicAction _expirationCheck;
    private List<GameObject> _activeMarkers;
    private bool _showMarkers = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _activeMarkers = new List<GameObject>();
        _expirationCheck = new PeriodicAction(30.0f, ExpirationCheck, null);
        _spellRegistry = new Dictionary<short, SpawnedSpell>();
        _vfxTable = new Dictionary<byte, VFX>();
        ComponentRegister.Spawner = this;
        LoadSpellPrefabs();
        LoadVFXPrefabs();
        LoadAppliedEffectPrefabs();
    }
    void Start()
    {
        
    }
    public void ClearMarkers()
    {
        foreach (GameObject marker in _activeMarkers)
        {
            Destroy(marker);
        }
        _activeMarkers.Clear();
    }
    public void MarkerToggle()
    {
        _showMarkers = !_showMarkers;
    }
    public void SpawnMarker(Vector3 position, float scale)
    {
        if (!_showMarkers)
        {
            return;
        }
        GameObject marker = Instantiate(Marker);
        marker.transform.position = position;
        marker.transform.localScale = new Vector3(scale, scale, scale);
        _activeMarkers.Add(marker);
    }
    public void SpawnVFX(byte vfxCode, Vector3 position)
    {
        if (_vfxTable.ContainsKey(vfxCode))
        {
            VFX vfx = Instantiate(_vfxTable[vfxCode]);
            vfx.transform.position = position;
        }
    }
    public void SpawnVFX(byte vfxCode, Transform parent)
    {
        GameObject spawned = null;
        SpawnVFX(vfxCode, parent, ref spawned);
    }
    public void SpawnVFX(byte vfxCode, Transform parent, ref GameObject spawned)
    {
        Debug.Log("SpawnVFX: " + vfxCode);
        if (_vfxTable.ContainsKey(vfxCode))
        {
            VFX vfx = Instantiate(_vfxTable[vfxCode]);
            vfx.transform.parent = parent;
            vfx.transform.localPosition = Vector3.zero;
            spawned = vfx.gameObject;
            Debug.Log("VFX " + vfxCode + " successfully spawned.");
        }
        else
        {
            Debug.Log("No VFX for code " + vfxCode);
        }
    }
   
    private void ExpirationCheck()
    {
        float currentTime = Time.realtimeSinceStartup;
        foreach(SpawnedSpell spell in _spellRegistry.Values)
        {
            if (spell.IsExpired(currentTime))
            {
                spell.MarkForDestruction();
            }
        }
    }
    private void LoadVFXPrefabs()
    {
        _vfxTable = new Dictionary<byte, VFX>();
        VFX[] vfxContainers = Resources.LoadAll<VFX>("VFX");
        Debug.Log("LoadVFXPrefabs(): VFX count: " + vfxContainers.Length);
        foreach (VFX vfx in vfxContainers)
        {
            _vfxTable.Add(vfx.VFXCode, vfx);
            Debug.Log("LoadVFXPrefabs(): Loaded " + vfx.VFXCode + ", " + vfx.name);
        }
    }
    private void LoadAppliedEffectPrefabs()
    {
        _appliedEffects = new Dictionary<byte, AppliedEffect>();
        AppliedEffect[] effects = Resources.LoadAll<AppliedEffect>("AppliedEffects");
        Debug.Log("LoadAppliedEffectPrefabs(): AE count: " + effects.Length);
        foreach (AppliedEffect ae in effects)
        {
            _appliedEffects.Add(ae.EffectCode, ae);
            Debug.Log("LoadAppliedEffectPrefabs(): Loaded " + ae.EffectCode + ", " + ae.name);
        }
    }
    private void LoadSpellPrefabs()
    {
        _spellPrefabs = new Dictionary<byte, SpellSpawner>();
        SpellSpawner[] spawners = Resources.LoadAll<SpellSpawner>("SpellPrefabs");
        foreach (SpellSpawner spawner in spawners)
        {
            _spellPrefabs.Add(spawner.SpellKey, spawner);
        }
    }
    // Update is called once per frame
    void Update()
    {
        _expirationCheck.ProcessAction(Time.deltaTime);
    }
    public void DeregisterSpawnedSpell(SpawnedSpell toDeregister)
    {
        _spellRegistry.Remove(toDeregister.CastID);
    }
    public void RegisterSpawnedSpell(SpawnedSpell toRegister)
    {
        _spellRegistry.Add(toRegister.CastID, toRegister);
    }
    public bool SpawnAppliedEffect(byte effectCode, ref AppliedEffect effect)
    {
        if(_appliedEffects == null)
        {
            Debug.Log("AE is null");
        }
        if (_appliedEffects.ContainsKey(effectCode))
        {
            effect = Instantiate(_appliedEffects[effectCode]);
            return true;
        }
        else
        {
            Debug.Log("No AE for code: " + effectCode);
        }
            return false;
    }
    public bool SpawnSpellPrefab(byte spellKey, ref SpellSpawner spawner)
    {
        if (_spellPrefabs.ContainsKey(spellKey))
        {
            spawner = Instantiate(_spellPrefabs[spellKey]);
            return true;
        }
        return false;
    }
    public Avatar SpawnAvatar()
    {
        return Instantiate(AvatarPrefab).GetComponent<Avatar>();
    }
    public GameObject SpawnDeadBody(GameObject model, Vector3 position, float yRotation, RuntimeAnimatorController deathAnim, Avatar deadPlayer)
    {
        GameObject deadBody = Instantiate(DeadbodyPrefab);
        DeadBody db = deadBody.GetComponent<DeadBody>();
        db.Initialize(model, deadBody.transform);
        deadBody.transform.position = position;
        deadBody.transform.eulerAngles = new Vector3(0, yRotation, 0);
        SharedFunctions.SetLayerRecursive(deadBody, LayerManager.DeadBodyLayer);
        Animator anim = deadBody.GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = deathAnim;
        deadPlayer.SetDeadBody(db);
        return deadBody;
    }


}
