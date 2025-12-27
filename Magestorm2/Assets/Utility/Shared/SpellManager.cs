using System.Collections.Generic;
public static class SpellManager
{
    private static bool _init;
    private static Dictionary<int, SpellData> _spells;
    private static Dictionary<SpellDiscipline, List<SpellData>> _spellsOfDiscipline;
    public static void Init()
    {
        if (!_init)
        {
            _spells = new Dictionary<int, SpellData>();
            _spellsOfDiscipline = new Dictionary<SpellDiscipline, List<SpellData>>();
            string contents;
            if (SharedFunctions.GetPHPString("spells", out contents))
            {
                string[] fieldSplit = contents.Split("[FIELD]");
                string[] fields = fieldSplit[0].Split("<br>");
                string[] spelldata = fieldSplit[1].Split("[SPELL]");
                for (int i = 0; i < spelldata.Length-1; i++)
                {
                    SpellData toAdd = new SpellData(fields, spelldata[i]);
                    _spells.Add(int.Parse(toAdd.GetData(SpellAttributes.ID).ToString()), toAdd);
                    SpellDiscipline discipline = (SpellDiscipline)toAdd.GetByte(SpellAttributes.DISCIPLINE);
                    if (!_spellsOfDiscipline.ContainsKey(discipline))
                    {
                        _spellsOfDiscipline.Add(discipline, new List<SpellData>());
                    }
                    _spellsOfDiscipline[discipline].Add(toAdd);
                }
                
            }
        }
    }

    public static bool GetSpell(byte key, ref SpellData spellReference)
    {
        bool toReturn = false;
        if (_spells.ContainsKey(key))
        {
            toReturn = true;
            spellReference = _spells[key];
        }
        return toReturn;
    }
    public static List<SpellData> GetAvailableSpells(byte characterLevel, Dictionary<SpellDiscipline, byte> disciplineTable)
    {
        List<SpellData> toReturn = new List<SpellData>();
        foreach(SpellDiscipline disciplineKey in disciplineTable.Keys)
        {
            List<SpellData> toCheck = GetSpellsOfDiscipline(new SpellDiscipline[] { disciplineKey });
            foreach(SpellData data in toCheck)
            {
                if(data.GetByte(SpellAttributes.MINLEVEL) <= characterLevel &&
                    data.GetByte(SpellAttributes.SKILLNEEDED) <= disciplineTable[disciplineKey]){
                    toReturn.Add(data);
                }
            }
        }
        return toReturn;
    }
    public static List<SpellData> GetSpellsOfDiscipline(SpellDiscipline[] disciplines)
    {
        List<SpellData> toReturn = new List<SpellData> ();
        foreach (SpellDiscipline discipline in disciplines)
        {
            toReturn.AddRange(_spellsOfDiscipline[discipline]);
        }
        return toReturn;
    }

}
