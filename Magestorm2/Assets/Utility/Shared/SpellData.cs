using System.Collections.Generic;
using UnityEngine;

public class SpellData
{
    private Dictionary<string, object> _data;
    public SpellData(string[] fields, string contents)
    {
        _data = new Dictionary<string, object>();
        string[] split = contents.Split("<br>");
        for (int i = 0; i < fields.Length; i++)
        {
            _data.Add(fields[i], split[i+1]);
        }
    }

    public object GetData(string key)
    {
        return _data[key];
    }
}
public static class SpellAttributes
{
    public const string ID = "id";
    public const string NAME = "spellname";
    public const string MIN_DAMAGE_PER_ROLL = "mindamageperroll";
    public const string MAX_DAMAGE_PER_ROLL = "maxdamageperroll";
    public const string MIN_HEAL_PER_ROLL = "minhealperroll";
    public const string MAX_HEAL_PER_ROLL = "maxhealperroll";
    public const string ELEMENT = "element";
    public const string COST = "cost";
    public const string SPELLTYPE = "spelltype";
    public const string DESCRIPTION = "description";
    public const string SCHOOL = "school";
    public const string SKILLNEEDED = "skillneeded";
    public const string ROLLS = "rolls";
}