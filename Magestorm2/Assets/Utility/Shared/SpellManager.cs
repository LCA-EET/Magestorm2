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

    public static SpellData GetSpell(byte key)
    {
        return _spells[key];
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
