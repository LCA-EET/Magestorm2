using System;
using System.Collections.Generic;
using UnityEngine;
public class DisciplineManager
{
    private Dictionary<byte, DisciplineData> _disciplines;
    public DisciplineManager()
    {
        _disciplines = new Dictionary<byte, DisciplineData>();
        Game.ActiveDisciplineManager = this;
    }

    public bool ProcessDisciplineData(byte[] decrypted)
    {
        byte numToProcess = decrypted[1];
        Debug.Log("Disciplines to process: " + numToProcess);
        int index = 2;
        List<byte[]> disciplineData = new List<byte[]>();
        for (int i = 0; i < numToProcess; i++)
        {
            byte[] data = new byte[4];
            Array.Copy(decrypted, index, data, 0, data.Length);
            disciplineData.Add(data);
            index += data.Length;
        }
        if (Cryptographer.VerifyChecksum(decrypted, index))
        {
            foreach(byte[] data in disciplineData)
            {
                DisciplineData toAdd = new DisciplineData(data);
                _disciplines.Add(toAdd.DisciplineID, toAdd);
                Debug.Log("Added discipline " + toAdd.DisciplineID + ": " + toAdd.DisciplineName);
            }
            return true;
        }
        return false;
    }
    public DisciplineData GetDiscipline(byte disciplineID)
    {
        return _disciplines[disciplineID];
    }
    public List<SpellData> GetSpellsOfDiscipline(byte[] disciplines)
    {
        List<SpellData> toReturn = new List<SpellData>();
        foreach (byte discipline in disciplines)
        {
            if (_disciplines.ContainsKey(discipline))
            {
                toReturn.AddRange(_disciplines[discipline].SpellsOfDiscipline);
            }
        }
        return toReturn;
    }
}
