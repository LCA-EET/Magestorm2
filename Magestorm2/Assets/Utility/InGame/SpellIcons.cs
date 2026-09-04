using System.Collections.Generic;
using UnityEngine;
public static class SpellIcons
{
    private static Dictionary<byte, Sprite> _spellIcons;

    public static void Init()
    {
        _spellIcons = new Dictionary<byte, Sprite>();
        Sprite[] icons = Resources.LoadAll<Sprite>("icon/spells");
        for (int i = 0; i < icons.Length; i++)
        {
            //Debug.Log("Loaded icon " + icons[i].name);
            Sprite icon = icons[i];
            _spellIcons.Add(byte.Parse(icon.name), icon);
        }
    }
    public static Sprite GetIcon(byte spellKey)
    {
        if (!_spellIcons.ContainsKey(spellKey))
        {
            return _spellIcons[0];
        }
        else
        {
            return _spellIcons[spellKey];
        }
    }
}
