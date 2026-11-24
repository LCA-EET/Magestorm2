using System.Collections.Generic;
using UnityEngine;
namespace Magus
{

    public class SpriteSet
    {
        private List<IndexedSprite> _sprites;
        private int _nextIndex = 0;
        private bool _reverse, _flip;
        public SpriteSet(Sprite singleSprite)
        {
            _sprites = new List<IndexedSprite>();
            _sprites.Add(new IndexedSprite(singleSprite, 0));
        }

        public SpriteSet(Sprite singleSprite, bool reverse) : this(singleSprite)
        {
            _reverse = reverse;
            _flip = true;
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
            if (_reverse)
            {
                if(_nextIndex == _sprites.Count - 1 || _nextIndex == 0)
                {
                    _flip = !_flip;
                }
                _nextIndex = _flip ? _nextIndex - 1 : _nextIndex + 1;
            }
            else
            {
                _nextIndex = (_nextIndex == _sprites.Count - 1) ? 0 : _nextIndex + 1;
                
            }
            return toReturn;
        }
    }

}