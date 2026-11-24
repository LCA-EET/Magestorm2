using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;
namespace Magus
{

    public static class IconLibrary
    {
        private static Dictionary<byte, SpriteSet> _atlas;
        public static void Init()
        {
            _atlas = new Dictionary<byte, SpriteSet>();
            LoadSprites("icon/effects/forward", false);
            LoadSprites("icon/effects/reverse", true);
            foreach (SpriteSet spriteSet in _atlas.Values)
            {
                spriteSet.Sort();
            }
        }

        private static void LoadSprites(string path, bool reverse)
        {
            string name;
            Sprite[] loaded = Resources.LoadAll<Sprite>(path);
            foreach (Sprite sprite in loaded)
            {
                name = sprite.name;
                if (name.Contains("_"))
                {
                    string[] splitName = name.ToLower().Split('_');
                    byte setID = byte.Parse(splitName[0]);
                    if (!_atlas.ContainsKey(setID))
                    {
                        _atlas.Add(setID, new SpriteSet(sprite, reverse));
                    }
                    else
                    {
                        _atlas[setID].AppendSprite(sprite, int.Parse(splitName[1]));
                    }
                }
                else
                {
                    _atlas.Add(byte.Parse(name), new SpriteSet(sprite, reverse));
                }
            }
        }

        public static SpriteSet GetSpriteSet(byte setID)
        {
            return _atlas[setID];
        }
    }

}