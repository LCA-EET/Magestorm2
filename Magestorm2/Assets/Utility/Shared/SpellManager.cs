using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpellManager
{
    private Dictionary<byte, SpellData> _spells;
    public bool _spellsLoaded;
    public SpellManager()
    {
        _spells = new Dictionary<byte, SpellData>();
        Game.ActiveSpellManager = this;
    }
    public bool SpellsLoaded
    {
        get { return _spellsLoaded; }
    }
    public bool ProcessSpellData(byte[] decrypted)
    {
        int totalSpells = decrypted[1];
        int numToProcess = decrypted[2];
        Debug.Log("Processing " + numToProcess + " spells.");
        int index = 3;
        List<byte[]> spellData = new List<byte[]>();
        for(int i = 0; i < numToProcess; i++)
        {
            byte[] data = new byte[18];
            Array.Copy(decrypted, index, data, 0, 18);
            spellData.Add(data);
            index += 18;
        }
        if(Cryptographer.VerifyChecksum(decrypted, index))
        {
            foreach (byte[] data in spellData) 
            {
                SpellData toAdd = new SpellData(data);
                Debug.Log("Added spell " + toAdd.SpellID);
                if (_spells.ContainsKey(toAdd.SpellID))
                {
                    _spells[toAdd.SpellID] = toAdd;
                }
                else
                {
                    _spells.Add(toAdd.SpellID, toAdd);
                }
                byte discipline = toAdd.Discipline;

                Game.ActiveDisciplineManager.GetDiscipline(discipline).AddSpellToDiscipline(toAdd);
                if(_spells.Count == totalSpells)
                {
                    _spellsLoaded = true;
                    Debug.Log("All spells loaded.");
                    return true;
                }
            }
        }
        else
        {
            Debug.Log("ProcessSpellData: Checksum failure.");
        }
        return false;
    }
    public int GetSpellNameReference(byte key)
    {
        int toReturn = 276;
        if (_spells.ContainsKey(key))
        {
            return _spells[key].SpellNameReference;
        }
        return toReturn;
    }
    public bool GetSpell(byte key, ref SpellData spellReference)
    {
        bool toReturn = false;
        if (_spells.ContainsKey(key))
        {
            toReturn = true;
            spellReference = _spells[key];
        }
        Debug.Log("GetSpell Key: " + key + ", Result: " + toReturn);
        return toReturn;
    }
    public Dictionary<byte,SpellData> GetAvailableSpells(byte characterLevel, Dictionary<byte, byte> disciplineTable)
    {
        Dictionary<byte, SpellData> toReturn = new Dictionary<byte, SpellData>();
        foreach(byte disciplineKey in disciplineTable.Keys)
        {
            List<SpellData> toCheck = Game.ActiveDisciplineManager.GetSpellsOfDiscipline(new byte[] { disciplineKey });
            foreach(SpellData data in toCheck)
            {
                if(data.MinLevel <= characterLevel &&
                    data.SkillNeeded <= disciplineTable[disciplineKey]){
                    toReturn.Add(data.SpellID, data);
                }
            }
        }
        return toReturn;
    }
    

}
