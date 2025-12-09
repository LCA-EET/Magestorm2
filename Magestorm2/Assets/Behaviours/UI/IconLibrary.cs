using System.Collections.Generic;
using UnityEngine;

public static class IconLibrary
{
    private static Dictionary<string, SpriteSet> _atlas;
    public static void Init()
    {
        _atlas = new Dictionary<string, SpriteSet>();
        LoadSprites("icon/effects");
        foreach(SpriteSet spriteSet in _atlas.Values)
        {
            spriteSet.Sort();
        }
    }

    private static void LoadSprites(string path)
    {
        string name;
        Sprite[] loaded = Resources.LoadAll<Sprite>(path);
        foreach(Sprite sprite in loaded)
        {
            name = sprite.name;
            if (name.Contains("_"))
            {
                string[] splitName = name.ToLower().Split('_');
                if (!_atlas.ContainsKey(splitName[0]))
                {
                    _atlas.Add(splitName[0], new SpriteSet(sprite));
                }
                else
                {
                    _atlas[splitName[0]].AppendSprite(sprite, int.Parse(splitName[1]));
                }
            }
            else
            {
                _atlas.Add(name, new SpriteSet(sprite));
            }
        }
    }

    public static SpriteSet GetSpriteSet(string setName)
    {
        return _atlas[setName];
    }
}
