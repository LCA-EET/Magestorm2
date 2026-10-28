using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClassManager
{
    private Dictionary<byte, CharacterClassData> _characterClasses;
    private Dictionary<byte, List<DisciplineData>> _disciplinesOfClass;
    private List<byte> _classCodes;
    public CharacterClassManager()
    {
        _classCodes = new List<byte>();
        _characterClasses = new Dictionary<byte, CharacterClassData>();
        _disciplinesOfClass = new Dictionary<byte, List<DisciplineData>>();
        Game.ActiveCharacterClassManager = this;
    }
    public bool ProcessClassData(byte[] decrypted)
    {
        byte numToProcess = decrypted[1];
        Debug.Log("Classes to process: " + numToProcess);
        int index = 2;
        List<byte[]> classData = new List<byte[]>();
        for (int i = 0; i < numToProcess; i++)
        {
            byte[] data = new byte[6];
            Array.Copy(decrypted, index, data, 0, data.Length);
            classData.Add(data);
            index += data.Length;
        }
        if (Cryptographer.VerifyChecksum(decrypted, index))
        {
            foreach (byte[] data in classData)
            {
                CharacterClassData toAdd = new CharacterClassData(data);
                _characterClasses.Add(toAdd.CharacterClassID, toAdd);
                _classCodes.Add(toAdd.CharacterClassID);
            }
            return true;
        }
        return false;
    }
    public byte[] GetDisciplineCodesOfClass(byte classCode)
    {
        List<byte> toReturn = new List<byte>();
        List<DisciplineData> data = _disciplinesOfClass[classCode];
        foreach (DisciplineData toAdd in data) {
            toReturn.Add(toAdd.DisciplineID);
        }
        return toReturn.ToArray();
    }
    public  DisciplineData[] GetDisciplinesOfClass(byte classCode)
    {
        return _disciplinesOfClass[classCode].ToArray();
    }
    public  void AssociateDisciplineToClass(byte classCode, DisciplineData discipline)
    {
        Debug.Log("AssociateDisciplineToClass: " + classCode + ", " + discipline.DisciplineName);
        if (!_disciplinesOfClass.ContainsKey(classCode))
        {
            _disciplinesOfClass.Add(classCode, new List<DisciplineData>());
        }
        _disciplinesOfClass[classCode].Add(discipline);
    }
    public  CharacterClassData GetCharacterClassData(byte classCode)
    {
        return _characterClasses[classCode];
    }
    public  List<byte> CharacterClassCodes
    {
        get
        {
            return _classCodes;
        }
    }
}
