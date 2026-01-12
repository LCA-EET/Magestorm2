using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject AvatarPrefab;
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
}
