using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject AvatarPrefab;
    public GameObject DeadbodyPrefab;
    private Dictionary<byte, SpellSpawner> _spellPrefabs;
    private Dictionary<short, SpawnedSpell> _spellRegistry;
    private PeriodicAction _expirationCheck;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _expirationCheck = new PeriodicAction(30.0f, ExpirationCheck, null);
        _spellRegistry = new Dictionary<short, SpawnedSpell>();
        ComponentRegister.Spawner = this;
        LoadSpellPrefabs();
    }
    void Start()
    {
        
    }
    private void ExpirationCheck()
    {
        List<short> expired = new List<short>();
        float currentTime = Time.realtimeSinceStartup;
        foreach(SpawnedSpell spell in _spellRegistry.Values)
        {
            if (spell.IsExpired(currentTime))
            {
                expired.Add(spell.CastID);
            }
        }
        foreach(short castID in expired)
        {
            SpawnedSpell expiredSpell = _spellRegistry[castID];
            Destroy(expiredSpell.gameObject);
            _spellRegistry.Remove(castID);
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
    public GameObject SpawnDeadBody(GameObject model, Vector3 position, float yRotation, RuntimeAnimatorController deathAnim)
    {
        GameObject deadBody = Instantiate(DeadbodyPrefab);
        DeadBody db = deadBody.GetComponent<DeadBody>();
        db.Initialize(model, deadBody.transform);
        deadBody.transform.position = position;
        deadBody.transform.eulerAngles = new Vector3(0, yRotation, 0);
        SharedFunctions.SetLayerRecursive(deadBody, LayerManager.DeadBodyLayer);
        Animator anim = deadBody.GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = deathAnim;
        return deadBody;
    }


}
