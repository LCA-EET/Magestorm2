using System.Collections.Generic;

public class CharacterClassManager
{
    private static bool _init;
    private static Dictionary<byte, CharacterClassData> _characterClasses;
    private static Dictionary<byte, List<DisciplineData>> _disciplinesOfClass;
    private static List<byte> _classCodes;
    public static void Init()
    {
        if (!_init)
        {
            _classCodes = new List<byte>();
            _characterClasses = new Dictionary<byte, CharacterClassData>();
            _disciplinesOfClass = new Dictionary<byte, List<DisciplineData>>();
            string contents;
            if (SharedFunctions.GetPHPString("characterclasses", out contents))
            {
                string[] fieldSplit = contents.Split("[FIELD]");
                string[] fields = fieldSplit[0].Split("<br>");
                string[] spelldata = fieldSplit[1].Split("[CHARACTERCLASS]");
                
                for (int i = 0; i < spelldata.Length - 1; i++)
                {
                    CharacterClassData toAdd = new CharacterClassData(fields, spelldata[i]);
                    _characterClasses.Add(toAdd.CharacterClassID, toAdd);
                    _classCodes.Add(toAdd.CharacterClassID);
                }
            }
        }
    }
    public static byte[] GetDisciplineCodesOfClass(byte classCode)
    {
        List<byte> toReturn = new List<byte>();
        List<DisciplineData> data = _disciplinesOfClass[classCode];
        foreach (DisciplineData toAdd in data) {
            toReturn.Add(toAdd.DisciplineID);
        }
        return toReturn.ToArray();
    }
    public static DisciplineData[] GetDisciplinesOfClass(byte classCode)
    {
        return _disciplinesOfClass[classCode].ToArray();
    }
    public static void AssociateDisciplineToClass(byte classCode, DisciplineData discipline)
    {
        if (!_disciplinesOfClass.ContainsKey(classCode))
        {
            _disciplinesOfClass.Add(classCode, new List<DisciplineData>());
        }
        _disciplinesOfClass[classCode].Add(discipline);
    }
    public static CharacterClassData GetCharacterClassData(byte classCode)
    {
        return _characterClasses[classCode];
    }
    public static List<byte> CharacterClassCodes
    {
        get
        {
            return _classCodes;
        }
    }
}
