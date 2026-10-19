using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class SpellManager
{
    private static bool _init;
    private static Dictionary<byte, SpellData> _spells;
    public static void Init()
    {
        _spells = new Dictionary<byte, SpellData>();
        /*
        if (!_init)
        {
            _spells = new Dictionary<byte, SpellData>();
            string contents = Game.LoadDatFile("spells");
            string[] fieldSplit = contents.Split("[FIELD]");
            string[] fields = fieldSplit[0].Split("\n");
            string[] spelldata = fieldSplit[1].Split("[SPELL]");
            for (int i = 0; i < spelldata.Length - 1; i++)
            {
                SpellData toAdd = new SpellData(fields, spelldata[i]);
                _spells.Add(toAdd.SpellID, toAdd);
                byte discipline = toAdd.Discipline;
                DisciplineManager.GetDiscipline(discipline).AddSpellToDiscipline(toAdd);
            }
        }
        */
    }
    public static void ProcessSpellData(byte[] decrypted)
    {
        int numToProcess = decrypted[3];
        Debug.Log("Processing " + numToProcess + " spells.");
        int index = 4;
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
                _spells.Add(toAdd.SpellID, toAdd);
                byte discipline = toAdd.Discipline;
                DisciplineManager.GetDiscipline(discipline).AddSpellToDiscipline(toAdd);
            }
            
        }
        else
        {
            Debug.Log("ProcessSpellData: Checksum failure.");
        }
    }
    public static int GetSpellNameReference(byte key)
    {
        int toReturn = 276;
        if (_spells.ContainsKey(key))
        {
            return _spells[key].SpellNameReference;
        }
        return toReturn;
    }
    public static bool GetSpell(byte key, ref SpellData spellReference)
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
    public static Dictionary<byte,SpellData> GetAvailableSpells(byte characterLevel, Dictionary<byte, byte> disciplineTable)
    {
        Dictionary<byte, SpellData> toReturn = new Dictionary<byte, SpellData>();
        foreach(byte disciplineKey in disciplineTable.Keys)
        {
            List<SpellData> toCheck = DisciplineManager.GetSpellsOfDiscipline(new byte[] { disciplineKey });
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
