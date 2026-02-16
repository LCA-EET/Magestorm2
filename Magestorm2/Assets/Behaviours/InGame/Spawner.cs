using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject AvatarPrefab;
    public GameObject DeadbodyPrefab;
    private Dictionary<byte, SpellSpawner> _spellPrefabs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        ComponentRegister.Spawner = this;
        LoadSpellPrefabs();
    }
    void Start()
    {
        
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
