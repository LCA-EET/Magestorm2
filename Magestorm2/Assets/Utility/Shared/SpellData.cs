using System.Collections.Generic;
using UnityEngine;

public class SpellData
{
    private Dictionary<string, object> _data;
    private int _spellNameReference;
    public SpellData(string[] fields, string contents)
    {
        _data = new Dictionary<string, object>();
        string[] split = contents.Split("<br>");
        for (int i = 0; i < fields.Length; i++)
        {
            _data.Add(fields[i], split[i+1]);
        }
        _spellNameReference = GetInt(SpellAttributes.SPELL_NAME_REFERENCE);
    }
    public int SpellNameReference
    {
        get { return _spellNameReference; }
    }
    public object GetData(string key)
    {
        return _data[key];
    }

    public string GetString(string key)
    {
        return _data[key].ToString();
    }

    public byte GetByte(string key)
    {
        return byte.Parse(_data[key].ToString());
    }

    public int GetInt(string key)
    {
        return int.Parse(_data[key].ToString());
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
    public const string DISCIPLINE = "school";
    public const string SKILLNEEDED = "skillneeded";
    public const string ROLLS = "rolls";
    public const string SPELL_NAME_REFERENCE = "spellnameref";
    public const string MINLEVEL = "minlevel";
}