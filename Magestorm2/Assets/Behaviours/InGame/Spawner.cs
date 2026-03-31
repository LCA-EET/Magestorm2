using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject AvatarPrefab;
    public GameObject DeadbodyPrefab;
    private Dictionary<byte, SpellSpawner> _spellPrefabs;
    private Dictionary<short, SpawnedSpell> _spellRegistry;
    private Dictionary<byte, VFX> _vfxTable;
    private PeriodicAction _expirationCheck;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _expirationCheck = new PeriodicAction(30.0f, ExpirationCheck, null);
        _spellRegistry = new Dictionary<short, SpawnedSpell>();
        _vfxTable = new Dictionary<byte, VFX>();
        ComponentRegister.Spawner = this;
        LoadSpellPrefabs();
        LoadVFXPrefabs();
    }
    void Start()
    {
        
    }
    public void SpawnVFX(byte vfxCode, Vector3 position)
    {
        if (_vfxTable.ContainsKey(vfxCode))
        {
            VFX vfx = Instantiate(_vfxTable[vfxCode]);
            vfx.transform.position = position;
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
        foreach (VFX vfx in vfxContainers)
        {
            _vfxTable.Add(vfx.VFXCode, vfx);
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
