using System.Collections.Generic;
using UnityEngine;
public class SpriteSet 
{
    private List<IndexedSprite> _sprites;
    private int _nextIndex = 0;
    public SpriteSet(Sprite singleSprite)
    {
        _sprites = new List<IndexedSprite>();
        _sprites.Add(new IndexedSprite(singleSprite, 0));
    }

    public void AppendSprite(Sprite toAppend, int index)
    {
        _sprites.Add(new IndexedSprite(toAppend, index));
    }

    public List<IndexedSprite> Sprites
    {
        get
        {
            return _sprites;
        }
    }

    public void Sort()
    {
        _sprites.Sort();
    }

    public Sprite GetNextSprite()
    {
        Sprite toReturn = _sprites[_nextIndex].Sprite;
        _nextIndex = (_nextIndex == _sprites.Count - 1) ? 0 :_nextIndex + 1;
        return toReturn;
    }
}
