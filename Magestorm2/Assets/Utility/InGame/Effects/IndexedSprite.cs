using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class IndexedSprite : IComparable<IndexedSprite>
{
    private Sprite _sprite;
    private int _index;

    public IndexedSprite(Sprite sprite, int index)
    {
        _sprite = sprite; 
        _index = index;
    }

    public int Index
    {
        get
        {
            return _index;
        }
    }

    public Sprite Sprite
    {
        get { return _sprite; }
    }

    public int CompareTo(IndexedSprite other)
    {
        return _index.CompareTo(other._index);
    }
}
