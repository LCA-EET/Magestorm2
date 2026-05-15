using System.Collections.Generic;
public class DisciplineManager
{
    private static bool _init;
    private static Dictionary<byte, DisciplineData> _disciplines;
    public static void Init()
    {
        if (!_init)
        {
            _disciplines = new Dictionary<byte, DisciplineData>();
            string contents;
            if (SharedFunctions.GetPHPString("disciplines", out contents))
            {
                string[] fieldSplit = contents.Split("[FIELD]");
                string[] fields = fieldSplit[0].Split("<br>");
                string[] spelldata = fieldSplit[1].Split("[DISCIPLINE]");
                for (int i = 0; i < spelldata.Length - 1; i++)
                {
                    DisciplineData toAdd = new DisciplineData(fields, spelldata[i]);
                    _disciplines.Add(toAdd.DisciplineID, toAdd);
                }

            }
        }
    }

    public static DisciplineData GetDiscipline(byte disciplineID)
    {
        return _disciplines[disciplineID];
    }
    public static List<SpellData> GetSpellsOfDiscipline(byte[] disciplines)
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
