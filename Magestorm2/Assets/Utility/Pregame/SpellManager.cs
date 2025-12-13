using System.Collections.Generic;
using UnityEngine;
public static class SpellManager
{
    private static bool _init;
    private static Dictionary<int, SpellData> _spells;
    public static void Init()
    {
        if (!_init)
        {
            _spells = new Dictionary<int, SpellData>();
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
                }
                
            }
        }
    }

}
