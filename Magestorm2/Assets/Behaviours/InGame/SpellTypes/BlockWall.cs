using System.Collections.Generic;
using UnityEngine;
namespace Magus
{
    public class BlockWall : SpawnedSpell
    {
        public GameObject BlockPrefab;
        public BoxCollider WallCollider;
        public byte Width = 5;
        public byte Height = 5;
        private byte _row, _col;
        public float BuildTime = 1.0f;
        private bool _instant;
        private int _numBlocks, _blocksCreated;
        private PeriodicAction _placeBlock;
        private List<WallBlock> _blocks;
        private float _xAdjust, _yAdjust, _xScale, _yScale;
        public override void InitializeNoCaster(Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
        {
            base.InitializeNoCaster(castingTeam, castID, parent, spellReference, payload);
            _blocks = new List<WallBlock>();
            _xScale = BlockPrefab.transform.localScale.x;
            _yScale = BlockPrefab.transform.localScale.y;
            _xAdjust = (_xScale * Width / 2.0f) - (_xScale / 2.0f);
            _yAdjust = (_yScale * Height / 2.0f) - (_yScale / 2.0f);
            _instant = CasterID == 0;
            _numBlocks = Width * Height;
            _row = 0;
            _col = 0;
            _placeBlock = new PeriodicAction(BuildTime / _numBlocks, PlaceBlock, null);
        }
        private void DisableBlockColliders()
        {
            foreach(WallBlock block in _blocks)
            {
                block.DisableCollider();
            }
            WallCollider.size = new Vector3(_xScale * Width, _yScale * Height, BlockPrefab.transform.localScale.z);
            WallCollider.enabled = true;
            
        }
        private void PlaceBlock()
        {
            if (_col == Width)
            {
                _col = 0;
                _row++;
            }
            WallBlock block = Instantiate(BlockPrefab).GetComponent<WallBlock>();
            _blocks.Add(block);
            block.transform.eulerAngles = transform.eulerAngles;
            block.transform.SetParent(transform);
            block.transform.localPosition = new Vector3((_col * _xScale) - _xAdjust, 
                (_row * _yScale) - _yAdjust,  0);
            _col++;
            _blocksCreated++;
            if(_blocksCreated == _numBlocks)
            {
                DisableBlockColliders();
            }
        }
        private void Start()
        {
            if (_instant)
            {
                for (int i = 0; i < _numBlocks; i++)
                {
                    PlaceBlock();
                }
            }
        }
        public override void Update()
        {
            base.Update();
            if(!_instant && _blocksCreated < _numBlocks)
            {
                _placeBlock.ProcessAction(Time.deltaTime);
            }
        }

    }
}

